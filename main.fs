#if INTERACTIVE
#load ".paket/load/main.group.fsx"
#else
module Main
#endif

open FSharp.Data.Sql

let [<Literal>] ResPath = __SOURCE_DIRECTORY__ + "/lib"

type Sql = FSharp.Data.Sql.SqlDataProvider<
            DatabaseVendor = Common.DatabaseProviderTypes.MYSQL,
            ConnectionString ="Server=localhost;Database=data;User=sqluser;Password=sqluserpwd",
            ResolutionPath = ResPath,
            IndividualsAmount=1000,
            UseOptionTypes = true
            >

open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Giraffe

FSharp.Data.Sql.Common.QueryEvents.SqlQueryEvent |> Event.add (printfn "Executing SQL: %O")

let connString = 
    System.Environment.GetEnvironmentVariable("MYSQL_CONNSTRING")
    |> Option.ofObj
    |> Option.defaultValue "Server=localhost;Database=mysqlpoc;User=root;Password=Hello"

printfn "using %s connString" connString

let add x = 
    let ctx = Sql.GetDataContext(connString)
    let p = ctx.Data.Person.Create() in p.Name <- Some x

    do ctx.SubmitUpdates()
    
    ctx.Data.Person
    |> Seq.map (fun x -> x.Name)
    |> Seq.toArray
    |> sprintf "%i - %A" System.Threading.Thread.CurrentThread.ManagedThreadId
    
let getAll () = 
    let ctx = Sql.GetDataContext(connString)
    printfn "getAll called"
    query { 
        for p in ctx.Data.Person do 
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