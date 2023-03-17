# Game
 MicroServices sample application

Game.Catlog.Service - WebAPI



To add dotnet certificate to trusted execute below command
dotnet dev-certs https --trust

******For Running Profile******
reference - https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-7.0#development-and-launchsettingsjson
which ever profile you want to be default should be added first under "profiles". 
It will be picked up when no profile is mentioned explicitly for run command as below
dotnet run --launch-profile "SampleApp"


********Docker********

Using MongoDB Docker Image from Docker Hub. as it as pre requsite.

Using Docker engine to download and run the image on the local system

Once the docker is excuted using run command it will create a container. This will act as fully running MongoDb server for the service.

Database will be out side of container. As we do not want to have database deleted if the container is destroyed.


To create and run docker container
docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo

docker run - to run the docker
-d so that we are not attached to docker process all the time
--rm to delete container if we stop it
--name <any name> to assign name of the container
-p <external port>:<internal port> external port to reach the container mapped to MongoDB port within the container. default 27017 port number used by mongo db. external port can be any port number.

-v mongodbdata:/data/db volume where mongo db will store data. so when ever mongo db tries to store data files in location /data/db it will be stored in location mongodbdata out side the container

mongo - name of the docker e-machine we want to run

to confirm docker image is up and running by excuting "docker ps" command in terminal.


Dependency injection configued
Configuration system to read from app setting enabled