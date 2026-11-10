namespace MBUNAY_S6.Modelos;

  
public class Estudiante
{
    public int Codigo { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public int Edad { get; set; }

    public string Apellido { get; set; } = string.Empty;

    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

    public string Descripcion => $"{Codigo} · {NombreCompleto} ({Edad} años)";
}
