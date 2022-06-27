﻿namespace ExtraDry.Core.Tests.Helpers;

public class DataConverterTests {

    [Theory]
    [InlineData(TestEnum.SimpleName, "SimpleName")]
    [InlineData(TestEnum.DisplayName, "Display Name")]
    [InlineData((TestEnum)10, "10")]
    public void TestDataConverter(TestEnum test, string expected)
    {
        var actual = DataConverter.DisplayEnum(test);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NullEnumDisplayException()
    {
        Assert.Throws<ArgumentNullException>(() => DataConverter.DisplayEnum(null));
    }

    public enum TestEnum {

        SimpleName = 0,

        [Display(Name = "Display Name")]
        DisplayName = 1,

    }
    
    [Theory]
    [InlineData("Test", "Test")]
    [InlineData("TEST", "TEST")]
    [InlineData("TestTest", "Test Test")]
    [InlineData("TESTTest", "TEST Test")]
    [InlineData("ABC123", "ABC123")]
    [InlineData("Abc123", "Abc123")]
    [InlineData("123Abc", "123 Abc")]
    public void TitleCasing(string uncased, string expected)
    {
        var spaced = DataConverter.CamelCaseToTitleCase(uncased);

        Assert.Equal(expected, spaced);
    }

    [Theory]
    [InlineData(-1, "Just now")]
    [InlineData(-60, "A minute ago")]
    [InlineData(-600, "10 minutes ago")]
    [InlineData(-60 * 60, "An hour ago")]
    [InlineData(-2 * 60 * 60, "2 hours ago")]
    [InlineData(-24 * 60 * 60, "Yesterday {0:hh:mm tt}")]
    [InlineData(-48 * 60 * 60, "{0:ddd hh:mm tt}")]
    [InlineData(-240 * 60 * 60, "{0:MMM dd hh:mm tt}")]
    public void RelativeDatesDisplay(int secondOffset, string expected)
    {
        DataConverter.CurrentDateTime = () => new DateTime(2022, 06, 30, 8, 0, 0);
        var current = DataConverter.CurrentDateTime().AddSeconds(secondOffset);

        var display = DataConverter.DateToRelativeTime(current);

        expected = string.Format(expected, current);
        Assert.Equal(expected, display);
    }

}