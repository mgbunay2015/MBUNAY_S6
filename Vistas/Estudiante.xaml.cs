using Microsoft.Extensions.DependencyInjection;
using MBUNAY_S6.Servicios;
using EstudianteModel = MBUNAY_S6.Modelos.Estudiante;

namespace MBUNAY_S6.Vistas;

public partial class Estudiante : ContentPage
{
    private readonly EstudianteApiService _api;
    private readonly CollectionView _datosView;

    public Estudiante() : this(ObtenerServicio())
    {
    }

    public Estudiante(EstudianteApiService api)
    {
        InitializeComponent();
        _api = api;

        _datosView = new CollectionView
        {
            ItemTemplate = new DataTemplate(() => CrearFilaGrid())
        };

        Content = new VerticalStackLayout
        {
            Margin = new Thickness(8),
            Children =
            {
                CrearEncabezadoGrid(),
                _datosView
            }
        };
    }

    private static Grid CrearEncabezadoGrid()
    {
        var grid = CrearGridBase();

        grid.Add(new Label { Text = "Codigo", FontAttributes = FontAttributes.Bold }, 0, 0);
        grid.Add(new Label { Text = "Nombre", FontAttributes = FontAttributes.Bold }, 1, 0);
        grid.Add(new Label { Text = "Apellido", FontAttributes = FontAttributes.Bold }, 2, 0);
        grid.Add(new Label { Text = "Edad", FontAttributes = FontAttributes.Bold }, 3, 0);

        return grid;
    }

    private static Grid CrearFilaGrid()
    {
        var grid = CrearGridBase();

        var codigo = new Label();
        codigo.SetBinding(Label.TextProperty, nameof(EstudianteModel.Codigo));

        var nombre = new Label();
        nombre.SetBinding(Label.TextProperty, nameof(EstudianteModel.Nombre));

        var apellido = new Label();
        apellido.SetBinding(Label.TextProperty, nameof(EstudianteModel.Apellido));

        var edad = new Label();
        edad.SetBinding(Label.TextProperty, nameof(EstudianteModel.Edad));

        grid.Add(codigo, 0, 0);
        grid.Add(nombre, 1, 0);
        grid.Add(apellido, 2, 0);
        grid.Add(edad, 3, 0);

        return grid;
    }

    private static Grid CrearGridBase()
    {
        return new Grid
        {
            Padding = new Thickness(0, 6),
            ColumnSpacing = 8,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = 60 },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = 50 },
            }
        };
    }

    private static EstudianteApiService ObtenerServicio()
        => IPlatformApplication.Current?.Services.GetService<EstudianteApiService>()
           ?? new EstudianteApiService();

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            _datosView.ItemsSource = await _api.ListarAsync();
        }
        catch
        {
            _datosView.ItemsSource = new List<EstudianteModel>();
        }
    }
}
