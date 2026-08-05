using System.Globalization;
using FluentValidation;
using IranValidator.AspNetCore;
using IranValidator.Core.Extensions;
using IranValidator.Core.Results;
using IranValidator.DataAnnotations;
using IranValidator.FluentValidation;
using IranValidator.Localization;
using IranValidator.MinimalApis;
using Microsoft.Extensions.DependencyInjection;

// 1. Core: extension method + structured result on the net8.0 build.
if (!"0010350829".IsIranNationalCode())
    throw new InvalidOperationException("Core: IsIranNationalCode(valid) returned false.");
if ("0000000000".IsIranNationalCode())
    throw new InvalidOperationException("Core: IsIranNationalCode(invalid) returned true.");
var result = "0010350829".ValidateIranNationalCode();
if (!result.Success || result.ErrorCode != ValidationErrorCode.None)
    throw new InvalidOperationException("Core: ValidateIranNationalCode failed.");

// 2. DataAnnotations attribute.
if (!new NationalCodeAttribute().IsValid("0010350829"))
    throw new InvalidOperationException("DataAnnotations: attribute rejected a valid code.");

// 3. FluentValidation integration.
var validation = new SampleValidator().Validate(new Sample("0010350829"));
if (!validation.IsValid)
    throw new InvalidOperationException("FluentValidation: integration failed.");

// 4. Localization resolvers.
var fa = new PersianMessageResolver().GetMessage(
    ValidationErrorCode.InvalidChecksum, "nationalCode", CultureInfo.GetCultureInfo("fa-IR"));
var en = new EnglishMessageResolver().GetMessage(
    ValidationErrorCode.InvalidChecksum, "nationalCode", CultureInfo.InvariantCulture);
if (string.IsNullOrWhiteSpace(fa) || string.IsNullOrWhiteSpace(en))
    throw new InvalidOperationException("Localization: a resolver returned an empty message.");

// 5. Minimal APIs + ASP.NET Core DI wiring composed on net8.0.
var services = new ServiceCollection();
services.AddIranValidator().AddIranValidation().AddIranLocalization();
using var provider = services.BuildServiceProvider();
if (!provider.GetRequiredService<IranValidatorService>().ValidateNationalCode("0010350829").Success)
    throw new InvalidOperationException("MinimalApis: DI-resolved service rejected a valid code.");

// 6. ASP.NET Core types constructible on the net8.0 shared framework.
_ = new IranValidateAttribute();
_ = IranAspNetCoreLocalization.CreateDefaultResolver();

Console.WriteLine("IranValidator net8.0 smoke: OK");

internal sealed record Sample(string? Code);

internal sealed class SampleValidator : AbstractValidator<Sample>
{
    public SampleValidator() => RuleFor(x => x.Code).IranNationalCode();
}
