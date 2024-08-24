#if JSON_ENABLED
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Vector3Converter : JsonConverter<Vector3>
{
    public override Vector3 ReadJson(
        JsonReader reader,
        Type objectType,
        Vector3 existingValue,
        bool hasExistingValue,
        JsonSerializer serializer
    )
    {
        JObject obj = JObject.Load(reader);

        return new Vector3(
            obj["x"].ToObject<float>(),
            obj["y"].ToObject<float>(),
            obj["z"].ToObject<float>()
        );
    }

    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(nameof(value.x));
        writer.WriteValue(value.x);
        writer.WritePropertyName(nameof(value.y));
        writer.WriteValue(value.y);
        writer.WritePropertyName(nameof(value.z));
        writer.WriteValue(value.z);
        writer.WriteEndObject();
    }
}
#endif
