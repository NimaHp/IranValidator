using FluentAssertions;
using IranValidator.MinimalApis;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IranValidator.Tests.MinimalApis;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddIranValidator_RegistersIranValidatorService()
    {
        var services = new ServiceCollection();
        services.AddIranValidator();

        var provider = services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<IranValidatorService>();

        resolved.Should().NotBeNull();
        resolved.ValidateMobile("09121234567").Success.Should().BeTrue();
    }

    [Fact]
    public void AddIranValidator_ReturnsSameInstance_ForMultipleResolutions()
    {
        var services = new ServiceCollection();
        services.AddIranValidator();

        var provider = services.BuildServiceProvider();
        var instance1 = provider.GetRequiredService<IranValidatorService>();
        var instance2 = provider.GetRequiredService<IranValidatorService>();

        instance1.Should().BeSameAs(instance2);
    }

    [Fact]
    public void AddIranValidator_ThrowsOnNull()
    {
        var act = () => ((IServiceCollection)null!).AddIranValidator();
        act.Should().Throw<ArgumentNullException>();
    }
}
