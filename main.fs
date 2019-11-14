module Main

open FSharp.Data.Sql

type Sql = FSharp.Data.Sql.SqlDataProvider<
            DatabaseVendor = Common.DatabaseProviderTypes.MYSQL,
            ConnectionString ="Server=localhost;Database=mysqlpoc;User=root;Password=Hello",
            ResolutionPath = """C:\gh\dotnetlinux\lib""",
            IndividualsAmount=1000,
            UseOptionTypes = true
            >

[<EntryPoint>]
let main _argv =
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