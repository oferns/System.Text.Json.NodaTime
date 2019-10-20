namespace System.Text.Json.NodaTime.Tests {

    using global::NodaTime;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text.Json;

    [TestClass]
    public class DateIntervalConverterTests {

        private readonly JsonSerializerOptions options;
        private readonly JsonSerializerOptions camelOptions;
        private class TestObject {
            public DateInterval Interval { get; set; }
        }

        public DateIntervalConverterTests() {
            options = new JsonSerializerOptions();
            options.Converters.Add(NodaConverters.DateIntervalConverter);
            options.Converters.Add(NodaConverters.LocalDateConverter);

            camelOptions = new JsonSerializerOptions();
            camelOptions.Converters.Add(NodaConverters.DateIntervalConverter);
            camelOptions.Converters.Add(NodaConverters.LocalDateConverter);
            camelOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        }

        [TestMethod]
        public void ShouldCompleteRoundTrip() {
            // Arrange
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);
            var json = "{\"Start\":\"2012-01-02\",\"End\":\"2013-06-07\"}";

            // Act
            var seri = JsonSerializer.Serialize(dateInterval, options);
            var deseri = JsonSerializer.Deserialize<DateInterval>(seri, options);

            // Assert
            Assert.AreEqual(json, seri);
            Assert.AreEqual(dateInterval, deseri);
        }

        [TestMethod]
        public void ShouldCompleteRoundTripCamelCase() {
            // Arrange
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);
            var json = "{\"start\":\"2012-01-02\",\"end\":\"2013-06-07\"}";

            // Act
            var seri = JsonSerializer.Serialize(dateInterval, camelOptions);
            var deseri = JsonSerializer.Deserialize<DateInterval>(seri, camelOptions);

            // Assert
            Assert.AreEqual(json, seri);
            Assert.AreEqual(dateInterval, deseri);
        }

        [TestMethod]
        public void ShouldCompleteRoundTripAsProperty() {
            // Arrange
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);

            string json = "{\"Interval\":{\"Start\":\"2012-01-02\",\"End\":\"2013-06-07\"}}";

            var testObject = new TestObject { Interval = dateInterval };

            // Act
            var seri = JsonSerializer.Serialize(testObject, options);
            var deseri = JsonSerializer.Deserialize<TestObject>(seri, options);

            // Assert
            Assert.AreEqual(json, seri);
            Assert.AreEqual(testObject.Interval, deseri.Interval);
        }

        [TestMethod]
        public void ShouldCompleteCamelCaseRoundTripAsProperty() {
            // Arrange
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);

            string json = "{\"interval\":{\"start\":\"2012-01-02\",\"end\":\"2013-06-07\"}}";

            var testObject = new TestObject { Interval = dateInterval };

            // Act
            var seri = JsonSerializer.Serialize(testObject, camelOptions);
            var deseri = JsonSerializer.Deserialize<TestObject>(seri, camelOptions);

            // Assert
            Assert.AreEqual(json, seri);
            Assert.AreEqual(testObject.Interval, deseri.Interval);
        }

        [TestMethod]
        // Assert
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowWhenEndDateIsNull() {
            // Arrange
            var json = "{\"Start\":\"2012-01-02\",\"End\":null}";

            // Act
            JsonSerializer.Deserialize<DateInterval>(json, options);            
        }

        [TestMethod]
        // Assert
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowWhenStartDateIsNull() {
            // Arrange
            var json = "{\"Start\":null,\"End\":\"2013-06-07\"}";

            // Act
            JsonSerializer.Deserialize<DateInterval>(json, options);
        }
    }
}