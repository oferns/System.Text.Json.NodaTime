namespace System.Text.Json.NodaTime {
   
    using global::NodaTime;
    using global::NodaTime.Text;
    using System;
    using System.Text.Json;

    /// <summary>
    /// System.Text.Json converter for <see cref="DateInterval"/>.
    /// </summary>   
    internal sealed class IsoDateIntervalConverter : ConverterBase<DateInterval> {
        protected override DateInterval ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options) {
            if (reader.TokenType != JsonTokenType.String) {
                throw new ArgumentException(
                    $"Unexpected token parsing DateInterval. Expected String, got {reader.TokenType}.");
            }
            string text = reader.GetString();
            int slash = text.IndexOf('/');
            if (slash == -1) {
                throw new ArgumentException("Expected ISO-8601-formatted date interval; slash was missing.");
            }

            string startText = text.Substring(0, slash);
            if (string.IsNullOrEmpty(startText)) {
                throw new ArgumentException("Expected ISO-8601-formatted date interval; start date was missing.");
            }

            string endText = text.Substring(slash + 1);
            if (string.IsNullOrEmpty(endText)) {
                throw new ArgumentException("Expected ISO-8601-formatted date interval; end date was missing.");
            }

            var pattern = LocalDatePattern.Iso;
            var start = pattern.Parse(startText).Value;
            var end = pattern.Parse(endText).Value;

            return new DateInterval(start, end);
        }

        protected override void WriteJsonImpl(Utf8JsonWriter writer, DateInterval value, JsonSerializerOptions options) {
            var pattern = LocalDatePattern.Iso;
            string text = pattern.Format(value.Start) + "/" + pattern.Format(value.End);
            writer.WriteStringValue(text);
        }
    }
}