#if INTERACTIVE
#load ".paket/load/main.group.fsx"
#else
module Main
#endif

open FSharp.Data.Sql

let [<Literal>] ResPath = __SOURCE_DIRECTORY__ + "/lib"

type Sql = SqlDataProvider<
            DatabaseVendor = Common.DatabaseProviderTypes.MYSQL,
            ConnectionString ="Server=localhost;Database=data;User=sqluser;Password=sqluserpwd",
            ResolutionPath = ResPath,
            IndividualsAmount=1000,
            UseOptionTypes = true
            >

Common.QueryEvents.SqlQueryEvent |> Event.add (printfn "Executing SQL: %O")

open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http

open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration

open Giraffe
open FSharp.Control.Tasks.V2.ContextInsensitive

let connString = 
    System.Environment.GetEnvironmentVariable("MYSQL_CONNSTRING")
    |> Option.ofObj
    |> Option.defaultValue "Server=localhost;Database=data;User=root;Password=Hello"

printfn "using %s connString" connString

type [<CLIMutable>] Person =
    {
        Name : string
    }

let getAll () = 
    let ctx = Sql.GetDataContext(connString)
    printfn "getAll called"
    query { 
        for p in ctx.Data.Person do 
        select p.Name 
    }
    |> Seq.toArray
    |> sprintf "%i - %A" System.Threading.Thread.CurrentThread.ManagedThreadId

module Person = 
    let create name = { Name=name }

module Sql = 
    module Person = 
        let add (person:Person) =
            let ctx = Sql.GetDataContext(connString)
            ctx.Data.Person.Create(Name=Some person.Name) |> ignore 

            do ctx.SubmitUpdates()

        let list () = 
            let ctx = Sql.GetDataContext(connString)
            ctx.Data.Person
            |> Seq.choose(fun p -> p.Name |> Option.map Person.create) 
            |> Seq.toArray
        
        let tryGetByName name = 
            let ctx = Sql.GetDataContext(connString)
            ctx.Data.Person
            |> Seq.tryFind (fun p -> p.Name = Some name)
            |> Option.bind (fun p -> p.Name)
            |> Option.map Person.create

module Handler =
    module Person =
        let add : HttpHandler =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                task {
                    let! person = ctx.BindJsonAsync<Person>()
                    Sql.Person.add person

                    return! Successful.NO_CONTENT next ctx
                } 

        let get name = 
            fun (next : HttpFunc) (ctx : HttpContext) ->
                task {
                    match Sql.Person.tryGetByName name with
                    | Some p -> return! Successful.ok (json p) next ctx
                    | None -> return! RequestErrors.NOT_FOUND (sprintf "%s not found" name) next ctx
                }
        let list = 
            fun next ctx ->
                task {
                    let l = Sql.Person.list ()
                    return! Successful.OK l next ctx
                }
let webApp =
    choose [
        GET >=> route "/ping" >=> text "pong"

        subRoute "/person" (choose [
            GET >=> choose [
                route "s" >=> Handler.Person.list
                routef "/%s" Handler.Person.get
            ]

            POST >=> Handler.Person.add
        ])
    ]

[<EntryPoint>]
let main args =
    let config = ConfigurationBuilder().AddCommandLine(args).Build();
    WebHostBuilder()
        .UseKestrel()
        .Configure(fun app -> 
            printfn "Add Giraffe to the ASP.NET Core pipeline"
            app.UseGiraffe webApp)
        .ConfigureServices(fun services -> 
            printfn "Register default Giraffe dependencies"
            services.AddGiraffe() |> ignore)
        .UseConfiguration(config)
        .ConfigureLogging(fun logging -> logging.AddConsole() |> ignore)
        .Build()
        .Run()
    0