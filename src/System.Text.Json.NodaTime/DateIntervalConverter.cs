namespace System.Text.Json.NodaTime {

    using global::NodaTime;
    using System;
    using System.Text.Json;

    /// <summary>
    /// System.Text.Json converter for <see cref="DateInterval"/> using a compound representation. The start and
    /// end aspects of the date interval are represented with separate properties, each parsed and formatted
    /// by the <see cref="LocalDate"/> converter for the serializer provided.
    /// </summary>   
    internal sealed class DateIntervalConverter : ConverterBase<DateInterval> {

        protected override DateInterval ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options) {
            LocalDate? startLocalDate = null;
            LocalDate? endLocalDate = null;
            while (reader.Read()) {
                if (reader.TokenType != JsonTokenType.PropertyName) {
                    break;
                }

                var propertyName = reader.GetString();
                
                if (!reader.Read()) {
                    break;
                }

                var startPropertyName = options.PropertyNamingPolicy?.ConvertName(nameof(Interval.Start)) ?? "Start";
                if (propertyName == startPropertyName) {
                    startLocalDate = JsonSerializer.Deserialize<LocalDate?>(ref reader, options);
                }

                var endPropertyName = options.PropertyNamingPolicy?.ConvertName(nameof(Interval.End)) ?? "End";
                if (propertyName == endPropertyName) {
                    endLocalDate = JsonSerializer.Deserialize<LocalDate?>(ref reader, options);
                }
            }

            if (!startLocalDate.HasValue) {
                throw new ArgumentException("Expected date interval; start date was missing.");
            }

            if (!endLocalDate.HasValue) {
                throw new ArgumentException("Expected date interval; end date was missing.");
            }

            return new DateInterval(startLocalDate.Value, endLocalDate.Value);
        }

        protected override void WriteJsonImpl(Utf8JsonWriter writer, DateInterval value, JsonSerializerOptions options) {
            writer.WriteStartObject();

            var startPropertyName = options.PropertyNamingPolicy?.ConvertName(nameof(Interval.Start)) ?? "Start";
            writer.WritePropertyName(startPropertyName);
            JsonSerializer.Serialize(writer, value.Start, options);

            var endPropertyName = options.PropertyNamingPolicy?.ConvertName(nameof(Interval.End)) ?? "End";
            writer.WritePropertyName(endPropertyName);
            JsonSerializer.Serialize(writer, value.End, options);

            writer.WriteEndObject();
        }
    }
}