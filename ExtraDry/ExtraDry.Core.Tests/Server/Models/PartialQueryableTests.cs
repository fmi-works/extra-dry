﻿using ExtraDry.Core;
using ExtraDry.Server;
using ExtraDry.Server.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExtraDry.Core.Tests.Server.Models {
    public class PartialQueryableTests {

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task EmptyQueriesReturnEverything(string value)
        {
            var filter = new FilterQuery { Filter = value };
            var expected = Models.ToList();

            var actual = await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }


        [Fact]
        public async Task StringEqualsMatchNone()
        {
            var filter = new FilterQuery { Filter = "name:bob" };
            var expected = Models.Where(e => e.Name.Equals("bob", StringComparison.InvariantCultureIgnoreCase)).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task StringEqualsMatchSingle()
        {
            var filter = new FilterQuery { Filter = "name:bravo" };
            var expected = Models.Where(e => e.Name.Equals("bravo", StringComparison.InvariantCultureIgnoreCase)).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task StringEqualsMatchMultiple()
        {
            var filter = new FilterQuery { Filter = "name:alpha" };
            var expected = Models.Where(e => e.Name.Equals("alpha", StringComparison.InvariantCultureIgnoreCase)).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task SingleIntEquals()
        {
            var filter = new FilterQuery { Filter = "Id:1" };
            var expected = Models.Where(e => e.Id == 1).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task MultipleIntEquals()
        {
            var filter = new FilterQuery { Filter = "Id:1|8" };
            var expected = Models.Where(e => e.Id == 1 || e.Id == 8).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task IntRangeReturnsNone()
        {
            var filter = new FilterQuery { Filter = "Id:[-2,-1)" };
            var expected = Models.Where(e => e.Id >= -2 && e.Id < -1).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task IntRangeReturnsMultiple()
        {
            var filter = new FilterQuery { Filter = "Id:[2,5)" };
            var expected = Models.Where(e => e.Id >= 2 && e.Id < 5).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task ValidEnumValueReturnsNone()
        {
            var filter = new FilterQuery { Filter = "type:latin" };
            var expected = Models.Where(e => e.Type == ModelType.Latin).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task InvalidEnumValueThrowException()
        {
            var filter = new FilterQuery { Filter = "type:invalid" };

            await Assert.ThrowsAsync<DryException>(async () => await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync());
        }

        [Fact]
        public async Task ValidEnumValueReturnsSingle()
        {
            var filter = new FilterQuery { Filter = "type:hendrix" };
            var expected = Models.Where(e => e.Type == ModelType.Hendrix).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task ValidEnumValueReturnsMultiple()
        {
            var filter = new FilterQuery { Filter = "type:greek" };
            var expected = Models.Where(e => e.Type == ModelType.Greek).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task UnnammedFilterReturnsNone()
        {
            var filter = new FilterQuery { Filter = "bob" };
            var expected = Models.Where(e => e.Name == "bob" || e.Soundex.StartsWith("bob")).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Theory]
        [InlineData("alpha")]
        [InlineData("a41")]
        public async Task UnnammedFilterMatchesSingleKeyword(string keyword)
        {
            var filter = new FilterQuery { Filter = keyword };
            var expected = Models.Where(e => e.Name.Equals(keyword, StringComparison.InvariantCultureIgnoreCase) || e.Soundex.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase)).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task UnnammedFilterMatchesIntValue()
        {
            var filter = new FilterQuery { Filter = "10" };
            var expected = Models.Where(e => e.Id == 10).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task NonMatchingFilterWithDefaultLambdaReturnsNone()
        {
            var filter = new FilterQuery { Filter = "nothing" };
            var expected = Models.Where(e => e.Name == "nothing" && e.Type == ModelType.Phonetic).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter, e => e.Type == ModelType.Phonetic).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task NoFilterWithDefaultLambdaReturnsSet(string value)
        {
            var filter = new FilterQuery { Filter = value };
            var expected = Models.Where(e => e.Type == ModelType.Phonetic).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter, e => e.Type == ModelType.Phonetic).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task MatchingFilterWithDefaultLambdaReturnsSingle()
        {
            var filter = new FilterQuery { Filter = "Alpha" };
            var expected = Models.Where(e => e.Name == "Alpha" && e.Type == ModelType.Phonetic).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter, e => e.Type == ModelType.Phonetic).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task MatchingFilterWithDefaultLambdaReturnsMultiple()
        {
            var filter = new FilterQuery { Filter = "id:[5,8]" };
            var expected = Models.Where(e => e.Id >= 5 && e.Id <= 8 && e.Type == ModelType.Phonetic).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter, e => e.Type == ModelType.Phonetic).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }

        [Fact]
        public async Task FilterOverridesLambdaReturnsMultiple()
        {
            var filter = new FilterQuery { Filter = "type:greek" };
            var expected = Models.Where(e => e.Type == ModelType.Greek).ToList();

            var actual = await Models.AsQueryable().QueryWith(filter, e => e.Type == ModelType.Phonetic).ToFilteredCollectionAsync();

            Assert.Equal(expected, actual.Items);
        }
        
        private readonly List<Model> Models = new() {
            new Model { Id = 1, Name = "Alpha", Soundex = "A410", Type = ModelType.Greek, Notes = "Common with phonetic" },
            new Model { Id = 2, Name = "Beta", Soundex = "B300", Type = ModelType.Greek },
            new Model { Id = 3, Name = "Gamma", Soundex = "G500", Type = ModelType.Greek },
            new Model { Id = 4, Name = "Delta", Soundex = "D430", Type = ModelType.Greek, Notes = "Common with phonetic" },
            new Model { Id = 5, Name = "Epsilon", Soundex = "E124", Type = ModelType.Greek },
            new Model { Id = 6, Name = "Zeta", Soundex = "Z300", Type = ModelType.Greek },
            new Model { Id = 7, Name = "Alpha", Soundex = "A410", Type = ModelType.Phonetic, Notes = "Common with Greek" },
            new Model { Id = 8, Name = "Bravo", Soundex = "B610", Type = ModelType.Phonetic },
            new Model { Id = 9, Name = "Charlie", Soundex = "C640", Type = ModelType.Phonetic },
            new Model { Id = 10, Name = "Delta", Soundex = "D430", Type = ModelType.Phonetic, Notes = "Common with Greek" },
            new Model { Id = 11, Name = "Echo", Soundex = "E200", Type = ModelType.Phonetic },
            new Model { Id = 12, Name = "Foxtrot", Soundex = "F236", Type = ModelType.Phonetic },
            new Model { Id = 13, Name = "Foxxy", Soundex = "F200", Type = ModelType.Hendrix, Notes = "Jimi" },
        };

    }

    public class Model {

        [Filter]
        public int Id { get; set; }

        [Filter(FilterType.Equals)]
        public string Name { get; set; }

        [Filter(FilterType.StartsWith)]
        public string Soundex { get; set; }

        [Filter]
        public ModelType Type { get; set; }

        public string Notes { get; set; }
    }

    public enum ModelType {
        Phonetic,
        Greek,
        Hendrix,
        Latin,
    }
}
