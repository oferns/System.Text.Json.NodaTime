namespace System.Text.Json.NodaTime {

    using global::NodaTime.Text;
    using System;
    using System.Text.Encodings.Web;
    using System.Text.Json;

    /// <summary>
    /// A JSON converter for types which can be represented by a single string value, parsed or formatted
    /// from an <see cref="IPattern{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to convert to/from JSON.</typeparam>
    public sealed class NodaPatternConverter<T> : ConverterBase<T> {

        private readonly IPattern<T> pattern;
        private readonly Action<T> validator;

        /// <summary>
        /// Creates a new instance with a pattern and no validator.
        /// </summary>
        /// <param name="pattern">The pattern to use for parsing and formatting.</param>
        /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is null.</exception>
        public NodaPatternConverter(IPattern<T> pattern) : this(pattern, null) {

        }

        /// <summary>
        /// Creates a new instance with a pattern and an optional validator. The validator will be called before each
        /// value is written, and may throw an exception to indicate that the value cannot be serialized.
        /// </summary>
        /// <param name="pattern">The pattern to use for parsing and formatting.</param>
        /// <param name="validator">The validator to call before writing values. May be null, indicating that no validation is required.</param>
        /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is null.</exception>
        public NodaPatternConverter(IPattern<T> pattern, Action<T> validator) {

            this.pattern = pattern ?? throw new ArgumentNullException(nameof(pattern), "Pattern cannot be null");
            this.validator = validator;
        }

        protected override T ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options) {
            if (reader.TokenType != JsonTokenType.String) {
                throw new ArgumentException(
                    $"Unexpected token parsing {typeof(T).Name}. Expected String, got {reader.TokenType}.");
            }
            string text = reader.GetString();
            return pattern.Parse(text).Value;
        }

        protected override void WriteJsonImpl(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
            validator?.Invoke(value);
            var frm = pattern.Format(value);
            var txt = JsonEncodedText.Encode(frm, JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
            writer.WriteStringValue(txt);
        }
    }
}
