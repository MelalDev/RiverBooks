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