﻿using Caliburn.Micro;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seeker
{
    public class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        private CompositionContainer container;

        protected override void Configure()
        {
            container = new CompositionContainer(new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()));

            CompositionBatch batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(container);

            container.Compose(batch);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = container.GetExportedValues<object>(contract);

            if (exports.Count() > 0)
            {
                return exports.First();
            }

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            ThemeManager.ChangeAppStyle(App.Current,
                                    ThemeManager.GetAccent(Properties.Settings.Default.UI_Accent),
                                    ThemeManager.GetAppTheme("BaseLight"));

            DisplayRootViewFor<AppViewModel>();
        }
    }
}
