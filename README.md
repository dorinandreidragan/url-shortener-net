# Url Shortener with .NET

## Setup

```bash
dotnet new sln --name UrlShortener
dotnet new editorconfig
dotnet new web --name UrlShortener.WebApi
dotnet sln add UrlShortener.WebApi

dotnet dev-certs https --trust
```

Create API testing UI with Swagger

```bash
cd UrlShortener.WebApi
dotnet add UrlShortener.WebApi.csproj package NSwag.AspNetCore
```

Configure Swagger middleware

Add these lines of code `Program.cs` right before and after the initialization `var app = builder.Build()`:

```csharp
// new code
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "UrlShortener";
    config.Title = "Url Shortener";
    config.Version = "v1";
});

// existing code
var app = builder.Build()

// new code
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "UrlShortener";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}
```
