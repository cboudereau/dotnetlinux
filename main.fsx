#if INTERACTIVE
#load ".paket/load/main.group.fsx"
#else
module Main
#endif

open FSharp.Data.Sql

let [<Literal>] ConnString  = "Server=localhost;Database=mysqlpoc;User=root;Password=Hello"

let [<Literal>] DbVendor    = Common.DatabaseProviderTypes.MYSQL

let [<Literal>] IndivAmount = 1

let [<Literal>] UseOptTypes = true

let [<Literal>] ResPath = """C:\gh\dotnetlinuxscript\lib"""

type Sql = SqlDataProvider<
            DbVendor,
            ConnString,
            ResolutionPath = ResPath,
            IndividualsAmount = IndivAmount,
            UseOptionTypes = UseOptTypes
            >

[<EntryPoint>]
let main _argv =
    let ctx = Sql.GetDataContext()
     
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