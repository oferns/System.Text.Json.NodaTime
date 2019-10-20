namespace System.Text.Json.NodaTime.Tests {
    using global::NodaTime;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Text.Json;

    [TestClass]
    public class NodaConvertersTests {

        [TestClass]
        public class OffsetConverterTests {

            [TestMethod]
            public void ShouldSerializeAndDeserialize() {
                // Arrange
                var value = Offset.FromHoursAndMinutes(5, 30);
                string json = "\"+05:30\"";

                var options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.OffsetConverter);

                // Act
                var seri = JsonSerializer.Serialize(value, options);
                var deseri = JsonSerializer.Deserialize<Offset>(seri, options);

                // Assert
                Assert.AreEqual(json, seri);
                Assert.AreEqual(value, deseri);
            }
        }

        [TestClass]
        public class InstantConverterTests {

            [TestMethod]
            public void ShouldSerializeAndDeserialize() {
                // Arrange
                var value = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
                string json = "\"2012-01-02T03:04:05Z\"";

                var options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.InstantConverter);

                // Act
                var seri = JsonSerializer.Serialize(value, options);
                var deseri = JsonSerializer.Deserialize<Instant>(seri, options);

                // Assert
                Assert.AreEqual(json, seri);
                Assert.AreEqual(value, deseri);
            }
        }

        [TestClass]
        public class LocalDateConverterTests {
            
            [TestMethod]
            public void ShouldSerializeAndDeserialize() {
                // Arrange
                var value = new LocalDate(2012, 1, 2, CalendarSystem.Iso);
                string json = "\"2012-01-02\"";

                var options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.LocalDateConverter);

                // Act
                var seri = JsonSerializer.Serialize(value, options);
                var deseri = JsonSerializer.Deserialize<LocalDate>(seri, options);

                // Assert
                Assert.AreEqual(json, seri);
                Assert.AreEqual(value, deseri);
            }
        }

        [TestClass]
        public class LocalDateTimeConverterTests {

            [TestMethod]
            public void ShouldSerializeAndDeserialize() {
                // Arrange
                var value = new LocalDateTime(2012, 1, 2, 3, 4, 5, CalendarSystem.Iso).PlusNanoseconds(123456789);
                var json = "\"2012-01-02T03:04:05.123456789\"";

                var options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.LocalDateTimeConverter);

                // Act
                var seri = JsonSerializer.Serialize(value, options);
                var deseri = JsonSerializer.Deserialize<LocalDateTime>(seri, options);

                // Assert
                Assert.AreEqual(json, seri);
                Assert.AreEqual(value, deseri);
            }

            [TestMethod]
            // Assert
            [ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnNonIsoCalenderSystem() {
                // Arrange
                var localDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, CalendarSystem.Coptic);
                
                var options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.LocalDateTimeConverter);
                
                // Act
                JsonSerializer.Serialize(localDateTime, options);
            }
        }

        [TestClass]
        public class LocalTimeConverterTests {

            [TestMethod]
            public void ShouldSerializeAndDeserialize() {           
                // Arrange
                var value = LocalTime.FromHourMinuteSecondMillisecondTick(1, 2, 3, 4, 5).PlusNanoseconds(67);
                var json = "\"01:02:03.004000567\"";

                var options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.LocalTimeConverter);

                // Act
                var seri = JsonSerializer.Serialize(value, options);
                var deseri = JsonSerializer.Deserialize<LocalTime>(seri, options);

                // Assert
                Assert.AreEqual(json, seri);
                Assert.AreEqual(value, deseri);
            }
        }

        [TestClass]
        public class PeriodConverterTests {

            [TestMethod]
            public void ShouldSerializeRoundTrip() {
                // Arrange
                var value = Period.FromDays(2) + Period.FromHours(3) + Period.FromMinutes(90);
                string json = "\"P2DT3H90M\"";

                var options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.RoundtripPeriodConverter);

                // Act
                var seri = JsonSerializer.Serialize(value, options);
                var deseri = JsonSerializer.Deserialize<Period>(seri, options);

                // Assert
                Assert.AreEqual(json, seri);
                Assert.AreEqual(value, deseri);
            }

            [TestMethod]
            public void ShouldSerializeNormalizingIsoWithNormalization() {
                // Arrange
                var options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.NormalizingIsoPeriodConverter);

                string expectedJson = "\"P2DT4H30M\"";

                var period = Period.FromDays(2) + Period.FromHours(3) + Period.FromMinutes(90);
                
                // Act
                var json = JsonSerializer.Serialize(period, options);

                // Assert
                Assert.AreEqual(expectedJson, json);
            }

            [TestMethod]
            public void ShouldSerializeNormalizingIsoWithAlreadyNormalized() {
                // Arrange
                var value = Period.FromDays(2) + Period.FromHours(4) + Period.FromMinutes(30);
                string json = "\"P2DT4H30M\"";

                var options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.NormalizingIsoPeriodConverter);

                // Act
                var seri = JsonSerializer.Serialize(value, options);
                var deseri = JsonSerializer.Deserialize<Period>(seri, options);

                // Assert
                Assert.AreEqual(json, seri);
                Assert.AreEqual(value, deseri);
            }
        }

        [TestClass]
        public class ZonedDateTimeConverterTests {

            [TestMethod]
            public void ShouldSerializeAndDeserialize() {
                // Arrange
                // Deliberately give it an ambiguous local time, in both ways.
                var zone = DateTimeZoneProviders.Tzdb["Europe/London"];
                var earlierValue = new ZonedDateTime(new LocalDateTime(2012, 10, 28, 1, 30), zone, Offset.FromHours(1));
                var laterValue = new ZonedDateTime(new LocalDateTime(2012, 10, 28, 1, 30), zone, Offset.FromHours(0));

                string earlierJson = "\"2012-10-28T01:30:00+01 Europe/London\"";
                string laterJson = "\"2012-10-28T01:30:00Z Europe/London\"";

                var options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.CreateZonedDateTimeConverter(DateTimeZoneProviders.Tzdb));

                // Act
                var earlyseri = JsonSerializer.Serialize(earlierValue, options);
                var laterseri = JsonSerializer.Serialize(laterValue, options);

                var earlydeseri = JsonSerializer.Deserialize<ZonedDateTime>(earlyseri, options);
                var laterdeseri = JsonSerializer.Deserialize<ZonedDateTime>(laterseri, options);

                // Assert
                Assert.AreEqual(earlierValue, earlydeseri);
                Assert.AreEqual(laterValue, laterdeseri);

                Assert.AreEqual(earlierJson, earlyseri);
                Assert.AreEqual(laterJson, laterseri);
            }
        }

        [TestClass]
        public class OffsetDateTimeConverterTests {

            private readonly JsonSerializerOptions options;
            public OffsetDateTimeConverterTests() {
                options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.OffsetDateTimeConverter);
            }

            [TestMethod]
            public void ShouldSerializeAndDeserilize() {
                // Arrange
                var value = new LocalDateTime(2012, 1, 2, 3, 4, 5).PlusNanoseconds(123456789).WithOffset(Offset.FromHoursAndMinutes(-1, -30));
                string json = "\"2012-01-02T03:04:05.123456789-01:30\"";

                // Act
                var seri = JsonSerializer.Serialize(value, options);
                var deseri = JsonSerializer.Deserialize<OffsetDateTime>(seri, options);

                // Assert
                Assert.AreEqual(json, seri);
                Assert.AreEqual(value, deseri);
            }

            [TestMethod]
            public void ShouldSerializeAndDeserilizeWholeHours() {
                // Arrange
                var value = new LocalDateTime(2012, 1, 2, 3, 4, 5).PlusNanoseconds(123456789).WithOffset(Offset.FromHours(5));
                string json = "\"2012-01-02T03:04:05.123456789+05:00\""; 
                
                // Act
                var seri = JsonSerializer.Serialize<OffsetDateTime>(value, options);
                var deseri = JsonSerializer.Deserialize<OffsetDateTime>(seri, options);

                // Assert
                Assert.AreEqual(json, seri);
                Assert.AreEqual(value, deseri);
            }

            [TestMethod]
            public void ShouldSerializeAndDeserilizeZeroOffset() {
                // Arrange
                // Redundantly specify the minutes, so that Javascript can parse it and it's RFC3339-compliant.
                // See issue 284 for details.
                var value = new LocalDateTime(2012, 1, 2, 3, 4, 5).PlusNanoseconds(123456789).WithOffset(Offset.Zero);
                string json = "\"2012-01-02T03:04:05.123456789Z\"";

                // Act
                var seri = JsonSerializer.Serialize(value, options);
                var deseri = JsonSerializer.Deserialize<OffsetDateTime>(seri, options);

                // Assert
                Assert.AreEqual(json, seri);
                Assert.AreEqual(value, deseri);
            }
        }

        [TestClass]
        public class DurationeConverterTests {

            private readonly JsonSerializerOptions options;
            
            public DurationeConverterTests() {
                options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.DurationConverter);
            }

            [TestMethod]
            public void ShouldSerializeAndDeserializeWholeSeconds() {
               // Arrange
                var duration = Duration.FromHours(48);
                var json = "\"48:00:00\"";

                // Act
                var seri = JsonSerializer.Serialize(duration, options);
                var deseri = JsonSerializer.Deserialize<Duration>(seri, options);

                // Assert
                Assert.AreEqual(json, seri);
                Assert.AreEqual(duration, deseri);
            }

            [TestMethod]
            public void ShouldSerializeAndDeserializeFractionalSeconds() {
                // Arrange
                var dur1 = Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromNanoseconds(123456789);
                var dur1json = "\"48:00:03.123456789\"";

                var dur2 = Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromTicks(1230000);
                var dur2json = "\"48:00:03.123\"";

                var dur3 = Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromTicks(1234000);
                var dur3json = "\"48:00:03.1234\"";


                var dur4 = Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromTicks(12345);
                var dur4json = "\"48:00:03.0012345\"";

                // Act
                var dur1seri = JsonSerializer.Serialize(dur1, options);
                var dur2seri = JsonSerializer.Serialize(dur2, options);
                var dur3seri = JsonSerializer.Serialize(dur3, options);
                var dur4seri = JsonSerializer.Serialize(dur4, options);

                var dur1unseri = JsonSerializer.Deserialize<Duration>(dur1seri, options);
                var dur2unseri = JsonSerializer.Deserialize<Duration>(dur2seri, options);
                var dur3unseri = JsonSerializer.Deserialize<Duration>(dur3seri, options);
                var dur4unseri = JsonSerializer.Deserialize<Duration>(dur4seri, options);

                // Assert
                Assert.AreEqual(dur1, dur1unseri);
                Assert.AreEqual(dur2, dur2unseri);
                Assert.AreEqual(dur3, dur3unseri);
                Assert.AreEqual(dur4, dur4unseri);

                Assert.AreEqual(dur1json, dur1seri);
                Assert.AreEqual(dur2json, dur2seri);
                Assert.AreEqual(dur3json, dur3seri);
                Assert.AreEqual(dur4json, dur4seri);
            }

            [TestMethod]
            public void ShouldSerializeAndDeserializeMinMax() {
               // Arrange
                var minduration = Duration.FromTicks(long.MinValue);
                var maxduration = Duration.FromTicks(long.MaxValue);

                string minjson = "\"-256204778:48:05.4775808\"";
                string maxjson = "\"256204778:48:05.4775807\"";

                // Act
                var minseri = JsonSerializer.Serialize(minduration, options);
                var maxseri = JsonSerializer.Serialize(maxduration, options);

                var min = JsonSerializer.Deserialize<Duration>(minseri, options);
                var max = JsonSerializer.Deserialize<Duration>(maxseri, options);

                // Assert
                Assert.AreEqual(minduration, min);
                Assert.AreEqual(maxduration, max);
                Assert.AreEqual(minjson, minseri);
                Assert.AreEqual(maxjson, maxseri);
            }

            [TestMethod]
            public void ShouldDeserializePartialFractionalSecondsWithTrailingZeroes() {
                // Arrange
                string json = "\"25:10:00.1234000\"";
                var expectedDuration = Duration.FromHours(25) + Duration.FromMinutes(10) + Duration.FromTicks(1234000);
                
                // Act
                var parsed = JsonSerializer.Deserialize<Duration>(json, options);
                
                // Assert
                Assert.AreEqual(expectedDuration, parsed);
            }
        }

        [TestClass]
        public class OffsetDateConverterTests {
            
            private readonly JsonSerializerOptions options;
           
            public OffsetDateConverterTests() {
                options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.OffsetDateConverter);
            }

            [TestMethod]
            public void ShouldSerializeAndDeserialize() {
                // Arrange
                var value = new LocalDate(2012, 1, 2).WithOffset(Offset.FromHoursAndMinutes(-1, -30));
                string json = "\"2012-01-02-01:30\"";

                // Act
                var seri = JsonSerializer.Serialize(value, options);
                var deseri = JsonSerializer.Deserialize<OffsetDate>(seri, options);

                // Assert
                Assert.AreEqual(json, seri);
                Assert.AreEqual(value, deseri);
            }
        }

        [TestClass]
        public class OffsetTimeConverterTests {

            private readonly JsonSerializerOptions options;
            public OffsetTimeConverterTests() {
                options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.OffsetTimeConverter);
            }

            [TestMethod]
            public void ShouldSerializeAndDeserialize() {
                // Arrange
                var value = new LocalTime(3, 4, 5).PlusNanoseconds(123456789).WithOffset(Offset.FromHoursAndMinutes(-1, -30));
                string json = "\"03:04:05.123456789-01:30\"";

                // Act
                var seri = JsonSerializer.Serialize(value, options);
                var deseri = JsonSerializer.Deserialize<OffsetTime>(seri, options);

                // Assert
                Assert.AreEqual(json, seri);
                Assert.AreEqual(value, deseri);
            }
        }

        [TestClass]
        public class PatternConverterTests {

            [TestMethod]
            // Assert
            [ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowOnNullPattern() {
                new NodaPatternConverter<LocalDate>(null);
            }

            [TestMethod]
            // Assert
            [ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnReadNonStringValue() {
                // Arrange
                string json = "5";

                var options = new JsonSerializerOptions();
                options.Converters.Add(NodaConverters.LocalDateConverter);

                // Act
                JsonSerializer.Deserialize<LocalDate>(json, options);
            }
        }
    }
}