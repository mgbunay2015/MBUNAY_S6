using MBUNAY_S6.Vistas;

namespace MBUNAY_S6
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(Estudiante), typeof(Estudiante));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        }
    }
}
