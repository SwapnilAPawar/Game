# Game<!-- omit from toc -->
This is a simple application which demonstrates the implementation microservices architecture.
I have implemented two web api as mentioned below for general game application. I have established asynchronous communication between this services using RabbitMQ and MassTransit.

**Web Api/Service**
1. Catalog service that provide details of different stuff that players can buy while playing.
2. Inventory service that provides current inventory information of a player.

## Table of Contents <!-- omit from toc -->
- [Concepts](#concepts)
  - [Microservices](#microservices)
    - [Advantages of Microservices](#advantages-of-microservices)
    - [Disadvantages of Microservices](#disadvantages-of-microservices)
  - [Synchronous Communication](#synchronous-communication)
  - [Asynchronous communication](#asynchronous-communication)
    - [Microservices autonomy](#microservices-autonomy)
      - [Advantages of Asynchronous communication](#advantages-of-asynchronous-communication)
      - [Dependency](#dependency)
    - [Asynchronous propagation of data](#asynchronous-propagation-of-data)
  - [RabbitMQ and MassTransit](#rabbitmq-and-masstransit)
    - [Advantages of RabbitMQ](#advantages-of-rabbitmq)
  - [MassTransit](#masstransit)
  - [Docker Compose](#docker-compose)
- [Solution Structure:](#solution-structure)
  - [Game.Catalog.Contracts](#gamecatalogcontracts)
  - [Game.Catalog.Service](#gamecatalogservice)
  - [Game.Common](#gamecommon)
  - [Game.Infra](#gameinfra)
  - [Game.Inventory.Service](#gameinventoryservice)
- [Additional Configuration](#additional-configuration)
  - [Trusting Dotnet certificate](#trusting-dotnet-certificate)
  - [For Running Profile](#for-running-profile)
  - [Project Specific Nuget Source](#project-specific-nuget-source)
  - [Configuration](#configuration)
    - [Dependency Injection](#dependency-injection)
    - [Timeouts and Retries with exponential back off.](#timeouts-and-retries-with-exponential-back-off)
    - [Circuit Breaker Pattern](#circuit-breaker-pattern)

## Pre requisites
I have implemented this solution on a system having Microsoft OS. Below are software/application used to develop the application
1. <a href="https://code.visualstudio.com/download" target="_blank">Visual Studio Code</a>
2. <a href="https://www.docker.com/products/docker-desktop/" target="_blank">Docker Desktop</a>

## Technical Stack
1. Web API
2. Generics
3. .Net 7
4. C#
5. MongoDB
6. RabbitMQ
7. MassTransit

# Concepts
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

## Synchronous Communication
When we have direct communication between different services is considered to be synchronous communication. As failure of one service may result in failure of dependent service and might eventually make most of the system unavailable. This will result in
1. Increased Latency
2. Partial Failure Amplification
3. Reduced SLA

## Asynchronous communication
- Client does not wait for a response in timely manner.
- There might be no response at all.
- Usually involves use of light weight message broker.
- Message broker should have high availability.
- Messages can be sent to message broker and received by
    - Single Receiver(asynchronous command)
    - Multiple Receiver(publish/subscribe events)

One of the Key benefit of using asynchronous communication is it enforces microservices autonomy.

### Microservices autonomy
- Here client talks to a service. 
- Service will immediately acknowledge to the client of successful reception o request.
- Service does not directly talks to it dependent service.
- It will send and asynchronous message to the broker.
- The dependent service will consume the message and will reply back to the same broker.
- Called Service can receive reply messages from broker and perform updates.
- Client can request for update or Called service can revert back to reply url.

#### Advantages of Asynchronous communication
- If one of the service fails it won't have impact on other services.
- Partial Failure won't propagate
- Independent service SLA
- Microservices Autonomy enforced

#### Dependency
- Highly available message broker

### Asynchronous propagation of data
Here the service publishes an event each time a record is created/updated/deleted.
Client service can listen to this event and create collection of records in its own database.
Since the message broker is highly available the client service data should consistent with regards to source service data.
This helps to provide immediate reply to client request with data from its own database.

## RabbitMQ and MassTransit
RabbitMQ
RabbitMQ introduces concept of exchanges similar mailbox.
Service wants to publish messages, it will send it to RabbitMQ exchange
With appropriate bindings in place RabbitMQ exchange will distribute messages to the configured queue
From there RabbitMQ will take care of delivering the messages to any subscribed service.
### Advantages of RabbitMQ
1. Lightweight
2. Supports Advanced Message Queuing Protocol(AMQP)
3. Easy to run locally
4. popular in open source community.

We can directly use RabbitMQ in our by creating exchanges and queues and configuring bindings to ensure messages are properly routed.
This will make our code tied to RabbitMQ. So, later if we decide to use some other message broker it will result in a lot of code changes. To avoid this we will be using MassTransit.

## MassTransit 
MassTransit is a popular open source distributed application framework for .Net.
It can take care of most of the heritage mq configuration and able to integrate to multiple other message brokers. 
It allows our service to connect to higher layer of broker agnostic apis.
It introduces a concept of publisher and consumer.

## Docker Compose
Compose is a tool for defining and running multi-container Docker applications. With Compose, you use a YAML file to configure your application’s services. Then, with a single command, you create and start all the services from your configuration.

Compose works in all environments: production, staging, development, testing, as well as CI workflows. It also has commands for managing the whole lifecycle of your application:

- Start, stop, and rebuild services
- View the status of running services
- Stream the log output of running services
- Run a one-off command on a service

**The key features of Compose that make it effective are:**

- Have multiple isolated environments on a single host
- Preserves volume data when containers are created
- Only recreate containers that have changed
- Supports variables and moving a composition between environments


# Solution Structure:
1. Game.Catalog - 
    - src
        - Game.Catalog.Contracts - Class Library used as Nuget.
        - Game.Catalog.Service - Web API
2. Game.Common - 
    - src
        - Game.Common - Class Library used as Nuget.
3. Game.Infra - For maintaining docker dependencies.
4. Game.Inventory - 
    - src
        - Game.Inventory.Service - Web API. 

## Game.Catalog.Contracts
We need to define contracts for messaging between different services that uses asynchronous communication.
We will create a separate class library for this. So that we can package and share this with other services.
``` powershell
# to create project
dotnet new classlib -n Game.Catalog.Contracts

#to create nuget package of library project
dotnet pack -o <output directory path>

#In my case
dotnet pack -o ..\..\..\packages\

#Also we should specify version number while packaging new changes. This helps our service to know about the new changes.
dotnet pack -p:PackageVersion=1.0.1  -o ..\..\..\packages\
```

## Game.Catalog.Service
This is a web api application. It will help to manage inventory of a user. 
``` powershell
# to create project
dotnet new webapi -n Game.Catalog.Service

#Adding reference in Game.Catalog.Service
dotnet add reference ..\Game.Catalog.Contracts\Game.Catalog.Contracts.csproj

# add custom nuget package using below command
dotnet add package Game.Common
```

## Game.Common
This is a class library project with common code that could be shared across microservices by creating nuget package.

```powershell
# to create project
dotnet new classlib -n Game.Common

#add reference packages
dotnet add package MongoDB.driver
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Binder
dotnet add package Microsoft.Extensions.DependencyInjection

#to create nuget package of library project
dotnet pack -o <output directory path>

#In my case
dotnet pack -o ..\..\..\packages\

#Also we should specify version number while packaging new changes. This helps our service to know about the new changes.
dotnet pack -p:PackageVersion=1.0.1  -o ..\..\..\packages\
```

## Game.Infra
This is folder that contains the docker compose file. This file contains configuration for creating all the containers in single command execution. Here we have following container configured
1. Mongo with following configuration
   - Ports:
     - 5672 port for communication
   - Volume: specifying volume is important or Mongo will save in data in random location. Also it helps to keep data safe even after the container is destroyed.

2. RabbitMQ with following configuration
   - Ports:
     - 5672 port will be used to publish/consume messages into RabbitMQ
     - 15672 used to go to portal
   - Hostname: specifying hostname is important or RabbitMQ will save in location with random name each time we start it
   - Volume: specifying volume is important or RabbitMQ will be saved in random location

    **Note:** We can access RabbitMQ portal http://localhost:15672 using credentials
   - Username: guest
   - Password: guest

```powershell
# to execute docker compose
docker-compose up

# OR to execute without comments/output
docker-compose up -d
```

## Game.Inventory.Service
This is a web api application. It will help to manage inventory of a user. 
``` powershell
# to create project
dotnet new webapi -n Game.Inventory.Service

# add custom nuget package using below command
dotnet add package Game.Common
dotnet add package Game.Catalog.Contracts 
```

# Additional Configuration

## Trusting Dotnet certificate
Since api are running using dotnet certificates while debugging we will see browser warning about heading to untrusted website. So we will always have to ask it to proceed. To avoid this additional checks we can add our dotnet certificate to trusted group. Below command can be used for that
```powershell
dotnet dev-certs https --trust
```
## For Running Profile
When we create the project launchSetting.json will have a few profiles created for us. To make a profile default it should be added first under "profiles". It will be picked up when no profile is mentioned explicitly. To specify specific profile for execution we can use below command
```powershell
dotnet run --launch-profile "<Profile Name>"
```
reference - https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-7.0#development-and-launchsettingsjson

## Project Specific Nuget Source
We can create a Nuget config file that can have our custom Nuget source location. This will help us to add our Nuget to projects just like the regular packages
```powershell
# Creating Nuget Config file
dotnet new nugetconfig

# Adding source
dotnet nuget add source <folder path> -n <name>
```

## Configuration
1. Dependency Injection
2. Timeouts and Retries with exponential back off
3. Circuit Breaker Pattern

### Dependency Injection
Dependency Injection (DI) software design pattern is a technique for achieving Inversion of Control (IoC) between classes and their dependencies. I have made use of .NET's built-in DI, along with configuration, logging, and the options pattern.

### Timeouts and Retries with exponential back off.
Dependent service might some time take time to respond. This will result in delayed response to client. But if this delay exceeds might result in bad user experience. So I have implemented policy for timeout and retries using polly nuget. Polly nuget helps to handle transient failures in .net application
```powershell
dotnet add package Microsoft.Extensions.Http.Polly
```

### Circuit Breaker Pattern
Some time there might be network outage or service issues which may cause long down time. During which if there are continue requests attempts done this may make requester as well as responder service unavailable for further requests.
Circuit breaker will prevent the service from performing requests that are likely to fail.
It will prevent the service from resource exhaustion.
It will help to avoid overwhelming of dependency.

It will open circuit for specified wait time if request failure reaches the threshold count. After waiting it allow one request to go through to check if the dependent service is up and running. if yes, it will close the circuit and allow all requests to go through.