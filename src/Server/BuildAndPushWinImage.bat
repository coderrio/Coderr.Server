COPY docker-compose_windows.yml docker-compose.yml
COPY docker-compose_windows.override.yml docker-compose.override.yml
CD Coderr.Server.Web
COPY Dockerfile_Windows Dockerfile

CD ..

CALL BuildFrontend.bat

docker-compose build

ECHO built successfully

docker push coderrio/win_coderrserverweb

ECHO successfully pushed to dockerhub

CD Coderr.Server.Web
DEL Dockerfile
CD ..
DEL docker-compose.yml
DEL docker-compose.override.yml

ECHO Completed!
PAUSE
