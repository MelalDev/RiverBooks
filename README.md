## Run Docker compose
```zsh
docker compose up -d
```

## Run project
```zsh
cd src/RiverBooks.Web
dotnet run --launch-profile https
```

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
dotnet sln RiverBooks.sln add ./src/RiverBooks.Web/RiverBooks.Web.csproj
```

create class library for Books module
```zsh
dotnet new classlib -n RiverBooks.Books -o ./RiverBooks.Books
```

add Books module to .sln file
```zsh
dotnet sln RiverBooks.sln add ./src/RiverBooks.Books/RiverBooks.Books.csproj
```

add nuget package
```zsh
dotnet add package "[package name]"
```

## Instal Migration tools
Add nuget package to Web project
```zsh
cd src/RiverBooks.Web
dotnet add package Microsoft.EntityFrameworkCore.Design
```

Install dotnet migration tool
```zsh
dotnet tool install --global dotnet-ef

dotnet tool update --global dotnet-ef
```

## Db Migrations Books
Add migration
```zsh
cd src/RiverBooks.Web
dotnet ef migrations add Initial -c BookDbContext -p ../BooksModule/RiverBooks.Books/RiverBooks.Books.csproj -s ./RiverBooks.Web.csproj -o Data/Migrations
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
cd src/RiverBooks.Web
dotnet ef database update -c BookDbContext

For Testing, for more detail: https://learn.microsoft.com/en-us/ef/core/cli/dotnet#aspnet-core-environment
dotnet ef database update -- --environment Testing
```

## Db Migrations Users
Add migration
```zsh
cd src/RiverBooks.Web
dotnet ef migrations add InitialUsers -c UsersDbContext -p ../UsersModule/RiverBooks.Users/RiverBooks.Users.csproj -s ./RiverBooks.Web.csproj -o Data/Migrations
```

Apply migration to Database
```zsh
cd src/RiverBooks.Web
dotnet ef database update -c UsersDbContext

For Testing, for more detail: https://learn.microsoft.com/en-us/ef/core/cli/dotnet#aspnet-core-environment
dotnet ef database update -- --environment Testing
```

## Db Migrations OrderProcessing
Add migration
```zsh
cd src/RiverBooks.Web
dotnet ef migrations add Initial_OrderProcessing -c OrderProcessingDbContext -p ../OrderProcessingModule/RiverBooks.OrderProcessing/RiverBooks.OrderProcessing.csproj -s ./RiverBooks.Web.csproj -o Data/Migrations
```

Apply migration to Database
```zsh
cd src/RiverBooks.Web
dotnet ef database update -c OrderProcessingDbContext
```

## Add reference project
```zsh
dotnet add reference [your path project]
```

## Error: The SSL connection could not be established
```zsh
dotnet dev-certs https --trust
```