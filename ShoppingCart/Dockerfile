#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ShoppingCart/ShoppingCart.csproj", "ShoppingCart/"]
COPY ["ShoppingCart.Db/ShoppingCart.Db.csproj", "ShoppingCart.Db/"]
COPY ["ShoppingCart.Common/ShoppingCart.Common.csproj", "ShoppingCart.Common/"]
COPY ["ShoppingCart.Services/ShoppingCart.Services.csproj", "ShoppingCart.Services/"]
RUN dotnet restore "ShoppingCart/ShoppingCart.csproj"
COPY . .
WORKDIR "/src/ShoppingCart"
RUN dotnet build "ShoppingCart.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShoppingCart.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShoppingCart.dll"]