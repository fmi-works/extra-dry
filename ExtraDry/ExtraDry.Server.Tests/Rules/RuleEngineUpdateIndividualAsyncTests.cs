using ExtraDry.Core.Tests.Rules;

namespace ExtraDry.Server.Tests.Rules {
    public class RuleEngineUpdateIndividualAsyncTests {

        [Fact]
        public async Task IdentityUnchanged()
        {
            var rules = new RuleEngine(new ServiceProviderStub());
            var source = SampleEntity();
            source.Id = 0;
            var destination = SampleEntity();

            await rules.UpdateAsync(source, destination);

            var original = SampleEntity();
            Assert.Equal(original.Id, destination.Id);
            Assert.Equal(original.UndecoratedName, destination.UndecoratedName);
            Assert.Equal(original.IgnoreChangesAmount, destination.IgnoreChangesAmount);
            Assert.Equal(original.DefaultIgnoredReal, destination.DefaultIgnoredReal);
            Assert.Equal(original.ChangeableUuid, destination.ChangeableUuid);
            Assert.Equal(original.DefaultIgnoredString, destination.DefaultIgnoredString);
        }

        [Fact]
        public async Task BlockChangesThrowsException()
        {
            var rules = new RuleEngine(new ServiceProviderStub());
            var source = SampleEntity();
            var destination = SampleEntity();
            source.HoursWorked = 2;

            await Assert.ThrowsAsync<DryException>(async () => await rules.UpdateAsync(source, destination));
        }

        [Fact]
        public async Task IgnoreChanges()
        {
            var rules = new RuleEngine(new ServiceProviderStub());
            var source = SampleEntity();
            var destination = SampleEntity();
            source.IgnoreChangesAmount = 2.34m;

            await rules.UpdateAsync(source, destination);

            var original = SampleEntity();
            Assert.Equal(original.Id, destination.Id);
        }

        [Fact]
        public async Task JsonIgnoreChanges()
        {
            var rules = new RuleEngine(new ServiceProviderStub());
            var source = SampleEntity();
            var destination = SampleEntity();
            source.JsonIgnored = "different";
            destination.JsonIgnored = "json";

            await rules.UpdateAsync(source, destination);

            Assert.Equal("json", destination.JsonIgnored);
        }

        [Fact]
        public async Task JsonIgnoreChangesWithConditionFakout()
        {
            var rules = new RuleEngine(new ServiceProviderStub());
            var source = SampleEntity();
            var destination = SampleEntity();
            source.JsonIgnoredFakeOut = "different";
            destination.JsonIgnoredFakeOut = "json";

            await rules.UpdateAsync(source, destination);

            Assert.Equal("different", destination.JsonIgnoredFakeOut);
        }

        [Fact]
        public async Task UndecoratedDefaultsToAllow()
        {
            var rules = new RuleEngine(new ServiceProviderStub());
            var source = SampleEntity();
            var destination = SampleEntity();
            source.UndecoratedName = "Alice";

            await rules.UpdateAsync(source, destination);

            Assert.Equal("Alice", destination.UndecoratedName);
        }

        [Fact]
        public async Task IgnoreDefaultsValueTypeChangeValue()
        {
            var rules = new RuleEngine(new ServiceProviderStub());
            var source = SampleEntity();
            var destination = SampleEntity();
            source.DefaultIgnoredReal = 123;

            await rules.UpdateAsync(source, destination);

            Assert.Equal(123, destination.DefaultIgnoredReal);
        }

        //[Fact]
        //public async Task IgnoreDefaultsValueTypeIgnoresDefaults()
        //{
        //    var rules = new RuleEngine(new ServiceProviderStub());
        //    var source = SampleEntity();
        //    var destination = SampleEntity();
        //    source.DefaultIgnoredReal = 0;
        //    destination.DefaultIgnoredReal = 1.23;

        //    await rules.UpdateAsync(source, destination);

        //    Assert.Equal(1.23, destination.DefaultIgnoredReal);
        //}

