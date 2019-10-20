namespace System.Text.Json.NodaTime {

    using global::NodaTime;
    using global::NodaTime.Text;
    using System;
    using System.Text.Json;

    /// <summary>
    /// System.Text.Json converter for <see cref="Interval"/>.
    /// </summary>   
    internal sealed class IsoIntervalConverter : ConverterBase<Interval> {

        protected override Interval ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options) {
            if (reader.TokenType != JsonTokenType.String) {
                throw new ArgumentException(
                    $"Unexpected token parsing Interval. Expected String, got {reader.TokenType}.");
            }
            string text = reader.GetString();
            int slash = text.IndexOf('/');
            if (slash == -1) {
                throw new ArgumentException("Expected ISO-8601-formatted interval; slash was missing.");
            }

            string startText = text.Substring(0, slash);
            string endText = text.Substring(slash + 1);
            var pattern = InstantPattern.ExtendedIso;
            var start = string.IsNullOrEmpty(startText) ? (Instant?)null : pattern.Parse(startText).Value;
            var end = string.IsNullOrEmpty(endText) ? (Instant?)null : pattern.Parse(endText).Value;

            return new Interval(start, end);
        }

        protected override void WriteJsonImpl(Utf8JsonWriter writer, Interval value, JsonSerializerOptions options) {
            var pattern = InstantPattern.ExtendedIso;
            string text = (value.HasStart ? pattern.Format(value.Start) : "") + "/" + (value.HasEnd ? pattern.Format(value.End) : "");
            writer.WriteStringValue(text);
        }
    }
}