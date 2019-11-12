open FSharp.Data.Sql
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Hosting

// Learn more about F# at http://fsharp.org

let [<Literal>] connString  = "Server=localhost;Database=mysqlpoc;User=root;Password=Hello"

let [<Literal>] dbVendor    = Common.DatabaseProviderTypes.MYSQL

let [<Literal>] indivAmount = 1

let [<Literal>] useOptTypes = true

let [<Literal>] resPath = """C:\Users\Clément\.nuget\packages\mysqlconnector\0.60.3\lib\net471"""

type sql = SqlDataProvider<
            dbVendor,
            connString,
            ResolutionPath = resPath,
            IndividualsAmount = indivAmount,
            UseOptionTypes = useOptTypes
            >

let simpledemo _argv =
    let ctx = sql.GetDataContext()
    
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

open Giraffe

FSharp.Data.Sql.Common.QueryEvents.SqlQueryEvent |> Event.add (printfn "Executing SQL: %O")

let add x = 
    let ctx = sql.GetDataContext()
    let p = ctx.Mysqlpoc.Person.Create() in p.Name <- Some x

    do ctx.SubmitUpdates()
    
    ctx.Mysqlpoc.Person
    |> Seq.map (fun x -> x.Name)
    |> Seq.toArray
    |> sprintf "%i - %A" System.Threading.Thread.CurrentThread.ManagedThreadId
    
let getAll () = 
    let ctx = sql.GetDataContext()
    printfn "getAll called"
    query { 
        for p in ctx.Mysqlpoc.Person do 
        select p.Name 
    }
    |> Seq.toArray
    |> sprintf "%i - %A" System.Threading.Thread.CurrentThread.ManagedThreadId

let webApp =
    choose [
        route "/ping"   >=> text "pong"
        route "/"       >=> htmlFile "/pages/index.html"
        routef "/add/%s" (add >> text)
        route "/list"   >=> warbler (fun _ -> getAll () |> text) ]

type Startup() =
    member __.ConfigureServices (services : IServiceCollection) =
        printfn "Register default Giraffe dependencies"
        services.AddGiraffe() |> ignore

    member __.Configure (app : IApplicationBuilder)
                        (env : IWebHostEnvironment)
                        (loggerFactory : ILoggerFactory) =
        printfn "Add Giraffe to the ASP.NET Core pipeline"
        app.UseGiraffe webApp

[<EntryPoint>]
let main _ =
    WebHostBuilder()
        .UseKestrel()
        .UseStartup<Startup>()
        .Build()
        .Run()
    0