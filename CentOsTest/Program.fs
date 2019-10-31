open FSharp.Data.Sql

// Learn more about F# at http://fsharp.org

[<Literal>]
let connString  = "Server=localhost;Database=mysqlpoc;User=root;Password=Hello"

[<Literal>]
let dbVendor    = Common.DatabaseProviderTypes.MYSQL

[<Literal>]
let indivAmount = 1000

[<Literal>]
let useOptTypes = true

[<Literal>]
let resPath = """C:\Users\Clément\.nuget\packages\mysqlconnector\0.60.3\lib\net471"""

type sql = SqlDataProvider<
            dbVendor,
            connString,
            ResolutionPath = resPath,
            IndividualsAmount = indivAmount,
            UseOptionTypes = useOptTypes
            >
open System

let ctx = sql.GetDataContext()

[<EntryPoint>]
let main argv =
    
    printfn "Who do you want to add in mysql ?"

    System.Console.ReadLine() |> fun x -> if x <> "" then Some x else None
    |> Option.map (fun name -> let p = ctx.Mysqlpoc.Person.Create() in p.Name <- Some name; ctx.SubmitUpdates(); sprintf "%s has been added" name)
    |> Option.defaultValue "nothing has been added"
    |> printfn "%s"
    
    let persons = 
        ctx.Mysqlpoc.Person
        |> Seq.map (fun e -> e.Name)
        |> Seq.toList

    ctx.SubmitUpdates()

    printfn "Hello World from F#! %A" persons
    0 // return an integer exit code
