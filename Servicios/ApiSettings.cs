namespace MBUNAY_S6.Servicios;

public static class ApiSettings
{
#if ANDROID
    public const string Url = "http://10.0.2.2/ws_uisrael/api/estudiante.php";
#else
    public const string Url = "http://localhost/ws_uisrael/api/estudiante.php";
#endif
}
