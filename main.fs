module Main

open FSharp.Data.Sql

type Sql = FSharp.Data.Sql.SqlDataProvider<
            DatabaseVendor = Common.DatabaseProviderTypes.MYSQL,
            ConnectionString ="Server=localhost;Database=mysqlpoc;User=root;Password=Hello",
            ResolutionPath = """C:\gh\dotnetlinux\lib""",
            IndividualsAmount=1000,
            UseOptionTypes = true
            >

let simpledemo _argv =
    let ctx = Sql.GetDataContext()
     
    printfn "Who do you want to add in mysql (press enter to skip this step) ?"

    System.Console.ReadLine() |> fun x -> if x <> "" then Some x else None
    |> Option.map (fun name -> 
        let p = ctx.Mysqlpoc.Person.Create()
        p.Name <- Some name
        ctx.SubmitUpdates() 
        sprintf "%s has been added" name)
    |> Option.defaultValue "nothing has been added"
    |> printfn "%s"
    
    let persons = 
        ctx.Mysqlpoc.Person
        |> Seq.map (fun e -> e.Name)
        |> Seq.toList

    printfn "Hello World from F#! %A" persons
    
    System.Console.ReadLine() |> ignore
    
    0 // return an integer exit code 

open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Hosting
open Giraffe

FSharp.Data.Sql.Common.QueryEvents.SqlQueryEvent |> Event.add (printfn "Executing SQL: %O")

let add x = 
    let ctx = Sql.GetDataContext()
    let p = ctx.Mysqlpoc.Person.Create() in p.Name <- Some x

    do ctx.SubmitUpdates()
    
    ctx.Mysqlpoc.Person
    |> Seq.map (fun x -> x.Name)
    |> Seq.toArray
    |> sprintf "%i - %A" System.Threading.Thread.CurrentThread.ManagedThreadId
    
let getAll () = 
    let ctx = Sql.GetDataContext()
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