# IranValidator.AspNetCore

ASP.NET Core integration for [IranValidator](https://github.com/NimaHp/IranValidator) — dependency injection, action filters, and middleware.

## Installation

```
dotnet add package IranValidator.AspNetCore
```

## Usage

```csharp
// Program.cs
builder.Services.AddControllers();
builder.Services.AddIranValidation();
var app = builder.Build();
app.UseIranValidation();
```

```csharp
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [HttpPost]
    [IranValidate]
    public IActionResult Create(UserModel model) => Ok();
}
```

## License

MIT License — see the repository [LICENSE](https://github.com/NimaHp/IranValidator/blob/main/LICENSE).
