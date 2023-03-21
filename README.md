# Game
This is a simple application which demonstrates the implementation microservices architecture.
I have implemented two web api for general game application. 
1. Catalog service that provide details of different stuff that players can buy while playing.
2. Inventory service that provides current inventory information of a player.

## Microservices
Microservices is an architectural approach for building applications where we create and deploy service/api for each core  independently. Microservice architecture is distributed and loosely coupled, so one component’s failure will not break the whole app. Independent components work together and communicate with well-defined API contracts. This helps meet rapidly changing business needs and build new functionalities faster.

### Advantages of Microservices
1. Microservices are self-contained and independent deployment components.
2. Less cost of scaling. Any specific service can be scaled independently as and when required.
3. Independently managed services. We can enable/disable services or add more services are required.
4. Microservices follow single responsibility principle.
5. Faster release cycle. Less time required for testing as need to test only the functionality where there are changes instead of complete application as in case of monolithic architecture.
6. Improved Fault Tolerance as the services are independent of each other.

### Disadvantages of Microservices
1. Higher Complexity in tracking errors
2. Increased Network Traffic as the services depend on network on communication with each other.
3. Limited Reuse of Code

## Project Structure:
1. Game.Catalog - Web API
2. Game.Common - Class Library used as Nuget.
3. Game.Infra - For maintaining docker dependencies.
4. Game.Inventory - Web API. 

## Pre requisites
I have implemented this solution on a system having Microsoft OS. Below are software/application used to develop the application
1. Visual Studio Code
2. Docker Desktop

## Technical Stack
1. Microservices architecture
2. Repository pattern
3. Web API
4. Generics
6. .Net 7
7. C#
8. MongoDB





To add dotnet certificate to trusted execute below command
```powershell
dotnet dev-certs https --trust
```
**For Running Profile**
reference - https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-7.0#development-and-launchsettingsjson
which ever profile you want to be default should be added first under "profiles". 
It will be picked up when no profile is mentioned explicitly for run command as below
```powershell
dotnet run --launch-profile "SampleApp"
```

**Docker**

Using MongoDB Docker Image from Docker Hub. as it as pre requisite.

Using Docker engine to download and run the image on the local system

Once the docker is executed using run command it will create a container. This will act as fully running MongoDb server for the service.

Database will be out side of container. As we do not want to have database deleted if the container is destroyed.


## To create and run docker container
```powershell
docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo
```

+ **docker run**- to run the docker
+ **-d** - so that we are not attached to docker process all the time
+ **--rm** to delete container if we stop it
+ **--name <any name>** to assign name of the container
+ **-p <external port>:<internal port>** external port to reach the container mapped to MongoDB port within the container. default 27017 port number used by mongo db. external port can be any port number.
+ **-v mongodbdata:/data/db** volume where mongo db will store data. so when ever mongo db tries to store data files in location /data/db it will be stored in location mongodbdata out side the container
+ **mongo** - name of the docker e-machine we want to run

to confirm docker image is up and running by executing below command in terminal.
```powershell
docker ps
```

Dependency injection configured
Configuration system to read from app setting enabled

```powershell
dotnet new classlib -n Game.Common
```
#common class library project with common code that could be shared across microservices

## Packages added
```powershell
dotnet add package MongoDB.driver
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Binder
dotnet add package Microsoft.Extensions.DependencyInjection
```
to create nuget package of library project
```powershell
dotnet pack -o ..\..\..\packages\
```

-o ..\..\..\packages\ => output placed in specified.


added a nuget config file project specific to add nuget source for projects under it.
```powershell
dotnet new nugetconfig
```
or
can add source by executing command
```powershell
dotnet nuget add source <folder path> -n <name>
```
	
add ne custom nuget package using below command
```powershell
dotnet add package Game.Common 
```
there will be multiple dependency that we will need to run on docker 
so created GAME.INFRA project to have docker compose
docker compose will have all the dependency listed and can be started with single command
```powershell
docker-compose up
```
without comments/output
```powershell
docker-compose up -d
```

dotnet new webapi -n Game.Inventory.Service

## timeouts and retries with exponential backoff.
Dependent service might some time take time to respond. which will result in delay to client. But if this delay exceeds might result in bad user experience.
So I have implemented policy for timeout and retries using polly nuget.
polly nuget helps to handle transient failures in .net application
```powershell
dotnet add package Microsoft.Extensions.Http.Polly
```

## Implemented Circuit breaker pattern to avoid resource exhaustion.
Some time there might be network outage which may cause long down time. During which if there are continue requests attempts done this may make requester as well as responder service unavailable for further requests.