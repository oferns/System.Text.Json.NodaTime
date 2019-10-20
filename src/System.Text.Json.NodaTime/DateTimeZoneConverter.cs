namespace System.Text.Json.NodaTime {

    using global::NodaTime;
    using System;
    using System.Text.Json;

    internal sealed class DateTimeZoneConverter : ConverterBase<DateTimeZone> {

        private readonly IDateTimeZoneProvider provider;

        /// <param name="provider">Provides the <see cref="DateTimeZone"/> that corresponds to each time zone ID in the JSON string.</param>
        public DateTimeZoneConverter(IDateTimeZoneProvider provider) {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        protected override DateTimeZone ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options) {
            if (reader.TokenType != JsonTokenType.String) {
                throw new ArgumentException("Unexpected token parsing instant. Expected JsonTokenType.String.");
            }

            return provider[reader.GetString()];
        }

        protected override void WriteJsonImpl(Utf8JsonWriter writer, DateTimeZone value, JsonSerializerOptions options) {
            writer.WriteStringValue(value.Id);
        }
    }
}