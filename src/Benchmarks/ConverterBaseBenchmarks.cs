
namespace Benchmarcks {

    using BenchmarkDotNet.Attributes;
    using System.IO;
    using System.Text.Json;
    using System.Text.Json.NodaTime;

    public class ConverterBaseBenchmarks {

        private readonly ConverterBase<int> int32Converter = new DummyConverter<int>();
        private readonly ConverterBase<string> stringConverter = new DummyConverter<string>();
        private readonly ConverterBase<Stream> streamConverter = new DummyConverter<Stream>();

        private class DummyConverter<T> : ConverterBase<T> {

            protected override T ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options) => default;
            protected override void WriteJsonImpl(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
                writer.WriteStringValue(value.ToString());
            }
        }

        // Value types
        [Benchmark]
        public bool CanConvert_Int32_Int32() => int32Converter.CanConvert(typeof(int));

        [Benchmark]
        public bool CanConvert_Int32_NullableInt32() => int32Converter.CanConvert(typeof(int?));

        [Benchmark]
        public bool CanConvert_Int32_Object() => int32Converter.CanConvert(typeof(object));

        [Benchmark]
        public bool CanConvert_Int32_String() => int32Converter.CanConvert(typeof(string));

        [Benchmark]
        public bool CanConvert_Int32_UInt32() => int32Converter.CanConvert(typeof(uint));

        // Sealed classes
        [Benchmark]
        public bool CanConvert_String_String() => stringConverter.CanConvert(typeof(string));

        [Benchmark]
        public bool CanConvert_String_Object() => stringConverter.CanConvert(typeof(object));

        [Benchmark]
        public bool CanConvert_String_UInt32() => stringConverter.CanConvert(typeof(uint));

        // Unsealed classes
        [Benchmark]
        public bool CanConvert_Stream_Stream() => streamConverter.CanConvert(typeof(Stream));

        [Benchmark]
        public bool CanConvert_Stream_MemoryStream() => streamConverter.CanConvert(typeof(MemoryStream));

        [Benchmark]
        public bool CanConvert_Stream_Object() => streamConverter.CanConvert(typeof(object));

        [Benchmark]
        public bool CanConvert_Stream_String() => streamConverter.CanConvert(typeof(string));

        [Benchmark]
        public bool CanConvert_Stream_UInt32() => streamConverter.CanConvert(typeof(uint));



    }
}
