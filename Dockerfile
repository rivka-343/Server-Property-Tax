# השתמש בבנייה מרובת שלבים

# ----------------------------------------------------------------------------------------------------
# שלב 1: בנייה (SDK)
# ----------------------------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# העתקת קבצי הפרויקט
COPY PropertyTax.Core/PropertyTax.Core.csproj ./PropertyTax.Core/
COPY PropertyTax.Data/PropertyTax.Data.csproj ./PropertyTax.Data/
COPY PropertyTax.Service/PropertyTax.Service.csproj ./PropertyTax.Service/
COPY PropertyTax/PropertyTax.csproj ./PropertyTax/

# שחזור תלויות (NuGet) עבור כל פרויקט בנפרד
RUN dotnet restore ./PropertyTax.Core/PropertyTax.Core.csproj
RUN dotnet restore ./PropertyTax.Data/PropertyTax.Data.csproj
RUN dotnet restore ./PropertyTax.Service/PropertyTax.Service.csproj
RUN dotnet restore ./PropertyTax/PropertyTax.csproj

# העתקת כל קוד הפרויקט
COPY PropertyTax.Core ./PropertyTax.Core/
COPY PropertyTax.Data ./PropertyTax.Data/
COPY PropertyTax.Service ./PropertyTax.Service/
COPY PropertyTax ./PropertyTax/

# פרסום האפליקציה - מציין את הפרויקט במפורש
RUN dotnet publish ./PropertyTax/PropertyTax.csproj -c Release -o /app/out

# ----------------------------------------------------------------------------------------------------
# שלב 2: הפעלה (Runtime)
# ----------------------------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "PropertyTax/PropertyTax.dll"]