        [Fact]
        public async Task IgnoreDefaultsReferenceTypeIgnoresDefaults()
        {
            var rules = new RuleEngine(new ServiceProviderStub());
            var source = SampleEntity();
            var destination = SampleEntity();
            source.DefaultIgnoredString = null!;
            destination.DefaultIgnoredString = "something";

            await rules.UpdateAsync(source, destination);

            Assert.Equal("something", destination.DefaultIgnoredString);
        }

        [Fact]
        public async Task IgnoreDefaultsReferenceTypeChanges()
        {
            var rules = new RuleEngine(new ServiceProviderStub());
            var source = SampleEntity();
            var destination = SampleEntity();
            source.DefaultIgnoredString = "else";
            destination.DefaultIgnoredString = "something";

            await rules.UpdateAsync(source, destination);

            Assert.Equal("else", destination.DefaultIgnoredString);
        }

        [Fact]
        public async Task ExplicitAllowChanges()
        {
            var rules = new RuleEngine(new ServiceProviderStub());
            var source = SampleEntity();
            var destination = SampleEntity();
            source.ChangeableUuid = Guid.NewGuid();
            destination.ChangeableUuid = Guid.NewGuid();

            await rules.UpdateAsync(source, destination);

            Assert.Equal(source.ChangeableUuid, destination.ChangeableUuid);
        }

        [Fact]
        public async Task DestinationNotNull()
        {
            var rules = new RuleEngine(new ServiceProviderStub());
            var source = SampleEntity();
            var destination = SampleEntity();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await rules.UpdateAsync(source, null));
        }

        [Fact]
        public async Task SourceNotNull()
        {
            var rules = new RuleEngine(new ServiceProviderStub());
            var source = SampleEntity();
            var destination = SampleEntity();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await rules.UpdateAsync(null, destination));
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "abc")]
        [InlineData("abc", null)]
        [InlineData("abc", "abc")]
        [InlineData("abc", "def")]
        public async Task SourceToDestinationUncontrolled(string input, string output)
        {
            var rules = new RuleEngine(new ServiceProviderStub());
            var source = SampleEntity();
            var destination = SampleEntity();
            source.UndecoratedName = input;
            destination.UndecoratedName = output;

            await rules.UpdateAsync(source, destination);

            Assert.Equal(input, destination.UndecoratedName);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("abc", "abc")]
        public async Task SourceToDestinationOnBlockChanges(string input, string output)
        {
            var rules = new RuleEngine(new ServiceProviderStub());
            var source = SampleEntity();
            var destination = SampleEntity();
            source.BlockChangesString = input;
            destination.BlockChangesString = output;

            await rules.UpdateAsync(source, destination);

            Assert.Equal(output, destination.BlockChangesString);
        }

        public class Entity {

            [JsonIgnore]
            [Rules(RuleAction.Block)]
            public int Id { get; set; } = 1;

            public string UndecoratedName { get; set; } = "Bob";

            [Rules(RuleAction.Ignore)]
            public decimal IgnoreChangesAmount { get; set; } = 123;

            [Rules(RuleAction.IgnoreDefaults)]
            public double DefaultIgnoredReal { get; set; } = 1.23;

            [Rules(RuleAction.Allow)]
            public Guid ChangeableUuid { get; set; } = new Guid("372844B3-4963-4129-A4DE-AF457FED1A55");

            [Rules(RuleAction.IgnoreDefaults)]
            public string DefaultIgnoredString { get; set; } = "Something";

            [JsonIgnore]
            public string JsonIgnored { get; set; } = "json";

            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            public string JsonIgnoredFakeOut { get; set; } = "json";

            [Rules(RuleAction.Block)]
            public string? BlockChangesString { get; set; }

            [Rules(RuleAction.Block)]
            public int HoursWorked { get; set; }

            public int ReadOnly {
                get => HoursWorked;
            }
        }

        private static Entity SampleEntity() => new();

    }
}
