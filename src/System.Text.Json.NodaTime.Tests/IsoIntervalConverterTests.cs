namespace System.Text.Json.NodaTime.Tests {

    using global::NodaTime;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Text.Json;

    [TestClass]
    public class IsoIntervalConverterTests {

        private readonly JsonSerializerOptions options;

        public IsoIntervalConverterTests() {
            options = new JsonSerializerOptions();
            options.Converters.Add(NodaConverters.IsoIntervalConverter);
        }

        [TestMethod]
        public void ShouldCompleteFiniteRoundTrip() {

            // Arrange
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5) + Duration.FromMilliseconds(670);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10) + Duration.FromNanoseconds(123456789);
            var interval = new Interval(startInstant, endInstant);

            // Act
            var actualJson = JsonSerializer.Serialize<Interval>(interval, options);
            var deserializedValue = JsonSerializer.Deserialize<Interval>(actualJson, options);

            // Assert
            Assert.AreEqual($"\"{interval.ToString()}\"", actualJson);
            Assert.AreEqual(interval, deserializedValue);
        }

        [TestMethod]
        public void ShouldCompleteInfiniteRoundTrip() {
            // Arrange
            var instant = Instant.FromUtc(2013, 6, 7, 8, 9, 10) + Duration.FromNanoseconds(123456789);

            var starttoinfinity = new Interval(instant, null);
            var infintytoend = new Interval(null, instant);
            var infinitytoinfinity = new Interval(null, null);

            // Act
            var starttoinfinityjson = JsonSerializer.Serialize<Interval>(starttoinfinity, options);
            var infintytoendjson = JsonSerializer.Serialize<Interval>(infintytoend, options);
            var infinitytoinfinityjson = JsonSerializer.Serialize<Interval>(infinitytoinfinity, options);

            var starttoinfinityobj = JsonSerializer.Deserialize<Interval>(starttoinfinityjson, options);
            var infintytoendobj = JsonSerializer.Deserialize<Interval>(infintytoendjson, options);
            var infinitytoinfinitobj = JsonSerializer.Deserialize<Interval>(infinitytoinfinityjson, options);

            // Assert
            Assert.AreEqual("\"2013-06-07T08:09:10.123456789Z/\"", starttoinfinityjson);
            Assert.AreEqual("\"/2013-06-07T08:09:10.123456789Z\"", infintytoendjson);
            Assert.AreEqual("\"/\"", infinitytoinfinityjson);

            Assert.AreEqual(starttoinfinity, starttoinfinityobj);
            Assert.AreEqual(infintytoend, infintytoendobj);
            Assert.AreEqual(infinitytoinfinity, infinitytoinfinitobj);

        }

        [TestMethod]
        public void ShouldDeserializeWhenCommaIsUsedInsteadOfPeriod() {

            // Arrange
            // Comma is deliberate, to show that we can parse a comma decimal separator too.
            string json = "\"2012-01-02T03:04:05.670Z/2013-06-07T08:09:10,1234567Z\"";
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5) + Duration.FromMilliseconds(670);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10) + Duration.FromTicks(1234567);
            var expectedInterval = new Interval(startInstant, endInstant);

            // Act
            var interval = JsonSerializer.Deserialize<Interval>(json, options);

            // Assert
            Assert.AreEqual(expectedInterval, interval);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DataRow("\"2012-01-02T03:04:05Z2013-06-07T08:09:10Z\"")]
        public void ShouldThrowOnInvalidJson(string dodgjson) {
            JsonSerializer.Deserialize<Interval>(dodgjson, options);
        }


        [TestMethod]
        // Assert
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowOnNonStringValue() {
            // Arrange
            var json = "5";

            // Act
            JsonSerializer.Deserialize<Interval>(json, options);
        }

    }
}