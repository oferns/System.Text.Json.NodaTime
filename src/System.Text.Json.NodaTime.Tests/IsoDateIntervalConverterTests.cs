namespace System.Text.Json.NodaTime.Tests {
    using global::NodaTime;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Text.Json;

    [TestClass]
    public class IsoDateIntervalConverterTests {

        private readonly JsonSerializerOptions options;
        private class TestObject {
            public DateInterval Interval { get; set; }
        }

        public IsoDateIntervalConverterTests() {
            options = new JsonSerializerOptions();
            options.Converters.Add(NodaConverters.IsoDateIntervalConverter);
            options.Converters.Add(NodaConverters.LocalDateConverter);
        }

        [TestMethod]
        public void ShouldCompleteRoundTrip() {
            // Arrange
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);
            var json = "\"2012-01-02/2013-06-07\"";

            // Act
            var seri = JsonSerializer.Serialize(dateInterval, options);
            var deseri = JsonSerializer.Deserialize<DateInterval>(seri, options);

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
            var testObject = new TestObject { Interval = dateInterval };
            string json = "{\"Interval\":\"2012-01-02/2013-06-07\"}";

            // Act
            var seri = JsonSerializer.Serialize(testObject, options);
            var deseri = JsonSerializer.Deserialize<TestObject>(seri, options);

            // Assert
            Assert.AreEqual(json, seri);
            Assert.AreEqual(testObject.Interval, deseri.Interval);

        }

        [TestMethod]
        // Arrange
        [DataRow("\"2012-01-022013-06-07\"")]
        // Assert
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowSerializtionException(string json) {
            // Act
            JsonSerializer.Deserialize<DateInterval>(json, options);
        }

        [TestMethod]
        // Assert
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowOnNullStartDate() {
            // Arrange
            var json = "\"/2013-06-07\"";

            // Act
            JsonSerializer.Deserialize<DateInterval>(json, options);
        }

        [TestMethod]
        // Assert
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowOnNullEndDate() {
            // Arrange
            var json = "\"2013-06-07/\"";

            // Act
            JsonSerializer.Deserialize<DateInterval>(json, options);
        }

        [TestMethod]
        // Assert
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowOnNonStringValue() {
            // Arrange
            var json = "5";

            // Act
            JsonSerializer.Deserialize<DateInterval>(json, options);
        }
    }
}