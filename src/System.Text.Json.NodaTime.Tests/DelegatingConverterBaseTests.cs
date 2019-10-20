namespace System.Text.Json.NodaTime.Tests {

    using global::NodaTime;
    using global::NodaTime.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    [TestClass]
    public class DelegatingConverterBaseTests {

        private readonly JsonSerializerOptions options;

        private class Entity {
            [JsonConverter(typeof(ShortDateConverter))]
            public LocalDate ShortDate { get; set; }

            [JsonConverter(typeof(LongDateConverter))]
            public LocalDate LongDate { get; set; }
        }

        private class ShortDateConverter : DelegatingConverterBase<LocalDate> {
            public ShortDateConverter() : base(NodaConverters.LocalDateConverter) { }
        }

        private class LongDateConverter : DelegatingConverterBase<LocalDate> {
            // No need to create a new one of these each time...
            private static readonly JsonConverter<LocalDate> converter =
                new NodaPatternConverter<LocalDate>(LocalDatePattern.CreateWithInvariantCulture("d MMMM yyyy"));

            public LongDateConverter() : base(converter) {
            }
        }

        private class NullProviderConverter : DelegatingConverterBase<LocalDate> {
            internal NullProviderConverter() : base(null) {
            }
        }

        public DelegatingConverterBaseTests() {
            options = new JsonSerializerOptions();
            options.Converters.Add(new ShortDateConverter());
            options.Converters.Add(new LongDateConverter());
        }

        [TestMethod]
        // Assert
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowOnNullProvider() {
            new NullProviderConverter();
        }

        [TestMethod]
        public void Serialize() {
            // Arrange
            string expected = "{'ShortDate':'2017-02-20','LongDate':'20 February 2017'}"
                .Replace("'", "\"");
            var date = new LocalDate(2017, 2, 20);
            var entity = new Entity { ShortDate = date, LongDate = date };

            // Act
            var actual = JsonSerializer.Serialize(entity, options);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Deserialize() {
            // Arrange
            string json = "{'ShortDate':'2017-02-20','LongDate':'20 February 2017'}"
                .Replace("'", "\"");
            var expectedDate = new LocalDate(2017, 2, 20);

            // Act
            var entity = JsonSerializer.Deserialize<Entity>(json);

            // Assert
            Assert.AreEqual(expectedDate, entity.ShortDate);
            Assert.AreEqual(expectedDate, entity.LongDate);
        }
    }
}