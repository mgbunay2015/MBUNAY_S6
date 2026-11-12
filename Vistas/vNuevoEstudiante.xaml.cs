using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MBUNAY_S6.Servicios;
using EstudianteModel = MBUNAY_S6.Modelos.Estudiante;

namespace MBUNAY_S6.Vistas;

public partial class vNuevoEstudiante : ContentPage
{
    private readonly EstudianteApiService _api;

    public vNuevoEstudiante() : this(ObtenerServicio())
    {
    }

    public vNuevoEstudiante(EstudianteApiService api)
    {
        InitializeComponent();
        _api = api;
        EdadEntry.TextChanged += OnEdadTextChanged;
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

    private async void OnGuardarClicked(object? sender, EventArgs e)
    {
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
            Nombre = nombre,
            Apellido = apellido,
            Edad = edad
        };

        int codigo = await _api.InsertarAsync(estudiante);
        if (codigo <= 0)
        {
            await MostrarAvisoAsync(_api.Mensaje);
            return;
        }

        await DisplayAlertAsync("Guardado", $"Estudiante registrado. Codigo: {codigo}", "OK");
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
