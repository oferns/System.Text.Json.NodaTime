namespace System.Text.Json.NodaTime {
    
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Converter which does nothing but delegate to another one for all operations.
    /// </summary>
    /// <remarks>
    /// Nothing in this class is specific to Noda Time. Its purpose is to make it easy
    /// to reuse other converter instances with <see cref="JsonConverterAttribute"/>,
    /// which can only identify a converter by type.
    /// </remarks>
    /// <example>
    /// <para>
    /// If you had some <see cref="LocalDate"/> properties which needed one converter,
    /// but others that needed another, you might want to have different types implementing
    /// those converters. Each type would just derive from this, passing the right converter
    /// into the base constructor.
    /// </para>
    /// <code>
    /// public sealed class ShortDateConverter<LocalDate> : DelegatingConverterBase<LocalDate>
    /// {
    ///     public ShortDateConverter() : base(NodaConverters.LocalDateConverter) {}
    /// }
    /// </code>
    /// </example>
    public abstract class DelegatingConverterBase<T> : JsonConverter<T> {
        private readonly JsonConverter<T> original;

        /// <summary>
        /// Constructs a converter delegating to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The converter to delegate to. Must not be null.</param>
        protected DelegatingConverterBase(JsonConverter<T> original) {
            this.original = original ?? throw new ArgumentNullException(nameof(original));
        }

        public override bool CanConvert(Type typeToConvert) {
            return original.CanConvert(typeToConvert);
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            return original.Read(ref reader, typeToConvert, options);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
            original.Write(writer, value, options);
        }
    }
}