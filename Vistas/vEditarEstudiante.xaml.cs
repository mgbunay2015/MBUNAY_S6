using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MBUNAY_S6.Servicios;
using EstudianteModel = MBUNAY_S6.Modelos.Estudiante;

namespace MBUNAY_S6.Vistas;

public partial class vEditarEstudiante : ContentPage, IQueryAttributable
{
    private readonly EstudianteApiService _api;
    private int _codigo;

    public vEditarEstudiante() : this(ObtenerServicio())
    {
    }

    public vEditarEstudiante(EstudianteApiService api)
    {
        InitializeComponent();
        _api = api;
        EdadEntry.TextChanged += OnEdadTextChanged;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Estudiante", out var estObj) && estObj is EstudianteModel estudiante)
            CargarDatos(estudiante);
    }

    private void CargarDatos(EstudianteModel estudiante)
    {
        _codigo = estudiante.Codigo;
        CodigoLabel.Text = estudiante.Codigo.ToString();
        NombreEntry.Text = estudiante.Nombre;
        ApellidoEntry.Text = estudiante.Apellido;
        EdadEntry.Text = estudiante.Edad.ToString();
    }

    private void OnEdadTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry)
            return;

        var texto = e.NewTextValue ?? string.Empty;
        var soloDigitos = string.Concat(texto.Where(char.IsDigit));
        if (soloDigitos == texto)
            return;

        entry.Text = soloDigitos;
    }

    private static EstudianteApiService ObtenerServicio()
        => IPlatformApplication.Current?.Services.GetService<EstudianteApiService>()
             ?? new EstudianteApiService();

    private async void OnActualizarClicked(object? sender, EventArgs e)
    {
        if (_codigo <= 0)
        {
            await MostrarAvisoAsync("No hay estudiante cargado.");
            return;
        }

        var nombre = NombreEntry.Text?.Trim() ?? string.Empty;
        var apellido = ApellidoEntry.Text?.Trim() ?? string.Empty;
        var edadTexto = EdadEntry.Text?.Trim() ?? string.Empty;

        if (nombre.Length == 0 || apellido.Length == 0)
        {
            await MostrarAvisoAsync("Nombre y apellido son obligatorios.");
            return;
        }

        if (!int.TryParse(edadTexto, out var edad) || edad < 0 || edad > 130)
        {
            await MostrarAvisoAsync("Edad invalida (0 a 130).");
            return;
        }

        var estudiante = new EstudianteModel
        {
            Codigo = _codigo,
            Nombre = nombre,
            Apellido = apellido,
            Edad = edad
        };

        if (!await _api.ActualizarAsync(estudiante))
        {
            await MostrarAvisoAsync(_api.Mensaje);
            return;
        }

        await DisplayAlertAsync("Actualizado", "Estudiante modificado correctamente.", "OK");
        await VolverAListaAsync();
    }

    private async void OnEliminarClicked(object? sender, EventArgs e)
    {
        if (_codigo <= 0)
        {
            await MostrarAvisoAsync("No hay estudiante cargado.");
            return;
        }

        bool confirmar = await DisplayAlertAsync(
            "Eliminar",
            $"¿Eliminar el estudiante con codigo {_codigo}?",
            "Si",
            "No");

        if (!confirmar)
            return;

        if (!await _api.EliminarAsync(_codigo))
        {
            await MostrarAvisoAsync(_api.Mensaje);
            return;
        }

        await DisplayAlertAsync("Eliminado", "Estudiante eliminado.", "OK");
        await VolverAListaAsync();
    }

    private static async Task VolverAListaAsync()
    {
        if (Shell.Current is not null)
            await Shell.Current.GoToAsync("..");
    }

    private async Task MostrarAvisoAsync(string texto)
    {
        MensajeLabel.Text = texto;
        MensajeLabel.IsVisible = true;
        await DisplayAlertAsync("Aviso", texto, "OK");
    }
}
