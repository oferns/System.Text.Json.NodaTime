namespace System.Text.Json.NodaTime.Tests {

    using global::NodaTime;
    using global::NodaTime.TimeZones;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text.Json;

    [TestClass]
    public class DateTimeZoneCoverterTests {

        private readonly JsonSerializerOptions options;

        public DateTimeZoneCoverterTests() {
            options = new JsonSerializerOptions();
            options.Converters.Add(NodaConverters.CreateDateTimeZoneConverter(DateTimeZoneProviders.Tzdb));
        }

        [TestMethod]
        public void ShouldSerializeAndDeserialize() {
            // Arrange
            var dateTimeZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
            string json = "\"America/Los_Angeles\"";

            // Act
            var seri = JsonSerializer.Serialize(dateTimeZone, options);
            var deseri = JsonSerializer.Deserialize<DateTimeZone>(seri, options);

            // Assert
            Assert.AreEqual(json, seri);
            Assert.AreEqual(dateTimeZone, deseri);
        }

        [TestMethod]
        // Assert
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowOnNullProvider() {
            // Arrange
            // Act
            new DateTimeZoneConverter(null);
        }

        [TestMethod]
        // Assert
        [ExpectedException(typeof(DateTimeZoneNotFoundException))]
        public void ShouldThrowSerializationExceptionOnUnknownTimeZone() {
            // Arrange
            string json = "\"America/DOES_NOT_EXIST\"";

            // Act
            JsonSerializer.Deserialize<DateTimeZone>(json, options);
        }

        [TestMethod]
        // Assert
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowOnWrongTokenType() {
            // Arrange
            string json = "5";

            // Act
            JsonSerializer.Deserialize<DateTimeZone>(json, options);
        }
    }
}