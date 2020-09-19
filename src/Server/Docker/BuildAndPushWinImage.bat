@echo off
SET OLDDIR=%CD%
SET pushMode=%1
IF "%1"=="" (
	SET pushMode="local"
)
IF "%1"=="remote" (
	SET pushMode="coderrio/communityserver-win"
)
	
COPY /y docker-compose_windows.yml ..\docker-compose.yml
COPY /y docker-compose_windows.override.yml ..\docker-compose.override.yml

cd ..

COPY /y Coderr.Server.Web\Dockerfile_Windows Coderr.Server.Web\Dockerfile

rem CALL ..\Docker\BuildFrontend.bat Coderr.Server.Web\

docker-compose build
ECHO Built successfully

docker push %pushMode%
ECHO successfully pushed to dockerhub

echo Cleaning up
DEL Coderr.Server.Web\Dockerfile
DEL docker-compose.yml
DEL docker-compose.override.yml
cd %OLDDIR%
ECHO Completed!

