namespace System.Text.Json.NodaTime {

    using global::NodaTime;
    using System.Text.Json;

    /// <summary>
    /// System.Text.Json converter for <see cref="Interval"/> using a compound representation. The start and
    /// end aspects of the interval are represented with separate properties, each parsed and formatted
    /// by the <see cref="Instant"/> converter for the serializer provided.
    /// </summary>   
    internal sealed class IntervalConverter : ConverterBase<Interval> {

        protected override Interval ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options) {
            Instant? startInstant = null;
            Instant? endInstant = null;
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
                    startInstant = JsonSerializer.Deserialize<Instant>(ref reader, options);
                }

                var endPropertyName = options.PropertyNamingPolicy?.ConvertName(nameof(Interval.End)) ?? "End";
                if (propertyName == endPropertyName) {
                    endInstant = JsonSerializer.Deserialize<Instant>(ref reader, options);
                }
            }

            return new Interval(startInstant, endInstant);
        }

        protected override void WriteJsonImpl(Utf8JsonWriter writer, Interval value, JsonSerializerOptions options) {
            writer.WriteStartObject();

            if (value.HasStart) {

                var startPropertyName = options.PropertyNamingPolicy?.ConvertName(nameof(Interval.Start)) ?? "Start";
                writer.WritePropertyName(startPropertyName);
                JsonSerializer.Serialize<Instant>(writer, value.Start, options);
            }
            if (value.HasEnd) {
                var endPropertyName = options.PropertyNamingPolicy?.ConvertName(nameof(Interval.End)) ?? "End";
                writer.WritePropertyName(endPropertyName);
                JsonSerializer.Serialize<Instant>(writer, value.End, options);
            }
            writer.WriteEndObject();
        }
    }
}