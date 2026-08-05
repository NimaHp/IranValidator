using FluentAssertions;
using IranValidator.Core.Exceptions;
using Xunit;

namespace IranValidator.Tests.Core;

public class InvalidValidatorConfigurationExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        var ex = new InvalidValidatorConfigurationException("test error");
        ex.Message.Should().Be("test error");
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsBoth()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new InvalidValidatorConfigurationException("outer", inner);
        ex.Message.Should().Be("outer");
        ex.InnerException.Should().BeSameAs(inner);
    }

    [Fact]
    public void Constructor_ThrownAndCaught_WorksCorrectly()
    {
        Action act = () => throw new InvalidValidatorConfigurationException("config error");
        act.Should().Throw<InvalidValidatorConfigurationException>()
            .WithMessage("config error");
    }

    [Fact]
    public void Constructor_WithInnerException_ChainWorks()
    {
        Action act = () =>
        {
            try
            {
                throw new ArgumentException("root cause");
            }
            catch (Exception ex)
            {
                throw new InvalidValidatorConfigurationException("wrapper", ex);
            }
        };
        act.Should().Throw<InvalidValidatorConfigurationException>()
            .WithMessage("wrapper")
            .Which.InnerException.Should().BeOfType<ArgumentException>();
    }
}
