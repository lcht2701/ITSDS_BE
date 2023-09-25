using AutoMapper.Features;
using AutoWrapper.Extensions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Utils;

public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.FromDateTime(reader.GetDateTime());
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        var isoDate = value.ToString("O");
        writer.WriteStringValue(isoDate);
    }
}
