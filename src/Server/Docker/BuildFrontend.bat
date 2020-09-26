@echo off
IF "%1"=="" (SET NewDir=..\Coderr.Server.Web) ELSE (SET NewDir="%1")
echo %NewDir%
set OLDDIR=%CD%
CD %NewDir%

call npm i
call node node_modules/webpack/bin/webpack.js --config webpack.config.prod.js --mode=production

chdir /d %OLDDIR%
