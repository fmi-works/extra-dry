﻿using ExtraDry.Core;
using Sample.Shared;

namespace YellowJacket.Tests.Shared.Entities;

public class RegionTests {

    [Theory]
    [InlineData("Id", 2)]
    [InlineData("Code", "US")]
    [InlineData("Level", RegionLevel.Division)]
    [InlineData("Title", "USA")]
    [InlineData("Description", "United States of America")]

    public void RoundtripProperties(string propertyName, object propertyValue)
    {
        var region = ValidRegion;
        var property = region.GetType().GetProperty(propertyName)!;

        property.SetValue(region, propertyValue);
        var result = property.GetValue(region);

        Assert.Equal(propertyValue, result);
    }

    [Fact]
    public void ValidateValidRegion()
    {
        var request = ValidRegion;
        var validator = new DataValidator();

        validator.ValidateObject(request);

        Assert.Empty(validator.Errors);
    }

    [Theory]
    [InlineData("Title", "")] // required
    [InlineData("Title", "0123456789012345678901234567890123456789")]
    [InlineData("Description", "")]
    [InlineData("Description", "01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123")]
    public void InvalidPropertyValue(string propertyName, string propertyValue)
    {
        var request = ValidRegion;
        var validator = new DataValidator();
        var property = request.GetType().GetProperty(propertyName)!;

        property.SetValue(request, propertyValue);
        validator.ValidateObject(request);

        Assert.NotEmpty(validator.Errors);
    }

    [Theory]
    [InlineData(RegionLevel.Country, "AU")]
    [InlineData(RegionLevel.Country, "US")]
    [InlineData(RegionLevel.Division, "AU-QLD")]
    [InlineData(RegionLevel.Division, "US-CO")]
    public void ValidCodesForLevel(RegionLevel level, string code)
    {
        var request = ValidRegion;
        request.Code = code;
        request.Level = level;
        var validator = new DataValidator();

        validator.ValidateObject(request);

        Assert.Empty(validator.Errors);
    }

    [Theory]
    [InlineData(RegionLevel.Country, "AUS")]
    [InlineData(RegionLevel.Country, "U")]
    [InlineData(RegionLevel.Country, "")]
    [InlineData(RegionLevel.Country, "W!")]
    [InlineData(RegionLevel.Division, "AU-Q")]
    [InlineData(RegionLevel.Division, "AU-QABCDEF")]
    [InlineData(RegionLevel.Subdivision, "AU-QLD-")]
    [InlineData(RegionLevel.Division, "AU-QLD-ThisSuburbHasTooLongOfAName")]
    // valid codes at wrong level
    [InlineData(RegionLevel.Country, "AU-QLD")]
    [InlineData(RegionLevel.Country, "AU-QLD-Brisbane")]
    [InlineData(RegionLevel.Division, "AU")]
    [InlineData(RegionLevel.Division, "AU-QLD-Brisbane")]
    [InlineData(RegionLevel.Subdivision, "AU")]
    [InlineData(RegionLevel.Subdivision, "AU-QLD")]
    public void InvalidCodesForLevel(RegionLevel level, string code)
    {
        var request = ValidRegion;
        request.Code = code;
        request.Level = level;
        var validator = new DataValidator();

        validator.ValidateObject(request);

        Assert.NotEmpty(validator.Errors);
    }

    [Theory]
    [InlineData("Id", 123456)]
    [InlineData("Level", (int)RegionLevel.Subdivision)] // don't show number through Strata
    public void JsonIgnoreValue(string propertyName, object propertyValue)
    {
        var request = ValidRegion;
        var property = request.GetType().GetProperty(propertyName)!;

        property.SetValue(request, propertyValue);
        var json = JsonSerializer.Serialize(request);

        Assert.DoesNotContain(propertyValue.ToString(), json);
    }

    public static Region ValidRegion => new() {
        Id = 1,
        Code = "AU",
        Level = RegionLevel.Country,
        Title = "Australia",
        Description = "Commonwealth of Australia",
        ParentCode = "",
    };

}
