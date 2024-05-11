# ConsoleWithoutDependency
dotnet new console -n "ConsoleWithoutDependency"
cd ConsoleWithoutDependency
dotnet build

# ConsoleWithLibraryWithEFCore
dotnet new console -n ConsoleWithLibraryWithEFCore
dotnet new classlib -n LibraryWithEFCore
dotnet add ConsoleWithLibraryWithEFCore reference LibraryWithEFCore
dotnet add LibraryWithEFCore package Microsoft.EntityFrameworkCore --version 8.0.4
dotnet build ConsoleWithLibraryWithEFCore
