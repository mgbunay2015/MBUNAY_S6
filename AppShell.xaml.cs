using MBUNAY_S6.Vistas;

namespace MBUNAY_S6
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EstudianteView), typeof(EstudianteView));
            Routing.RegisterRoute("vNuevoEstudiante", typeof(vNuevoEstudiante));
            Routing.RegisterRoute(nameof(vEditarEstudiante), typeof(vEditarEstudiante));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        }
    }
}
