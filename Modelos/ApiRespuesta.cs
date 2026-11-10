using System.Text.Json;
using System.Text.Json.Serialization;

namespace MBUNAY_S6.Modelos;

/// <summary>
/// Sobre JSON del servicio: { "ok", "mensaje", "datos" }.
/// </summary>
public class ApiRespuesta
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("mensaje")]
    public string Mensaje { get; set; } = string.Empty;

    [JsonPropertyName("datos")]
    public JsonElement Datos { get; set; }
}
