# C# SPECFLOW 3 DEMO PROJECT USING ALLURE

## INSTALLATION
- Install Microsoft Visual Studio 2019: https://visualstudio.microsoft.com/vs/

- Add `msbuild.exe` path to `PATH` enviroment variable (Path according your local installation).
Example:
```sh
C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin
```

- Install dependencies executing next command:
```sh
./nuget.exe restore AllureDockerCSharpSpecFlow3Example.sln -Verbosity Detailed -NonInteractive
 ```

- Build project executing next command:
```sh
msbuild.exe AllureDockerCSharpSpecFlow3Example.sln
 ```

## USAGE
Execute Allure Docker Service from this directory
```sh
docker-compose up -d allure allure-ui
```

- Verify if Allure API is working. Go to -> http://localhost:5050/allure-docker-service/latest-report

- Verify if Allure UI is working. Go to -> http://localhost:5252/allure-docker-service-ui/

Note:
- In the example we are using this directory `${PWD}/AllureDockerCSharpSpecFlow3Example/bin/Debug/allure-results`. If you clean your build the `allure-results` directory will be removed, then docker will lose the reference of that volume. If you want to avoid that problem move the `allure-results` directory in another directory.


Each time you run tests, the Allure report will be updated.

Execute tests:
```sh
./AllureDockerCSharpSpecFlow3Example/bin/Debug/SpecFlowPlusRunner/net461/SpecRun.exe run default.srprofile --baseFolder ./AllureDockerCSharpSpecFlow3Example/bin/Debug --filter "@allure"
 ```

 Note:
 - If this error appears:
 https://github.com/SpecFlowOSS/SpecFlow/issues/2126
```sh
System.IO.FileLoadException : Could not load file or assembly 'System.Threading.Tasks.Extensions
```
Workaround: After building (before running tests), copy the `System.Threading.Tasks.Extensions.dll` to the `SpecFlowPlusRunner` directory
 ```sh
 Copy-Item AllureDockerCSharpSpecFlow3Example/bin/Debug/System.Threading.Tasks.Extensions.dll  -Destination  .\AllureDockerCSharpSpecFlow3Example\bin\Debug\SpecFlowPlusRunner\net461\
```

See documentation here:
- https://github.com/fescobar/allure-docker-service
- https://github.com/fescobar/allure-docker-service-ui
