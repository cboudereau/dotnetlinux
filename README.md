# dotnetlinux (a bad named repo...)

This repo contains a sample app which connects on a local mysql to make basic select and insert operations exposed as a simple http api.

The setup part contains the demo env with centos 6 + mysql. Please do not do that in prod, this config is only for dev.
	
## Features
 - mysql client with fsharp type provider : [SQLProvider](https://github.com/fsprojects/SQLProvider/tree/master/tests/SqlProvider.Core.Tests/MySql)
 - webserver : [Giraffe](https://github.com/giraffe-fsharp/Giraffe)
	
## Setup local dev environment (Install the localdb mysql container and dependencies for SQLProvider (lib folder))
    setup.cmd

## VsCode (settings.json)
### Extensions
 - Install Ionide extension 
### FSharp configuration for dotnet core
    {
      "FSharp.msbuildHost": ".net core",
      "FSharp.useSdkScripts": true
    }

## Build (the type provider will connect to the localdb container to build)
    build.cmd
  
## Deploy/Run (mount the app folder wich contains the poc and run the db and giraffe service)
    docker-compose up
  
## Clean
    docker-compose down

## Test
1. Insert into mysql "clem"
    iwr -Method Post -Body '{"name":"hel"}' -Uri http://localhost:5000/person
2. List the person table
    curl http://localhost:5000/persons

## Known issues
- Fix globalization (this is why I use DOTNET_SYSTEM_GLOBALIZATION_INVARIANT env var)
- Fix the PublishTrimmed which run for a very long time consuming 25% of CPU. 
- Wait the mysql starting between setup and build (maybe use the docker logs and wait for mysql connected)

## TODO
- Use FAKE to save schema config and avoid preparing mysql dependencies on build.