using MBUNAY_S6.Vistas;

namespace MBUNAY_S6
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnIrAlFormularioClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Estudiante));
        }
    }
}
