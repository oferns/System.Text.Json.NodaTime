namespace System.Text.Json.NodaTime {

    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public abstract class ConverterBase<T> : JsonConverter<T> {

        protected ConverterBase() { }

        // For value types and sealed classes, we can optimize and not call IsAssignableFrom.
#pragma warning disable RECS0108 // Warns about static fields in generic types
        private static readonly bool CheckAssignableFrom =
            !(typeof(T).GetTypeInfo().IsValueType || (typeof(T).GetTypeInfo().IsClass && typeof(T).GetTypeInfo().IsSealed));

        private static readonly Type NullableT = typeof(T).GetTypeInfo().IsValueType
            ? typeof(Nullable<>).MakeGenericType(typeof(T)) : typeof(T);
#pragma warning restore RECS0108 // Warns about static fields in generic types

        public override bool CanConvert(Type typeToConvert) =>
            typeToConvert == typeof(T) ||
            (CheckAssignableFrom && typeof(T).GetTypeInfo().IsAssignableFrom(typeToConvert.GetTypeInfo()));

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            if (reader.TokenType == JsonTokenType.Null) {
                if (typeToConvert != NullableT) {
                    throw new SerializationException($"Cannot convert value to {typeToConvert.Name}");
                }
                return default;
            }

            if (reader.TokenType == JsonTokenType.String && string.IsNullOrEmpty((string)reader.GetString())) {
                if (typeToConvert != NullableT) {
                    throw new SerializationException($"Cannot convert value to {typeToConvert.Name}");
                }
                return default;
            }

            return ReadJsonImpl(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {

            if (value is null) {
             
                writer.WriteNullValue();
                return;
            }

            if (value is T castValue) {
                WriteJsonImpl(writer, castValue, options);
                return;
            }

            throw new ArgumentException($"Unexpected value when converting. Expected {typeof(T).FullName}, got {value.GetType().FullName}.");
        }

        /// <summary>
        /// Implemented by concrete subclasses, this performs the final conversion from a non-null JSON value to
        /// a value of type T.
        /// </summary>
        /// <param name="reader">The JSON reader to pull data from</param>
        /// <returns>The deserialized value of type T.</returns>
        protected abstract T ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options);

        /// <summary>
        /// Implemented by concrete subclasses, this performs the final write operation for a non-null value of type T
        /// to JSON.
        /// </summary>
        /// <param name="writer">The writer to write JSON data to</param>
        /// <param name="value">The value to serializer</param>
        protected abstract void WriteJsonImpl(Utf8JsonWriter writer, T value, JsonSerializerOptions options);
    }
}