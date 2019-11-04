open FSharp.Data.Sql

// Learn more about F# at http://fsharp.org

let [<Literal>] connString  = "Server=localhost;Database=mysqlpoc;User=root;Password=Hello"

let [<Literal>] dbVendor    = Common.DatabaseProviderTypes.MYSQL

let [<Literal>] indivAmount = 1000

let [<Literal>] useOptTypes = true

let [<Literal>] resPath = """C:\Users\Clément\.nuget\packages\mysqlconnector\0.60.3\lib\net471"""

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
    
    printfn "Who do you want to add in mysql (press enter to skip this step) ?"

    System.Console.ReadLine() |> fun x -> if x <> "" then Some x else None
    |> Option.map (fun name -> let p = ctx.Mysqlpoc.Person.Create() in p.Name <- Some name; ctx.SubmitUpdates(); sprintf "%s has been added" name)
    |> Option.defaultValue "nothing has been added"
    |> printfn "%s"
    
    let persons = 
        ctx.Mysqlpoc.Person
        |> Seq.map (fun e -> e.Name)
        |> Seq.toList

    printfn "Hello World from F#! %A" persons
    0 // return an integer exit code
