using FluentAssertions;
using IranValidator.MinimalApis;
using Xunit;

namespace IranValidator.Tests.MinimalApis;

public class IranValidatorServiceTests
{
    private readonly IranValidatorService _service = new();

    [Theory]
    [InlineData("0010350829")]
    [InlineData("9876543210")]
    [InlineData("2468013573")]
    public void ValidateNationalCode_Valid_ReturnsSuccess(string code)
    {
        var result = _service.ValidateNationalCode(code);
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("09121234567")]
    [InlineData("09991234567")]
    [InlineData("09351234567")]
    public void ValidateMobile_Valid_ReturnsSuccess(string mobile)
    {
        var result = _service.ValidateMobile(mobile);
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("1234567890")]
    [InlineData("9876543210")]
    public void ValidatePostalCode_Valid_ReturnsSuccess(string postalCode)
    {
        var result = _service.ValidatePostalCode(postalCode);
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("02122345678")]
    [InlineData("03132445678")]
    public void ValidateTelephone_Valid_ReturnsSuccess(string telephone)
    {
        var result = _service.ValidateTelephone(telephone);
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("6037991234567893")]   // Bank Melli Iran
    [InlineData("6104331234567890")]   // Bank Mellat
    public void ValidateCardNumber_Valid_ReturnsSuccess(string card)
    {
        var result = _service.ValidateCardNumber(card);
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("10380284795")]
    [InlineData("14005124960")]
    public void ValidateCompanyId_Valid_ReturnsSuccess(string companyId)
    {
        var result = _service.ValidateCompanyId(companyId);
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("123456789019")]
    [InlineData("987654321057")]
    public void ValidateEconomicCode_Valid_ReturnsSuccess(string code)
    {
        var result = _service.ValidateEconomicCode(code);
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("P12345678")]
    [InlineData("A12345678")]
    public void ValidatePassport_Valid_ReturnsSuccess(string passport)
    {
        var result = _service.ValidatePassport(passport);
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("12ب34567")]
    [InlineData("11ج48514")]
    public void ValidateVehiclePlate_Valid_ReturnsSuccess(string plate)
    {
        var result = _service.ValidateVehiclePlate(plate);
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("IR820540102680020817909002")]
    [InlineData("IR910121234567890123456789")]
    public void ValidateIban_Valid_ReturnsSuccess(string iban)
    {
        var result = _service.ValidateIban(iban);
        result.Success.Should().BeTrue();
    }
}
