using System.Linq;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Web;
using System.Collections.Generic;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.Environment.ShellBuilders;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Mvc.ViewEngines;
using Orchard.Utility.Extensions;

namespace Orchard.Environment {
    public class DefaultOrchardHost : IOrchardHost {
        private readonly ControllerBuilder _controllerBuilder;

        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IShellContextFactory _shellContextFactory;
        private readonly IRunningShellTable _runningShellTable;

        private IEnumerable<ShellContext> _current;

        public DefaultOrchardHost(
            IShellSettingsManager shellSettingsManager,
            IShellContextFactory shellContextFactory,
            IRunningShellTable runningShellTable,
            ControllerBuilder controllerBuilder) {
            //_containerProvider = containerProvider;
            _shellSettingsManager = shellSettingsManager;
            _shellContextFactory = shellContextFactory;
            _runningShellTable = runningShellTable;
            _controllerBuilder = controllerBuilder;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public IList<ShellContext> Current {
            get { return BuildCurrent().ToReadOnlyCollection(); }
        }

        void IOrchardHost.Initialize() {
            Logger.Information("Initializing");
            ViewEngines.Engines.Insert(0, LayoutViewEngine.CreateShim());
            _controllerBuilder.SetControllerFactory(new OrchardControllerFactory());
            //ServiceLocator.SetLocator(t => _containerProvider.RequestLifetime.Resolve(t));

            BuildCurrent();
        }

        void IOrchardHost.Reinitialize_Obsolete() {
            _current = null;
        }


        void IOrchardHost.BeginRequest() {
            Logger.Debug("BeginRequest");
            BeginRequest();
        }

        void IOrchardHost.EndRequest() {
            Logger.Debug("EndRequest");
            EndRequest();
        }

        IStandaloneEnvironment IOrchardHost.CreateStandaloneEnvironment(ShellSettings shellSettings) {
            Logger.Debug("Creating standalone environment for tenant {0}", shellSettings.Name);
            var shellContext = CreateShellContext(shellSettings);
            return new StandaloneEnvironment(shellContext.LifetimeScope);
        }


        IEnumerable<ShellContext> BuildCurrent() {
            lock (this) {
                return _current ?? (_current = CreateAndActivate().ToArray());
            }
        }

        IEnumerable<ShellContext> CreateAndActivate() {
            var allSettings = _shellSettingsManager.LoadSettings();
            if (allSettings.Any()) {
                return allSettings.Select(
                    settings => {
                        var context = CreateShellContext(settings);
                        context.Shell.Activate();
                        _runningShellTable.Add(settings);
                        return context;
                    });
            }

            var setupContext = CreateSetupContext();
            setupContext.Shell.Activate();
            _runningShellTable.Add(setupContext.Settings);
            return new[] { setupContext };
        }

        ShellContext CreateSetupContext() {
            Logger.Debug("Creating shell context for setup");
            return _shellContextFactory.CreateSetupContext();
        }

        ShellContext CreateShellContext(ShellSettings settings) {
            Logger.Debug("Creating shell context for tenant {0}", settings.Name);
            return _shellContextFactory.CreateShellContext(settings);
        }

        protected virtual void BeginRequest() {
            BuildCurrent();
        }

        protected virtual void EndRequest() {
        }


        private void HackSimulateExtensionActivation(ILifetimeScope shellContainer) {
            var containerProvider = new FiniteContainerProvider(shellContainer);
            try {
                var requestContainer = containerProvider.RequestLifetime;

                var hackInstallationGenerator = requestContainer.Resolve<IHackInstallationGenerator>();
                hackInstallationGenerator.GenerateActivateEvents();
            }
            finally {
                containerProvider.EndRequestLifetime();
            }
        }

    }
}
