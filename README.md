## How to build

### From Visual Studio Code:

>1) Open project in Visual Studio Code (`SLang-Metrics` folder)
>2) Open debug panel on left side of IDE (or press Ctrl + Shift + D)
>3) Launch project with "green arrow" button (also you can select configuration near it)

Use "Build & Launch .NET Core" configuration to start project from scratch
Use "Launch .NET Core" to launch program that was already built
Use "Test .NET Core" to launch tests

### From Visual Studio: (not recommended)

>1) Open solution in Visual Studio using `SLang-Metrics.csproj`
>2) Start project with "Slang-Matrics"

## How to run tests
Use "Test" configuration with launch or change variable
`isTestingMode = true;` in file Testing.cs and recompile project.

Tests raise exception window if some test fails and show error message or writes to console "All tests SUCCED" if all tests passed.

## Reference
- [Metrics description](https://hackmd.io/s/rkw2nF0YQ)
