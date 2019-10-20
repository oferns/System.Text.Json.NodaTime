# Text.Json.NodaTime

## JSON Converters for NodaTime &amp; .NET Standard 2.1 + System.Text.Json.
This is a library that facilitates serializing and deserializing the types provided by [NodaTime](https://nodatime.org/) 
using the new [System.Text.Json](), with no dependency on Newtonsoft.Json. It is a replacement for the [NodaTime.Serialization](https://www.nuget.org/packages/NodaTime.Serialization.JsonNet/) package
for apps that can consume .NET Standard 2.1 compatible libraries, such as ASP.NET core v3+.

### Usage


#### Quick start

Install the nuget package   
`Install-Package System.Text.Json.NodaTime`

Add the using directive   
`using System.Text.Json.NodaTime`

Create an options object   
`JsonSerializerOptions options = new JsonSerializerOptions();`

Add the default NodaTime converters   
`options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);`
            
Use the options in the (static) JsonSerializer class and serialize a NodaTime type (Instant)   
`Instant instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);`

`string serializedInstantJson = JsonSerializer.Serialize(instant, options);`

and then deserialize it back to an Instant   
`Instant deserializedInstant = JsonSerializer.Deserialize<Instant>(serializedInstantJson, options);`

See *Supported types and default representations* for all the types that are supported using this default configuration.


#### Extension Helpers

The package tries to stay as close to the Newtonsoft.Json version as much as possible. Previously, converters could be added to the JsonConvertSettings object or directly through the constructor of the JsonSerializer object. 
This is no longer the case in the System.Text.Json implementation. All converters are now added to an instance of JsonSerializerOptions and passed to the static JsonSerializer struct. Therefore there is only one implementation 
of the helper methods previously available, and they are extensions of the new JsonSerializerOptions object.

`ConfigureForNodaTime(IDateTimeZoneProvider provider)` - adds the default converters for NodaTime types.

`WithIsoIntervalConverter`() - Replaces the default Interval converter with the ISO one.

`WithIsoDateIntervalConverter()` - Replaces the default Interval converter with the ISO one.

`NodaConverters`   - A static helper class giving access to an instance of all the converters. Useful if only adding specific converters and when using the JsonConverterAttribute (see DelegatingConverterBaseTests for an example of that)

*Some Converters rely on others*
For example, in order to serialize and deserialize the Interval type, both the Interval and the Instant converters must be added (as below).

`options.Converters.Add(NodaConverters.IntervalConverter);`   
 `options.Converters.Add(NodaConverters.InstantConverter);`

#### NodaPatternConverter\<T> class

A sealed public class is available to convert any Noda type that can be represented by an IPattern<T> implementation. This is useful for serializing to 
and deserializing from custom formats.

```
var customLocalDateConverter = new NodaPatternConverter<LocalDate>(LocalDatePattern.CreateWithInvariantCulture("d MMMM yyyy")); // My Custom Format   
options.Converters.Add(customLocalDateConverter);

```

#### DelegatingConverterBase\<T> 

An abstract public class is available to inherit from which delegates the conversion to another converter. This is useful with the JsonConverterAttribute.

```
public class CustomDateConverter : DelegatingConverterBase<LocalDate> {
    
    private static readonly JsonConverter<LocalDate> converter =
        new NodaPatternConverter<LocalDate>(LocalDatePattern.CreateWithInvariantCulture("d MMMM yyyy")); // My Custom Format

    public CustomDateConverter() : base(converter) {

           }
        }
```

This class can then be used with the JsonConverterAttribute

```
public class SomeModel {

    [JsonConverter(typeof(CustomDateConverter))]
    public LocalDate LongDate { get; set; }
}
```


#### Supported types and default representations

All default patterns use the invariant culture.

- **Instant**: an ISO-8601 pattern extended to handle fractional seconds: `yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFF'Z'`
- **LocalDate**: ISO-8601 date pattern: `yyyy'-'MM'-'dd`
- **LocalTime**: ISO-8601 time pattern, extended to handle fractional seconds: `HH':'mm':'ss.FFFFFFF`
- **LocalDateTime**: ISO-8601 date/time pattern with no time zone specifier, extended to handle fractional seconds: `yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFF`
- **OffsetDateTime**: ISO-8601 date/time with offset pattern: `yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFo<G>`
- **ZonedDateTime**: As OffsetDateTime, but with a time zone ID at the end: `yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFo<G> z`
- **Interval**: A compound object of the form `{ Start: xxx, End: yyy }` where xxx and yyy are represented however the serializer sees fit. (Typically using the default representation above.)
- **Offset**: general pattern, e.g. `+05` or `-03:30`
- **Period**: The round-trip period pattern; NodaConverters.NormalizingIsoPeriodConverter provides a converter using the normalizing ISO-like pattern
- **Duration**: A duration pattern of `-H:mm:ss.FFFFFFF` (like the standard round-trip pattern, but starting with hours instead of days)
- **DateTimeZone**: The ID, as a string

#### Limitations
Currently only ISO calendars are supported, and handling for negative and non-four-digit years will depend on the appropriate underlying pattern implementation. (Non-ISO converters are now possible, but the results would be very specific to Noda Time.)
There's no indication of the time zone provider or its version in the DateTimeZone representation.