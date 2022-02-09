@echo off
call publish_clean.cmd
del /q bin\Release\PublishOutput\log4net*.config
del /q bin\Release\PublishOutput\appsettings*.json
del /q bin\Release\PublishOutput\web.config
