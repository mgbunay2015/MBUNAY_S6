using Microsoft.Extensions.DependencyInjection;
using MBUNAY_S6.Modelos;
using MBUNAY_S6.Servicios;

namespace MBUNAY_S6.Vistas;

public partial class EstudianteView : ContentPage
{
    private readonly EstudianteApiService _api;
    private List<Estudiante> _lista = [];

    public EstudianteView() : this(ObtenerServicio())
    {
    }

    public EstudianteView(EstudianteApiService api)
    {
        InitializeComponent();
        _api = api; 
    }

    private static EstudianteApiService ObtenerServicio()
        => IPlatformApplication.Current?.Services.GetService<EstudianteApiService>()
           ?? new EstudianteApiService();

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            _lista = await _api.ListarAsync();
            DatosView.ItemsSource = _lista;
        }
        catch
        {
            _lista = [];
            DatosView.ItemsSource = _lista; 
        }
    }

    private async void btnagregar_Clicked(object? sender, EventArgs e)
    {
        if (Shell.Current is null)
            return;

        await Shell.Current.GoToAsync(nameof(vNuevoEstudiante));
    }

    private async void OnItemSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Estudiante estudiante)
            return;

        DatosView.SelectedItem = null;

        await IrEditarDesdeGridAsync(estudiante);
    }

    private async Task IrEditarDesdeGridAsync(Estudiante estudiante)
    {
        if (Shell.Current is null)
            return;

        int indice = _lista.FindIndex(f => f.Codigo == estudiante.Codigo);
        if (indice < 0)
            indice = 0;

        await Shell.Current.GoToAsync(
            nameof(vEditarEstudiante),
            new Dictionary<string, object>
            {
                ["Indice"] = indice,
                ["Codigo"] = estudiante.Codigo,
                ["Nombre"] = estudiante.Nombre,
                ["Apellido"] = estudiante.Apellido,
                ["Edad"] = estudiante.Edad,
                ["Estudiante"] = estudiante
            });
    }
}
