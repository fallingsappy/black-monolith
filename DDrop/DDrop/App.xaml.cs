﻿using DDrop.BL.DropPhoto;
using DDrop.BL.Series;
using System.Windows;
using DDrop.BL.AppStateBL;
using DDrop.BL.Calculation;
using DDrop.BL.GeometryBL;
using DDrop.DAL;
using DDrop.Utility.ExceptionHandling.ExceptionHandling;
using Unity;
using DDrop.BL.ImageProcessing.CSharp;
using DDrop.BL.ImageProcessing.Python;
using DDrop.Utility.Logger;
using Unity.Injection;
using Unity.Lifetime;

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
            container.RegisterType<IGeometryBL, GeometryBL>();
            container.RegisterType<IDropPhotoBL, DropPhotoBL>();
            container.RegisterType<ICalculationBL, CalculationBL>(new InjectionConstructor(
                new ResolvedParameter<IDDropRepository>()));
            container.RegisterType<IAppStateBL, AppStateBL>();
            container.RegisterType<IDDropRepository, DDropRepository>();
            container.RegisterType<IDropletImageProcessor, DropletImageProcessor>();
            container.RegisterType<IPythonProvider, PythonProvider>();
            container.RegisterType<ILogger, Logger>(new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<IDDropRepository>()
                ));

            container.Resolve<MainWindow>();
        }
    }
}
