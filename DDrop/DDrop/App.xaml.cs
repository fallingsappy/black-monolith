using DDrop.BL.DropPhoto;
using DDrop.BL.Series;
using System.Windows;
using Unity;

namespace DDrop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IUnityContainer container = new UnityContainer();
            container.RegisterType<ISeriesBL, SeriesBL>();
            container.RegisterType<IDropPhotoBL, DropPhotoBL>();

            var window = container.Resolve<MainWindow>();
            //window.Show();
        }
    }
}
