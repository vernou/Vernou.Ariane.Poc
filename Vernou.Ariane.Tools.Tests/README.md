# SimpleConsoleApp
dotnet new console -n "SimpleConsoleApp"
cd SimpleConsoleApp
dotnet build

# ConsoleWithLibraryWithEFCore
dotnet new console -n ConsoleWithLibraryWithEFCore
dotnet new classlib -n LibraryWithEFCore
dotnet add ConsoleWithLibraryWithEFCore reference LibraryWithEFCore
dotnet add LibraryWithEFCore package Microsoft.EntityFrameworkCore --version 8.0.4
dotnet build ConsoleWithLibraryWithEFCore
