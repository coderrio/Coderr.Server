IF EXIST "bin\Release\netcoreapp3.1\publish\appsettings.Development.json" (
  del /q bin\Release\netcoreapp3.1\publish\appsettings.json
  del /q bin\Release\netcoreapp3.1\publish\appsettings.Development.json
  del /q bin\Release\netcoreapp3.1\publish\appsettings.Test.json
  del /q bin\Release\netcoreapp3.1\publish\appsettings.OnPremiseDev.json
  del /q bin\Release\netcoreapp3.1\publish\appsettings.Production.json
  move bin\Release\netcoreapp3.1\publish\appsettings.Publish.json bin\Release\netcoreapp3.1\publish\appsettings.json
)

del /q bin\Release\netcoreapp3.1\publish\log4net.production.config
del /q bin\Release\netcoreapp3.1\publish\log4net.test.config
del /q bin\Release\netcoreapp3.1\publish\*.pdb

rmdir /q /s bin\Release\netcoreapp3.1\publish\de
rmdir /q /s bin\Release\netcoreapp3.1\publish\es
rmdir /q /s bin\Release\netcoreapp3.1\publish\fr
rmdir /q /s bin\Release\netcoreapp3.1\publish\it
rmdir /q /s bin\Release\netcoreapp3.1\publish\ja
rmdir /q /s bin\Release\netcoreapp3.1\publish\ko
rmdir /q /s bin\Release\netcoreapp3.1\publish\ru
rmdir /q /s bin\Release\netcoreapp3.1\publish\zh-Hans
rmdir /q /s bin\Release\netcoreapp3.1\publish\zh-Hant
rmdir /q /s bin\Release\netcoreapp3.1\publish\cs
rmdir /q /s bin\Release\netcoreapp3.1\publish\pl
rmdir /q /s bin\Release\netcoreapp3.1\publish\pt-BR
rmdir /q /s bin\Release\netcoreapp3.1\publish\tr
