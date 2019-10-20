namespace System.Text.Json.NodaTime.Tests {

    using global::NodaTime;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text.Json;

    [TestClass]
    public class IntervalConverterTests {

        private readonly JsonSerializerOptions options;
        private readonly JsonSerializerOptions camelOptions;
        private class TestObject {
            public Interval Interval { get; set; }
        }

        public IntervalConverterTests() {
            options = new JsonSerializerOptions();
            camelOptions = new JsonSerializerOptions();

            options.Converters.Add(NodaConverters.IntervalConverter);
            options.Converters.Add(NodaConverters.InstantConverter);

            camelOptions.Converters.Add(NodaConverters.IntervalConverter);
            camelOptions.Converters.Add(NodaConverters.InstantConverter);
            camelOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
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
            Assert.AreEqual("{\"Start\":\"2012-01-02T03:04:05.67Z\",\"End\":\"2013-06-07T08:09:10.123456789Z\"}", actualJson);
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
            Assert.AreEqual("{\"Start\":\"2013-06-07T08:09:10.123456789Z\"}", starttoinfinityjson);
            Assert.AreEqual("{\"End\":\"2013-06-07T08:09:10.123456789Z\"}", infintytoendjson);
            Assert.AreEqual("{}", infinitytoinfinityjson);

            Assert.AreEqual(starttoinfinity, starttoinfinityobj);
            Assert.AreEqual(infintytoend, infintytoendobj);
            Assert.AreEqual(infinitytoinfinity, infinitytoinfinitobj);

        }

        [TestMethod]
        public void ShouldSerializeAsPropertyOfObject() {
            // Arrange
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var interval = new Interval(startInstant, endInstant);

            var testObject = new TestObject { Interval = interval };

            string expectedJson = "{\"Interval\":{\"Start\":\"2012-01-02T03:04:05Z\",\"End\":\"2013-06-07T08:09:10Z\"}}";

            // Act
            var json = JsonSerializer.Serialize(testObject, options);

            // Assert
            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void ShouldSerializeAsPropertyOfObjectToCamelCase() {
            // Arrange
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var interval = new Interval(startInstant, endInstant);
            string expectedJson = "{\"interval\":{\"start\":\"2012-01-02T03:04:05Z\",\"end\":\"2013-06-07T08:09:10Z\"}}";

            var testObject = new TestObject { Interval = interval };

            // Act
            var json = JsonSerializer.Serialize(testObject, camelOptions);

            // Assert
            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void ShouldDeserializeAsPropertyOfObject() {
            // Arrange
            string intervalJson = "{\"Interval\":{\"Start\":\"2012-01-02T03:04:05Z\",\"End\":\"2013-06-07T08:09:10Z\"}}";

            var expectedInterval = new Interval(Instant.FromUtc(2012, 1, 2, 3, 4, 5), Instant.FromUtc(2013, 6, 7, 8, 9, 10));

            // Act
            var testObject = JsonSerializer.Deserialize<TestObject>(intervalJson, options);

            // Assert
            Assert.AreEqual(expectedInterval, testObject.Interval);
        }

        [TestMethod]
        public void ShouldDeserializeAsPropertyOfObjectInCamelCase() {
            // Arrange
            string json = "{\"interval\":{\"start\":\"2012-01-02T03:04:05Z\",\"end\":\"2013-06-07T08:09:10Z\"}}";

            var expectedInterval = new Interval(Instant.FromUtc(2012, 1, 2, 3, 4, 5), Instant.FromUtc(2013, 6, 7, 8, 9, 10));

            // Act
            var testObject = JsonSerializer.Deserialize<TestObject>(json, camelOptions);

            // Assert
            Assert.AreEqual(expectedInterval, testObject.Interval);
        }
    }
}