namespace System.Text.Json.NodaTime {

    using global::NodaTime;
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Static class containing extension methods to configure System.Text.Json for Noda Time types.
    /// </summary>
    public static class Extensions {

        /// <summary>
        /// Configures System.Text.Json with everything required to properly serialize and deserialize NodaTime data types.
        /// </summary>
        /// <param name="options">The existing settings to add Noda Time converters to.</param>
        /// <param name="provider">The time zone provider to use when parsing time zones and zoned date/times.</param>
        /// <returns>The original <paramref name="options"/> value, for further chaining.</returns>
        public static JsonSerializerOptions ConfigureForNodaTime(this JsonSerializerOptions options, IDateTimeZoneProvider provider) {

            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }
            if (provider == null) {
                throw new ArgumentNullException(nameof(provider));
            }
            
            // Add our converters
            AddDefaultConverters(options.Converters, provider);
            
            return options;
        }

        /// <summary>
        /// Configures the given serializer settings to use <see cref="NodaConverters.IsoIntervalConverter"/>.
        /// Any other converters which can convert <see cref="Interval"/> are removed from the serializer.
        /// </summary>
        /// <param name="options">The existing serializer settings to add Noda Time converters to.</param>
        /// <returns>The original <paramref name="options"/> value, for further chaining.</returns>
        public static JsonSerializerOptions WithIsoIntervalConverter(this JsonSerializerOptions options) {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }
            ReplaceExistingConverters<Interval>(options.Converters, NodaConverters.IsoIntervalConverter);
            return options;
        }

        /// <summary>
        /// Configures the given serializer settings to use <see cref="NodaConverters.IsoDateIntervalConverter"/>.
        /// Any other converters which can convert <see cref="DateInterval"/> are removed from the serializer.
        /// </summary>
        /// <param name="options">The existing serializer settings to add Noda Time converters to.</param>
        /// <returns>The original <paramref name="options"/> value, for further chaining.</returns>
        public static JsonSerializerOptions WithIsoDateIntervalConverter(this JsonSerializerOptions options) {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }
            ReplaceExistingConverters<DateInterval>(options.Converters, NodaConverters.IsoDateIntervalConverter);
            return options;
        }

        private static void ReplaceExistingConverters<T>(IList<JsonConverter> converters, JsonConverter newConverter) {
            for (int i = converters.Count - 1; i >= 0; i--) {
                if (converters[i].CanConvert(typeof(T))) {
                    converters.RemoveAt(i);
                }
            }
            converters.Add(newConverter);
        }

        private static void AddDefaultConverters(IList<JsonConverter> converters, IDateTimeZoneProvider provider) {
            converters.Add(NodaConverters.InstantConverter);
            converters.Add(NodaConverters.IntervalConverter);
            converters.Add(NodaConverters.LocalDateConverter);
            converters.Add(NodaConverters.LocalDateTimeConverter);
            converters.Add(NodaConverters.LocalTimeConverter);
            converters.Add(NodaConverters.DateIntervalConverter);
            converters.Add(NodaConverters.OffsetConverter);
            converters.Add(NodaConverters.CreateDateTimeZoneConverter(provider));
            converters.Add(NodaConverters.DurationConverter);
            converters.Add(NodaConverters.RoundtripPeriodConverter);
            converters.Add(NodaConverters.OffsetDateTimeConverter);
            converters.Add(NodaConverters.OffsetDateConverter);
            converters.Add(NodaConverters.OffsetTimeConverter);
            converters.Add(NodaConverters.CreateZonedDateTimeConverter(provider));
        }
    }
}