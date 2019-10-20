namespace System.Text.Json.NodaTime.Tests {

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text.Json;

    [TestClass]
    public class ConverterBaseTests {

        private class TestIntConverter : ConverterBase<int> {

            protected override int ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options) {
                return reader.GetInt32();
            }

            protected override void WriteJsonImpl(Utf8JsonWriter writer, int value, JsonSerializerOptions options) {
                writer.WriteNumberValue(value);
            }
        }

        private class TestStringConverter : ConverterBase<string> {

            protected override string ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options) {
                return reader.GetString();
            }

            protected override void WriteJsonImpl(Utf8JsonWriter writer, string value, JsonSerializerOptions options) {
                writer.WriteStringValue(value);
            }
        }

        /// <summary>
        /// Just use for CanConvert testing...
        /// </summary>
        private class TestInheritanceConverter : ConverterBase<Stream> {

            protected override Stream ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options) {
                throw new NotImplementedException();
            }

            protected override void WriteJsonImpl(Utf8JsonWriter writer, Stream value, JsonSerializerOptions options) {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void ShouldSerializeNonNullValue() {
            // Arrange
            var options = new JsonSerializerOptions();
            options.Converters.Add(new TestIntConverter());

            // Act
            var result = JsonSerializer.Serialize(5, typeof(int), options);

            // Assert
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void ShouldSerializeNullValueOnNullableType() {
            // Arrange
            var options = new JsonSerializerOptions();
            options.Converters.Add(new TestStringConverter());

            // Act
            var serival = JsonSerializer.Serialize<string>(default, options);

            // Assert
            Assert.AreEqual("null", serival);

        }

        [TestMethod]
        [DataRow("null")]
        [DataRow("\"\"")]
        // Assert
        [ExpectedException(typeof(SerializationException))]
        public void ShouldThrowWithNullValueOnNonNullTypeWhenDeserializing(string nullvalues) {
            // Arrange
            var options = new JsonSerializerOptions();
            options.Converters.Add(new TestIntConverter());

            // Act
            JsonSerializer.Deserialize<int>(nullvalues, options);

        }

        [TestMethod]
        public void ShouldDeserializeNonNullValueForNonNullableType() {

            // Arrange
            var options = new JsonSerializerOptions();
            options.Converters.Add(new TestStringConverter());

            // Act
            var val = JsonSerializer.Deserialize<int>("5", options);

            // Assert
            Assert.AreEqual(5, val);

        }

        [TestMethod]
        public void ShouldReturnTrueForCanConvertWhenTypeIsValid() {

            // Arrange
            var converter = new TestIntConverter();

            // Act
            var canconvert = converter.CanConvert(typeof(int));

            // Assert
            Assert.IsTrue(canconvert);
        }

        [TestMethod]
        [DataRow(typeof(int?))]
        [DataRow(typeof(uint))]
        [DataRow(typeof(Int16))]
        [DataRow(typeof(long))]
        public void ShouldReturnFalseForCanConvertWhenTypeIsInvalid(Type type) {
            // Arrange
            var converter = new TestIntConverter();

            // Act
            var canconvert = converter.CanConvert(type);

            // Assert
            Assert.IsFalse(canconvert);

        }

        [TestMethod]
        public void ShouldReturnTrueForCanConvertForInheritedType() {
            // Arrange
            var converter = new TestInheritanceConverter();

            // Act
            var canconvert = converter.CanConvert(typeof(MemoryStream));
            // Assert
            Assert.IsTrue(canconvert);

        }

        [TestMethod]
        public void ShouldWriteNullValue() {
            // Arrange
            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);
            var converter = new TestInheritanceConverter();

            // Act
            converter.Write(writer, null, new JsonSerializerOptions());
            writer.Flush();

            string json = Encoding.UTF8.GetString(stream.ToArray());

            Assert.AreEqual(json, "null");
        }
    }
}