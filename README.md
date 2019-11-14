# CentOsTest

This repo contains a sample app which connect on a local mysql to make basic select and insert operations exposed as a simple http api.

The setup part contains the demo env with centos 6 + mysql. Please do not do that in prod, this config is only for dev.

## Setup
    docker run -it --name mysqlpoc --hostname mysqlpoc -p 3306:3306 centos:6 
  
    # Install mysql-server
    yum update
    yum install mysql-server
    /sbin/chkconfig --levels 235 mysqld on
    # Yes to all expect for root remoting and use Hello as password, only for tests!
    mysql_secure_installation
  
    # Create the Database
    mysql -uroot -pHello
    create database mysqlpoc;
    create table person (id int not null auto_increment primary key, name text);
    insert into person (name) values ('Hello');
    select * from person;
	
	# Install Dependencies for SQLProvider (lib folder)
    setup.cmd

## VsCode FSharp configuration
vscode with this configuration for dotnet core
    {
      "FSharp.msbuildHost": ".net core",
      "FSharp.useSdkScripts": true
    }

## Features
 - mysql client with fsharp type provider : [SQLProvider](https://github.com/fsprojects/SQLProvider/tree/master/tests/SqlProvider.Core.Tests/MySql)
 - webserver : [Giraffe](https://github.com/giraffe-fsharp/Giraffe)
	
## Build
    build.cmd
  
## Deploy
    docker cp bin\Release\netcoreapp3.0\rhel.6-x64\publish mysqlpoc:/home/dotnetlinux
  
## Run
    docker exec -it mysqlpoc bash
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true /home/centostest/dotnetlinux
  
## Known issues
- Fix globalization (this is why I use DOTNET_SYSTEM_GLOBALIZATION_INVARIANT env var)
- Fix the local build which failed in VS but works in the build script.
- Fix the PublishTrimmed which run for a very long time consuming 25% of CPU. 
