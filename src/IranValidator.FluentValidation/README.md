# IranValidator.FluentValidation

[FluentValidation](https://docs.fluentvalidation.net/) rules for [IranValidator](https://github.com/NimaHp/IranValidator).

## Installation

```
dotnet add package IranValidator.FluentValidation
```

## Usage

```csharp
using IranValidator.FluentValidation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.NationalCode).IranNationalCode();
        RuleFor(x => x.Mobile).IranMobile();
        RuleFor(x => x.Iban).IranIban();
    }
}
```

Validation rules: IranNationalCode(), IranCardNumber(), IranCompanyId(), IranEconomicCode(), IranMobile(), IranPassport(), IranPostalCode(), IranTelephone(), IranVehiclePlate(), and IranIban().

## License

MIT License — see the repository [LICENSE](https://github.com/NimaHp/IranValidator/blob/main/LICENSE).
