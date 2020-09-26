@echo off
COPY docker-compose_linux.yml docker-compose.yml
COPY docker-compose_linux.override.yml docker-compose.override.yml
COPY Dockerfile_Linux ..\Dockerfile

CD ..

rem CALL Docker\BuildFrontend.bat Coderr.Server.Web\

docker-compose build

ECHO built successfully

docker push coderrio/coderr-communityserver

ECHO successfully pushed to dockerhub

DEL Dockerfile
CD Docker
DEL docker-compose.yml
DEL docker-compose.override.yml

ECHO Completed!
