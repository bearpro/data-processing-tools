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


type ResponseDataItem = { id: string; awarded_value_eur: string; date: DateTime }
type ResponseRoot = { total: int64; data: ResponseDataItem list}

type PolandTender = { id: int64; value: double; date: DateTime }

let weatherJson =
  Request.createUrl Get "https://tenders.guru/api/pl/tenders"
  |> Request.responseAsString

let parseData json : PolandTender list =
  let sourceData = Json.deserialize<ResponseRoot> json
  sourceData.data
  |> List.map (fun x -> { id = int64(x.id); value = float(x.awarded_value_eur); date = x.date} )


let tenders = Job.map parseData weatherJson

let main = 
  let user, password =
    match Environment.GetEnvironmentVariable("WEATHER_DB_CONN").Split(":") with
    | [|user; password |] -> user, password
    | _ -> failwith "Environment variable WEATHER_DB_CONN invalid."
    
  let connectionString = $"Server=localhost;Port=5432;Database=weather;User Id={user};Password={password};"

  job {
    use connection = new NpgsqlConnection(connectionString)

    let! lastInsertedId = connection.QueryFirstAsync<int>(
      "select max(id) from poland_tenders");

    let! tenders = 
      tenders 
      |> Job.map (Seq.where (fun x -> x.id > lastInsertedId))

    for tender in tenders do
      let! inserted = connection.ExecuteAsync(
        "INSERT INTO poland_tenders VALUES (@id, @date, @value)", 
        {| id = tender.id; date = tender.date; value = tender.value |})
      printfn $"Inserted {inserted} row, {tender}"
  }

do run main
