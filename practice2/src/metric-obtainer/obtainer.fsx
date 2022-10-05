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

let parseWeather json : Weather =
  let sourceData = Json.deserialize<SourceJsonModel.Root> json
  let dataseries = sourceData.dataseries
  let nearestForecast = 
    dataseries 
    |> List.minBy(fun x -> x.timepoint)
  let time = DateTime(
    year = (sourceData.init.Substring(0, 4) |> Int32.Parse),
    month = (sourceData.init.Substring(4, 2) |> Int32.Parse),
    day = (sourceData.init.Substring(6, 2) |> Int32.Parse))
  { date = time; temperature = nearestForecast.temp2m }


let weather = Job.map parseWeather weatherJson

let main = 
  job {
    let! weather = weather
    // TODO
    // use connection = new NpgsqlConnection("")
    // let! inserted = connection.ExecuteAsync("INSERT INTO metrics VALUES (@date, @weather)", weather)
    ()
  }

do run main
