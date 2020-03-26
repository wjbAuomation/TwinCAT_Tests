using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using TC_Prism.Views;

namespace TC_Prism
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // registering types for DI (in this case we have dependency container - unity)
            containerRegistry.Register<Services.IConnectionList, Services.ConnectionList>();
        }

        protected override Window CreateShell()
        {
            MainWindow w = Container.Resolve<MainWindow>();
            return w;
        }
    }
}
