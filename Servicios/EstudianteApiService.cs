using System.Text;
using System.Text.Json;
using MBUNAY_S6.Modelos;

namespace MBUNAY_S6.Servicios;

/// <summary>
/// Cliente HTTP del Web Service <c>api/estudiante.php</c>.
/// Usa <see cref="JsonSerializerConfig"/> y <c>EstudianteJsonConverter</c> para leer/escribir JSON.
/// </summary>
public class EstudianteApiService
{
    private static JsonSerializerOptions JsonOptions => JsonSerializerConfig.Opciones;

    private readonly HttpClient _http;

    public EstudianteApiService()
    {
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
    }

    public string Mensaje { get; private set; } = string.Empty;

    public string UltimaUrl { get; private set; } = string.Empty;

    public async Task<bool> ProbarConexionAsync()
    {
        try
        {
            var env = await GetAsync(ApiSettings.Url + "?accion=contar");
            if (env is { Ok: true })
            {
                Mensaje = $"Conexión correcta. {env.Mensaje}";
                return true;
            }

            Mensaje = string.IsNullOrWhiteSpace(Mensaje) ? "El servidor respondió sin datos." : Mensaje;
            return false;
        }
        catch (Exception ex)
        {
            Mensaje = $"No se pudo conectar: {ex.Message}";
            return false;
        }
    }

    public async Task<List<Estudiante>> ListarAsync()
    {
        var env = await GetAsync(ApiSettings.Url);
        if (!env.Ok || env.Datos.ValueKind != JsonValueKind.Array)
            return [];

        try
        {
            return env.Datos.Deserialize<List<Estudiante>>(JsonOptions) ?? [];
        }
        catch (JsonException ex)
        {
            Mensaje = "Respuesta inesperada del servidor: " + ex.Message;
            return [];
        }
    }

    public async Task<Estudiante?> ObtenerAsync(int codigo)
    {
        var env = await GetAsync($"{ApiSettings.Url}?id={codigo}");
        if (!env.Ok)
            return null;

        return env.Datos.Deserialize<Estudiante>(JsonOptions);
    }

    public async Task<int> ContarAsync()
    {
        var env = await GetAsync(ApiSettings.Url + "?accion=contar");
        if (!env.Ok || env.Datos.ValueKind != JsonValueKind.Object)
            return 0;

        return env.Datos.TryGetProperty("total", out var total) && total.TryGetInt32(out var n) ? n : 0;
    }

    public async Task<int> InsertarAsync(Estudiante estudiante)
    {
        var env = await EnviarEstudianteAsync(HttpMethod.Post, ApiSettings.Url, estudiante);
        if (!env.Ok)
            return 0;

        if (env.Datos.ValueKind == JsonValueKind.Object &&
            env.Datos.TryGetProperty("id", out var id) && id.TryGetInt32(out var nuevoId))
        {
            estudiante.Codigo = nuevoId;
            return nuevoId;
        }

        return 1;
    }

    public async Task<bool> ActualizarAsync(Estudiante estudiante)
    {
        var env = await EnviarEstudianteAsync(HttpMethod.Put, ApiSettings.Url, estudiante);
        return env.Ok;
    }

    public async Task<bool> EliminarAsync(int codigo)
    {
        try
        {
            UltimaUrl = $"{ApiSettings.Url}?id={codigo}";
            using var response = await _http.DeleteAsync(UltimaUrl);
            var env = await LeerRespuestaAsync(response);
            Mensaje = env.Mensaje;
            return env.Ok;
        }
        catch (Exception ex)
        {
            Mensaje = $"No se pudo conectar: {ex.Message}";
            return false;
        }
    }

    private async Task<ApiRespuesta> GetAsync(string url)
    {
        try
        {
            UltimaUrl = url;
            using var response = await _http.GetAsync(url);
            var env = await LeerRespuestaAsync(response);
            Mensaje = env.Mensaje;
            return env;
        }
        catch (Exception ex)
        {
            Mensaje = $"No se pudo conectar: {ex.Message}";
            return new ApiRespuesta { Ok = false, Mensaje = Mensaje };
        }
    }

    private async Task<ApiRespuesta> EnviarEstudianteAsync(HttpMethod metodo, string url, Estudiante estudiante)
    {
        try
        {
            UltimaUrl = url;
            var json = JsonSerializer.Serialize(estudiante, JsonOptions);
            using var request = new HttpRequestMessage(metodo, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            using var response = await _http.SendAsync(request);
            var env = await LeerRespuestaAsync(response);
            Mensaje = env.Mensaje;
            return env;
        }
        catch (Exception ex)
        {
            Mensaje = $"No se pudo conectar: {ex.Message}";
            return new ApiRespuesta { Ok = false, Mensaje = Mensaje };
        }
    }

    private static async Task<ApiRespuesta> LeerRespuestaAsync(HttpResponseMessage response)
    {
        var texto = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(texto))
        {
            return new ApiRespuesta
            {
                Ok = false,
                Mensaje = $"El servidor respondió vacío (HTTP {(int)response.StatusCode})."
            };
        }

        try
        {
            var env = JsonSerializer.Deserialize<ApiRespuesta>(texto, JsonOptions);
            if (env is null)
                return new ApiRespuesta { Ok = false, Mensaje = "Respuesta vacía del servidor." };

            if (!env.Ok && string.IsNullOrWhiteSpace(env.Mensaje))
                env.Mensaje = $"Error HTTP {(int)response.StatusCode}.";

            return env;
        }
        catch (JsonException)
        {
            return new ApiRespuesta
            {
                Ok = false,
                Mensaje = $"Respuesta no válida del servidor (HTTP {(int)response.StatusCode}). " +
                          "Revisa que Apache y MySQL estén iniciados en XAMPP."
            };
        }
    }
}
