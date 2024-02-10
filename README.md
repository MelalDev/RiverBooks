## Initialize your projects
create sln
```zsh
dotnet new sln -n RiverBooks
```

create webapi
```zsh
dotnet new webapi -n RiverBooks.Web
```

add RiverBooks.Web to .sln file
```zsh
dotnet sln RiverBooks.sln add ./RiverBooks.Web/RiverBooks.Web.csproj
```

create class library for Books module
```zsh
dotnet new classlib -n RiverBooks.Books -o ./RiverBooks.Books
```

add Books module to .sln file
```zsh
dotnet sln RiverBooks.sln add ./RiverBooks.Books/RiverBooks.Books.csproj
```

add package
```zsh
dotnet add package "[package name]"
```

## Db Migrations
Add nuget package to Web project
```zsh
cd RiverBooks.Web
dotnet add package Microsoft.EntityFrameworkCore.Design
```

Install dotnet migration tool
```zsh
dotnet tool install --global dotnet-ef

dotnet tool update --global dotnet-ef
```

Add migration
```zsh
cd RiverBooks.Web
dotnet ef migrations add Initial -c BookDbContext -p ../RiverBooks.Books/RiverBooks.Books.csproj -s ./RiverBooks.Web.csproj -o Data/Migrations
```
-c BookDbContext: specify where to find DbContext and what DbContext to use. Assuming you only have one DbContext
named BookDbContext, this is sufficient.

-p ../RiverBooks.Books/RiverBooks.Books.csproj: Then you have to specify the project where it's found

-s ./RiverBooks.Web.csproj: Then we need to specify the startup project, which is where we are.

-o Data/Migrations: And then we finally, you're going to want to say where should those migrations be output within the 
books module. And I'm going to put them inside of a migrations folder underneath a data folder, and it's going to be 
ahead and create those for us.

Apply migration to Database
```zsh
dotnet ef database update

For Testing, for more detail: https://learn.microsoft.com/en-us/ef/core/cli/dotnet#aspnet-core-environment
dotnet ef database update -- --environment Testing
```

## Add reference project
```zsh
dotnet add reference [your path project]
```