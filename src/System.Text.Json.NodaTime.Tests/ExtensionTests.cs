namespace System.Text.Json.NodaTime.Tests {

    using global::NodaTime;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NodaTime;
    using System.Text.Encodings.Web;
    using System.Text.Json;

    [TestClass]
    public class ExtensionTests {

        [TestClass]
        public class ConfigureForNodaTimeTests {

            [TestMethod]
            // Assert
            [ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowOnNullOptions() {
                // Arrange
                var provider = DateTimeZoneProviders.Tzdb;
                
                // Act
                Extensions.ConfigureForNodaTime(null, provider);
            }

            [TestMethod]
            // Assert
            [ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowOnNullProvider() {
                // Arrange
                var options = new JsonSerializerOptions();
                
                // Act
                options.ConfigureForNodaTime(null);
            }
        }

        [TestClass]
        public class WithIsoIntervalConverterTests {

            [TestMethod]
            // Assert
            [ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowOnNullOptions() {
                // Arrange
                // Act
                Extensions.WithIsoIntervalConverter(null);
            }
        }

        [TestClass]
        public class WithIsoDateIntervalConverter {

            [TestMethod]
            // Assert
            [ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowOnNullOptions() {
                // Arrange
                // Act
                Extensions.WithIsoDateIntervalConverter(null);
            }
        }

        [TestMethod]
        public void ShouldConfigureSettingsForDefaultInterval() {

            // Arrange
            var configuredoptions = new JsonSerializerOptions().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var explicitoptions = new JsonSerializerOptions();

            explicitoptions.Converters.Add(NodaConverters.IntervalConverter);
            explicitoptions.Converters.Add(NodaConverters.InstantConverter);

            var interval = new Interval(Instant.FromUnixTimeTicks(1000L), Instant.FromUnixTimeTicks(20000L));

            // Act
            var configuredseri = JsonSerializer.Serialize<Interval>(interval, configuredoptions);
            var explicitseri = JsonSerializer.Serialize<Interval>(interval, explicitoptions);

            // Assert
            Assert.AreEqual(configuredseri, explicitseri);
        }

        [TestMethod]
        public void ShouldConfigureSettingsForIsoInterval() {

            // Arrange
            var configuredoptions = new JsonSerializerOptions().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb).WithIsoIntervalConverter();
            var explicitoptions = new JsonSerializerOptions();

            explicitoptions.Converters.Add(NodaConverters.IsoIntervalConverter);

            var interval = new Interval(Instant.FromUnixTimeTicks(1000L), Instant.FromUnixTimeTicks(20000L));

            // Act
            var configuredseri = JsonSerializer.Serialize<Interval>(interval, configuredoptions);
            var explicitseri = JsonSerializer.Serialize<Interval>(interval, explicitoptions);

            // Assert
            Assert.AreEqual(configuredseri, explicitseri);

        }

        [TestMethod]
        public void ShouldConfigureSettingsForDefaultDateInterval() {

            // Arrange
            var configuredoptions = new JsonSerializerOptions().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var explicitoptions = new JsonSerializerOptions();

            explicitoptions.Converters.Add(NodaConverters.DateIntervalConverter);
            explicitoptions.Converters.Add(NodaConverters.LocalDateConverter);

            var interval = new DateInterval(new LocalDate(2001, 2, 3), new LocalDate(2004, 5, 6));

            // Act
            var configuredseri = JsonSerializer.Serialize<DateInterval>(interval, configuredoptions);
            var explicitseri = JsonSerializer.Serialize<DateInterval>(interval, explicitoptions);

            // Assert
            Assert.AreEqual(configuredseri, explicitseri);
        }

        [TestMethod]
        public void ShouldConfigureSettingsForIsoDateInterval() {

            // Arrange
            var configuredoptions = new JsonSerializerOptions().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb).WithIsoDateIntervalConverter();
            var explicitoptions = new JsonSerializerOptions();

            explicitoptions.Converters.Add(NodaConverters.IsoDateIntervalConverter);

            var interval = new DateInterval(new LocalDate(2001, 2, 3), new LocalDate(2004, 5, 6));

            // Act
            var configuredseri = JsonSerializer.Serialize<DateInterval>(interval, configuredoptions);
            var explicitseri = JsonSerializer.Serialize<DateInterval>(interval, explicitoptions);

            // Assert
            Assert.AreEqual(configuredseri, explicitseri); ;
        }
    }
}