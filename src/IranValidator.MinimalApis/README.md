# IranValidator.MinimalApis

Minimal APIs service integration for [IranValidator](https://github.com/NimaHp/IranValidator).

## Installation

```
dotnet add package IranValidator.MinimalApis
```

## Usage

```csharp
builder.Services.AddIranValidator(); // Registers IranValidatorService as a singleton

app.MapPost("/api/users", (UserModel model, IranValidatorService validator) =>
{
    var result = validator.ValidateNationalCode(model.NationalCode);
    return result.Success ? Results.Ok() : Results.BadRequest();
});
```

## License

MIT License — see the repository [LICENSE](https://github.com/NimaHp/IranValidator/blob/main/LICENSE).
