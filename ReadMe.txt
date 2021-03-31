- Intro

Program built in dotnetcore3.1 and c#, based on Clean Architeture and Domain Driven Design.
Using the CsvHelper library for csv parsing (https://joshclose.github.io/CsvHelper/).
Using XUnit library for unit testing.

- Compile and Run

Requirement: .NET Core 3.1 SDK (https://dotnet.microsoft.com/download)

1-Open the cli, navigate to "src" directory and run "dotnet publish .\vestingprogram\vestingprogram.csproj --output .\publish";
2-Navigate to "publish" and run "vestingprogram" (ex.:vestingprogram example3.csv 2021-01-01 1).

- Compile and Debug

Requirement: Visual Studio 2019 or similar IDE with .NET Core 3.1 SDK installed (like JetBrains Rider).

1-Navigate to src folder and open vesting_program.sln in the IDE;
2-In the Solution Explorer view, right-click on project VestingProgram and select "Set as Startup Project";
3-Right-click again on VestingProgram and select "Properties";
4-On the Debug tab, include the arguments in "Application arguments" (ex.: example3.csv 2021-01-01 1);
5-Press F5 to start debugging.