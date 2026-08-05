# IranValidator.DataAnnotations

[DataAnnotations](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations) attributes for [IranValidator](https://github.com/NimaHp/IranValidator).

## Installation

```
dotnet add package IranValidator.DataAnnotations
```

## Usage

```csharp
using IranValidator.DataAnnotations;

public class UserModel
{
    [NationalCode]
    public string NationalCode { get; set; } = string.Empty;

    [IranMobile]
    public string Mobile { get; set; } = string.Empty;

    [IranIban]
    public string? Iban { get; set; }
}
```

Available attributes: [NationalCode], [IranCardNumber], [IranCompanyId], [IranEconomicCode], [IranMobile], [IranPassport], [IranPostalCode], [IranTelephone], [IranVehiclePlate], and [IranIban].

## License

MIT License — see the repository [LICENSE](https://github.com/NimaHp/IranValidator/blob/main/LICENSE).
