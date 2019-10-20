namespace System.Text.Json.NodaTime.Tests {
  
    using global::NodaTime;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text.Json;

    [TestClass]
    public class InstantConverterTests  {

        private readonly JsonSerializerOptions options;

        public InstantConverterTests() {
            options = new JsonSerializerOptions();
            options.Converters.Add(NodaConverters.InstantConverter);
        }

        [TestMethod]
        public void ShouldSerializeNonNullableType() {
            // Arrange
            var instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            string json = "\"2012-01-02T03:04:05Z\"";

            // Act
            var seri = JsonSerializer.Serialize(instant, options);
            
            // Assert
            Assert.AreEqual(json, seri);
        }

        [TestMethod]
        public void ShouldSerializeNullableTypeWithNonNullValue() {
            // Arrange
            Instant? instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            string json = "\"2012-01-02T03:04:05Z\"";

            // Act
            var serializedInstant = JsonSerializer.Serialize(instant, options);
            
            // Assert
            Assert.AreEqual(json, serializedInstant);
        }

        [TestMethod]
        public void ShouldSerializeNullableTypeWithNullValue() {
            // Arrange
            Instant? instant = null;
            string json = "null";
            
            // Act
            var serializedInstant = JsonSerializer.Serialize(instant, options);
            
            // Assert
            Assert.AreEqual(json, serializedInstant);
        }

        [TestMethod]
        public void ShouldDeserializeToNonNullableType() {
            // Arrange
            string json = "\"2012-01-02T03:04:05Z\"";
            var expectedInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);

            // Act
            var instant = JsonSerializer.Deserialize<Instant>(json, options);
            
            // Assert
            Assert.AreEqual(expectedInstant, instant);
        }

        [TestMethod]
        public void ShouldDeserializeToNullableTypeWithNonNullValue() {
            // Arrange
            string json = "\"2012-01-02T03:04:05Z\"";
            var expectedInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);

            // Act
            var instant = JsonSerializer.Deserialize<Instant?>(json, options);
            
            // Assert
            Assert.AreEqual(expectedInstant, instant);
        }

        [TestMethod]
        public void ShouldDeserializeToNullableTypeWitNullValue() {
            // Arrange
            string json = "null";
            
            // Act
            var instant = JsonSerializer.Deserialize<Instant?>(json, options);
            
            // Assert
            Assert.IsNull(instant);
        }
    }
}