using DDrop.BL.DropPhoto;
using DDrop.BL.Series;
using System.Windows;
using DDrop.DAL;
using DDrop.Utility.ExceptionHandling.ExceptionHandling;
using Unity;
using DDrop.BL.ImageProcessing.CSharp;
using DDrop.BL.ImageProcessing.Python;

namespace DDrop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly WindowExceptionHandler _exceptionHandler;

        public App()
        {
            _exceptionHandler = new WindowExceptionHandler();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IUnityContainer container = new UnityContainer();
            container.RegisterType<ISeriesBL, SeriesBL>();
            container.RegisterType<IDropPhotoBL, DropPhotoBL>();
            container.RegisterType<IDDropRepository, DDropRepository>();
            container.RegisterType<IDropletImageProcessor, DropletImageProcessor>();
            container.RegisterType<IPythonProvider, PythonProvider>();

            container.Resolve<MainWindow>();
        }
    }
}
