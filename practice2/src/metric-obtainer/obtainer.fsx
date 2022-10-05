#r "nuget: Http.fs"
#r "nuget: Hopac"
#r "nuget: Dapper.FSharp"
#r "nuget: Npgsql"
#r "nuget: FSharp.Json"

open System
open Hopac
open HttpFs.Client
open Dapper
open Npgsql
open FSharp.Json

module SourceJsonModel = 
  type DataItem = { timepoint: int; temp2m: int }
  type Root = { product: string; init: string; dataseries: DataItem list}

module TargetModel =
  type Weather = { date: DateTime; temperature: int }

let weatherJson =
  Request.createUrl Get "https://www.7timer.info/bin/meteo.php?lon=37.5&lat=55.7&ac=0&unit=metric&output=json&tzshift=0"
  |> Request.responseAsString

open TargetModel

let parseWeather json : Weather list =
  let sourceData = Json.deserialize<SourceJsonModel.Root> json
  let dataseries = sourceData.dataseries
  
  dataseries 
  |> List.map (fun forecast -> 
      let time = DateTime(
        year = (sourceData.init.Substring(0, 4) |> Int32.Parse),
        month = (sourceData.init.Substring(4, 2) |> Int32.Parse),
        day = (sourceData.init.Substring(6, 2) |> Int32.Parse))
      let hourOffset = sourceData.init.Substring(8, 2) |> Int32.Parse

      let time = time.AddHours (float (forecast.timepoint + hourOffset))
      { date = time; temperature = forecast.temp2m }
    )


let forecast = Job.map parseWeather weatherJson

let main = 
  let [|user; password |] = 
    Environment.GetEnvironmentVariable("WEATHER_DB_CONN").Split(":")
  let connectionString = $"Server=localhost;Port=5432;Database=weather;User Id={user};Password={password};"
  
  job {
    use connection = new NpgsqlConnection(connectionString)

    let! forecast = forecast
    let firstMetric = forecast |> List.minBy (fun x -> x.date)
    
    let! deleted = connection.ExecuteAsync(
      "DELETE FROM metrics WHERE date >= @min_date", 
      {| min_date = firstMetric.date |} )
    printfn $"Deleted {deleted} rows"

    for weather in forecast do
      let! inserted = connection.ExecuteAsync(
        "INSERT INTO metrics VALUES (@date, @temperature)", 
        {| date = weather.date; temperature = weather.temperature |})
      printfn $"Inserted {inserted} row, {weather}"
  }

do run main
