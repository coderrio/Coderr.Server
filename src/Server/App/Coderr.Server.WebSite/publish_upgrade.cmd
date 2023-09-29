@echo off
call publish_clean.cmd
del /q bin\Release\netcoreapp3.1\publish\log4net*.config
del /q bin\Release\netcoreapp3.1\publish\appsettings*.json
del /q bin\Release\netcoreapp3.1\publish\web.config
