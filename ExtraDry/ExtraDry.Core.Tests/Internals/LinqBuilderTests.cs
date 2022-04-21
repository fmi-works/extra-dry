using ExtraDry.Server.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using Xunit;

namespace ExtraDry.Core.Tests.Internals {
    public class LinqBuilderTests {

        [Fact]
        public void OrderByNameCompatible()
        {
            var linqSorted = SampleData.OrderBy(e => e.FirstName).ToList();

            var linqBuilderSorted = SampleData.AsQueryable().OrderBy("FirstName", new ModelDescription(typeof(Datum))).ToList();

            Assert.Equal(linqSorted, linqBuilderSorted);
        }

        [Fact]
        public void OrderByNumberCompatible()
        {
            var linqSorted = SampleData.OrderBy(e => e.Number).ToList();

            var linqBuilderSorted = SampleData.AsQueryable().OrderBy("Number", new ModelDescription(typeof(Datum))).ToList();

            Assert.Equal(linqSorted, linqBuilderSorted);
        }

        [Fact]
        public void OrderByPublicNameCompatible()
        {
            var linqSorted = SampleData.OrderBy(e => e.InternalName).ToList();

            var linqBuilderSorted = SampleData.AsQueryable().OrderBy("PublicName", new ModelDescription(typeof(Datum))).ToList();

            Assert.Equal(linqSorted, linqBuilderSorted);
        }

        [Fact]
        public void OrderByDescendingNameCompatible()
        {
            var linqSorted = SampleData.OrderByDescending(e => e.FirstName).ToList();

            var linqBuilderSorted = SampleData.AsQueryable().OrderByDescending("FirstName", new ModelDescription(typeof(Datum))).ToList();

            Assert.Equal(linqSorted, linqBuilderSorted);
        }

        [Fact]
        public void OrderByDescendingNumberCompatible()
        {
            var linqSorted = SampleData.OrderByDescending(e => e.Number).ToList();

            var linqBuilderSorted = SampleData.AsQueryable().OrderByDescending("Number", new ModelDescription(typeof(Datum))).ToList();

            Assert.Equal(linqSorted, linqBuilderSorted);
        }

        [Fact]
        public void OrderByInvalidNameException()
        {
            Assert.Throws<DryException>(() =>
                SampleData.AsQueryable().OrderByDescending("Invalid", new ModelDescription(typeof(Datum))).ToList()
            );
        }

        [Fact]
        public void ThenByCompatible()
        {
            var linqSorted = SampleData.OrderBy(e => e.FirstName).ThenBy(e => e.Number).ToList();
            var modelDescription = new ModelDescription(typeof(Datum));

            var linqBuilderSorted = SampleData.AsQueryable().OrderBy("FirstName", modelDescription).ThenBy("Number", modelDescription).ToList();

            Assert.Equal(linqSorted, linqBuilderSorted);
        }

        [Fact]
        public void ThenByDescendingCompatible()
        {
            var linqSorted = SampleData.OrderByDescending(e => e.FirstName).ThenByDescending(e => e.Number).ToList();
            var modelDescription = new ModelDescription(typeof(Datum));

            var linqBuilderSorted = SampleData.AsQueryable().OrderByDescending("FirstName", modelDescription).ThenByDescending("Number", modelDescription).ToList();

            Assert.Equal(linqSorted, linqBuilderSorted);
        }

        [Fact]
        public void SingleEqualsWhereFilterCompatible()
        {
            var linqWhere = SampleData.Where(e => e.FirstName == "Bob").ToList();

            var filterProperty = GetFilterProperty("FirstName");
            var linqBuilderWhere = SampleData.AsQueryable().WhereFilterConditions(new FilterProperty[] { filterProperty }, "firstname:Bob").ToList();

            Assert.Equal(linqWhere, linqBuilderWhere);
        }

        [Fact]
        public void SingleStartsWithWhereFilterCompatible()
        {
            var linqWhere = SampleData.Where(e => e.LastName.StartsWith("Bark")).ToList();

            var filterProperty = GetFilterProperty("LastName");
            var linqBuilderWhere = SampleData.AsQueryable().WhereFilterConditions(new FilterProperty[] { filterProperty }, "LastName:Bark").ToList();

            Assert.Equal(linqWhere, linqBuilderWhere);
        }

        [Fact]
        public void MultipleWhereFilterCompatible()
        {
            var linqWhere = SampleData.Where(e => e.FirstName == "Bob" || e.LastName.StartsWith("Bark")).ToList();

            var firstName = GetFilterProperty("FirstName");
            var lastName = GetFilterProperty("LastName");
            var linqBuilderWhere = SampleData.AsQueryable().WhereFilterConditions(new FilterProperty[] { firstName, lastName }, "firstname:Bob lastname:Bark").ToList();

            Assert.Equal(linqWhere, linqBuilderWhere);
        }

        private static FilterProperty GetFilterProperty(string propertyName)
        {
            var property = typeof(Datum).GetProperty(propertyName);
            var filter = property.GetCustomAttributes(false).First() as FilterAttribute;
            return new FilterProperty(property, filter, property.Name);
        }

        public class Datum {

            [JsonIgnore]
            [Key]
            public int Id { get; set; }

            [Filter(FilterType.Equals)]
            public string FirstName { get; set; }

            [Filter(FilterType.StartsWith)]
            public string LastName { get; set; }

            [Filter] //(FilterType.Contains)] TODO: Implement contains with full text index.
            public string Keywords { get; set; }

            public int Number { get; set; }

            [JsonPropertyName("publicName")]
            public string InternalName { get; set; }
        }

        private readonly List<Datum> SampleData = new() {
            new Datum { FirstName = "Charlie", LastName = "Coase", Number = 111 },
            new Datum { FirstName = "Alice", LastName = "Cooper", Number = 333 },
            new Datum { FirstName = "Bob", LastName = "Barker", Number = 222 },
        };

        //private readonly List<Datum> SampleDataWithDuplicateNames = new() {
        //    new Datum { FirstName = "Charlie", LastName = "Coase", Number = 111},
        //    new Datum { FirstName = "Alice", LastName = "Cooper", Number = 333 },
        //    new Datum { FirstName = "Bob", LastName = "Barker", Number = 222 },
        //    new Datum { FirstName = "Alice", LastName = "Barker", Number = 123 },
        //    new Datum { FirstName = "Bob", LastName = "Ross", Number = 321 },
        //};

    }
}
