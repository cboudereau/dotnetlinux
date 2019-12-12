# dotnetlinux (a bad named repo...)

This repo contains a sample app which connects on a local mysql to make basic select and insert operations exposed as a simple http api.

The setup part contains the demo env with centos 6 + mysql. Please do not do that in prod, this config is only for dev.

## Features
 - A ready to use dev environment : a windows desktop with a fully compatible linux env based on docker.
 - mysql client with fsharp type provider : [SQLProvider](https://github.com/fsprojects/SQLProvider/tree/master/tests/SqlProvider.Core.Tests/MySql)
 - webserver : [Giraffe](https://github.com/giraffe-fsharp/Giraffe)
 - dotnetcore : an app compatible for windows (locally) and for linux (with dotnet publish)
 
## Setup Docker Desktop
### Normal version

You can install docker desktop for windows here https://www.docker.com/products/docker-desktop.

It works out of the box but this version is using a HyperV VM (MobyLinuxVM) which performance cannot be adjusted dynamically and really slower than the new WSL2 based version.

### Edge version (really fast! But only for testing now)
The edge version of docker desktop increases performance by at least 5x thanks to Microsoft wsl 2 optimisations.

At this time, wsl2 is only available on the slow ring of the windows insider program. You have to subscribe to windows insider then update your windows, activate wsl2, download edge version of Docker desktop then select the wsl 2 option from docker desktop

1. Subcribe to Windows Insider
Open Windows Settings > Update & Security > Windows Insider > Subscribe
When it works, you can select the ring (for wsl2 at least slow is ok) and the restart screen appears. In my case I tried several times in 3 days to get those screen.
You can check the Windows Insider Program in Windows Update & Security.

2. Install wsl2
Becarefull if you previously installed wsl1 or docker desktop. In my case, I used the docker export command to backup one really important container (I am migrating this big container into smaller one). In my case the underlying VM was still there and I did not really lost anything.

I removed wsl1 to have less problems but you can try directly if it works.

Follow the instructions from this page : https://docs.microsoft.com/en-us/windows/wsl/wsl2-install

3. Configure wsl2
    wsl --set-default-version 2

4. Install Docker Desktop Edge
Download the last Docker Desktop Edge which is a technical preview here : https://docs.docker.com/docker-for-windows/edge-release-notes/
When it is installed, you can switch to wsl2 back and forth in the general settings (Enable the experimental WSL 2 based engine)
For further details, follow those instructions : https://docs.docker.com/docker-for-windows/wsl-tech-preview/

5. Configure
When wsl2 is configured through Docker Desktop Edge it is possible to check the runtime 
    wsl -l -v

      NAME                   STATE           VERSION
    * Ubuntu                 Running         2
      docker-desktop-data    Running         2
      docker-desktop         Running         2

6. Fix for older system
Create a .wslconfig in order to run older os like centos 6

   %userprofile%\.wslconfig

   [wsl2]
   kernelCommandLine = vsyscall=emulate

In my case, I have the Ubuntu one and the 2 dockers
When you restart Windows Terminal, you can run ubuntu commands directly!

### (Optional) Install docker cli on Ubuntu WSL 2
If already have configured an ubuntu distrib running on wsl 2.
In the docker desktop settings > Resources > WSL Integration, activate the ubuntu distro to have the docker cli installed

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
    docker-compose down && git clean -fxd

## Run
    run.cmd

## Test
1. Insert into mysql "clem"
    iwr -Method Post -Body '{"name":"hel"}' -Uri http://localhost:5000/person
2. List the person table
    curl http://localhost:5000/persons

## Known issues
- SQLProvider with dotnetcore 3 and MySql : https://github.com/fsprojects/SQLProvider/tree/master/tests/SqlProvider.Core.Tests/MySql
- Fix globalization (this is why I use DOTNET_SYSTEM_GLOBALIZATION_INVARIANT env var)
- Fix the PublishTrimmed which run for a very long time consuming 25% of CPU. 

## TODO
- create a dotnet template new mysql with in parameter the name of the target os
- Use FAKE to save schema config and avoid preparing mysql dependencies on build.