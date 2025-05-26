using AwesomeAssertions;
using Soenneker.Tests.Unit;
using Xunit;

namespace Soenneker.Extensions.Int.Tests;

public class IntExtensionTests : UnitTest
{
    [Fact]
    public void Default()
    {
    }

    [Fact]
    public void ToGuidString_ValidInteger_ProducesValidGuidFormat()
    {
        // Arrange
        const int value = 123456789;

        // Act
        string guidString = value.ToGuidString();

        // Assert
        guidString.Should().MatchRegex(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$");
    }

    [Fact]
    public void ToGuidString_SameInteger_ReturnsConsistentGuid()
    {
        // Arrange
        const int value = 987653145;

        // Act
        string guid1 = value.ToGuidString();
        string guid2 = value.ToGuidString();

        // Assert
        guid1.Should().Be(guid2, because: "the same integer should always produce the same GUID");
    }

    [Fact]
    public void ToGuidString_is_deterministic()
    {
        const int value = 987653145;

        string guid1 = value.ToGuidString();

        // Don't want this to change
        guid1.Should().Be("3ade6419-8d00-0650-65ff-4c5bcbd204a6");
    }

    [Fact]
    public void ToGuidString_DifferentIntegers_ProduceDifferentGuids()
    {
        // Arrange
        const int value1 = 12345;
        const int value2 = 67890;

        // Act
        string guid1 = value1.ToGuidString();
        string guid2 = value2.ToGuidString();

        // Assert
        guid1.Should().NotBe(guid2, because: "different integers should generate different GUIDs");
    }

    [Fact]
    public void ToGuidString_Zero_ShouldReturnValidGuid()
    {
        // Arrange
        const int value = 0;

        // Act
        string guidString = value.ToGuidString();

        // Assert
        guidString.Should().NotBeNullOrEmpty().And.MatchRegex(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$");
    }

    [Fact]
    public void ToGuidString_MaxValue_ShouldReturnValidGuid()
    {
        // Arrange
        const int value = int.MaxValue;

        // Act
        string guidString = value.ToGuidString();

        // Assert
        guidString.Should().NotBeNullOrEmpty().And.MatchRegex(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$");
    }

    [Fact]
    public void ToGuidString_MinValue_ShouldReturnValidGuid()
    {
        // Arrange
        const int value = int.MinValue;

        // Act
        string guidString = value.ToGuidString();

        // Assert
        guidString.Should().NotBeNullOrEmpty().And.MatchRegex(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$");
    }
}