using System.Text.Json;
using System.Text.Json.Serialization;
using MBUNAY_S6.Converters;

namespace MBUNAY_S6.Servicios;

public static class JsonSerializerConfig
{
  public static JsonSerializerOptions Opciones { get; } = Crear();

  private static JsonSerializerOptions Crear()
  {
    var opciones = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
      NumberHandling = JsonNumberHandling.AllowReadingFromString,
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    opciones.Converters.Add(new EstudianteJsonConverter());
    return opciones;
  }
}
