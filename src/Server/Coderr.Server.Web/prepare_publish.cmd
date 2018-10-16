IF EXIST "bin\Release\PublishOutput\appsettings.Development.json" (
  del /q bin\Release\PublishOutput\appsettings.json
  del /q bin\Release\PublishOutput\appsettings.Development.json
  move bin\Release\PublishOutput\appsettings.Publish.json bin\Release\PublishOutput\appsettings.json
)

del /q bin\Release\PublishOutput\log4net.production.config
del /q bin\Release\PublishOutput\log4net.test.config

rmdir /q /s bin\Release\PublishOutput\de
rmdir /q /s bin\Release\PublishOutput\es
rmdir /q /s bin\Release\PublishOutput\fr
rmdir /q /s bin\Release\PublishOutput\it
rmdir /q /s bin\Release\PublishOutput\ja
rmdir /q /s bin\Release\PublishOutput\ko
rmdir /q /s bin\Release\PublishOutput\ru
rmdir /q /s bin\Release\PublishOutput\zh-Hans
rmdir /q /s bin\Release\PublishOutput\zh-Hant
