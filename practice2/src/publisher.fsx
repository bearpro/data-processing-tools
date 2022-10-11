#r "nuget: Npgsql"
#r "nuget: Dapper"

open Dapper
open System
open Npgsql

#nowarn "3511"

type Weather = { date: DateTime; temperature: int }

let getData = task {
    let user, password =
        match Environment.GetEnvironmentVariable("WEATHER_DB_CONN").Split(":") with
        | [|user; password |] -> user, password
        | _ -> failwith "Environment variable WEATHER_DB_CONN invalid."

    let connectionString = $"Server=localhost;Port=5432;Database=weather;User Id={user};Password={password};"

    use connection = new NpgsqlConnection(connectionString)
    return! connection.QueryAsync<Weather>(
        "select * from metrics where date > @minDate order by date limit 16",
        {| minDate = DateTime.Now |})
}

let seqEach n s = 
    seq {
        let mutable counter = 0
        for item in s do
            if counter % n = 0 then
                yield item
            counter <- counter + 1
    }

let postChart data =
    let chartType = "cht=ls"
    let chartSize = "chs=600x300"
    let axis = "chxt=x,y"
    let y = "chd=t:" + String.Join(',', [ for item in data -> item.temperature ])
    let x = "chxl=0:|" + String.Join('|', [ 
        for item in seqEach 5 data -> 
            System.Net.WebUtility.UrlEncode(item.date.ToString("dd MMM hh:mm")) ])
    
    let urlBase = "https://image-charts.com/chart?"
    let chartData =  String.Join('&', [chartType; y; x; axis; chartSize])
    printfn "%s%s" urlBase chartData
    ()


let main = task {
    let! data = getData

    postChart data
}

main.Wait()