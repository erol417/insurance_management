using System.Text.Json;
using System.Text.Json.Serialization;

namespace InsuranceManagement.Web.Infrastructure.Json;

public sealed class NullableEnumFromEmptyStringConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        var underlyingType = Nullable.GetUnderlyingType(typeToConvert);
        return underlyingType?.IsEnum == true;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var enumType = Nullable.GetUnderlyingType(typeToConvert)!;
        var converterType = typeof(NullableEnumFromEmptyStringConverter<>).MakeGenericType(enumType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private sealed class NullableEnumFromEmptyStringConverter<TEnum> : JsonConverter<TEnum?> where TEnum : struct, Enum
    {
        public override TEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                if (Enum.TryParse<TEnum>(value, true, out var parsed))
                {
                    return parsed;
                }
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                var number = reader.GetInt32();
                return (TEnum)Enum.ToObject(typeof(TEnum), number);
            }

            throw new JsonException("Gecersiz enum degeri.");
        }

        public override void Write(Utf8JsonWriter writer, TEnum? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString());
                return;
            }

            writer.WriteNullValue();
        }
    }
}
