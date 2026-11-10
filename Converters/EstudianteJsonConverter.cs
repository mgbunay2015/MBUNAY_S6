using System.Text.Json;
using System.Text.Json.Serialization;
using MBUNAY_S6.Modelos;

namespace MBUNAY_S6.Converters;

/// <summary>
/// Convierte el JSON del Web Service PHP a <see cref="Estudiante"/>.
/// El API envía <c>id</c> o <c>codigo</c> como clave primaria (CODIGO en MySQL).
/// </summary>
public sealed class EstudianteJsonConverter : JsonConverter<Estudiante>
{
    public override Estudiante? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Se esperaba un objeto JSON de estudiante.");

        int codigo = 0;
        int edad = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            var propiedad = reader.GetString();
            reader.Read();

            switch (propiedad)
            {
                case "id":
                case "codigo":
                case "CODIGO":
                    codigo = LeerEntero(ref reader);
                    break;
                case "nombre":
                case "NOMBRE":
                    nombre = reader.GetString() ?? string.Empty;
                    break;
                case "edad":
                case "EDAD":
                    edad = LeerEntero(ref reader);
                    break;
                case "apellido":
                case "APELLIDO":
                    apellido = reader.GetString() ?? string.Empty;
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        return new Estudiante
        {
            Codigo = codigo,
            Nombre = nombre,
            Edad = edad,
            Apellido = apellido
        };
    }

    public override void Write(Utf8JsonWriter writer, Estudiante value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.Codigo > 0)
            writer.WriteNumber("id", value.Codigo);

        writer.WriteString("nombre", value.Nombre.Trim());
        writer.WriteNumber("edad", value.Edad);
        writer.WriteString("apellido", value.Apellido.Trim());

        writer.WriteEndObject();
    }

    private static int LeerEntero(ref Utf8JsonReader reader)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.TryGetInt32(out var n) ? n : 0,
            JsonTokenType.String => int.TryParse(reader.GetString(), out var s) ? s : 0,
            _ => 0
        };
    }
}
