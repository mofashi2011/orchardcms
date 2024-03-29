﻿using System;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using Autofac;
using Autofac.Configuration;
using Autofac.Integration.Web;
using Orchard.Caching;
using Orchard.Environment.AutofacUtil;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Folders;
using Orchard.Environment.Extensions.Loaders;
using Orchard.Environment.ShellBuilders;
using Orchard.Environment.Topology;
using Orchard.Events;
using Orchard.FileSystems.AppData;
using Orchard.FileSystems.WebSite;
using Orchard.Logging;
using Orchard.Services;

namespace Orchard.Environment {
    public static class OrchardStarter {
        public static IContainer CreateHostContainer(Action<ContainerBuilder> registrations) {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggingModule());
            builder.RegisterModule(new EventsModule());
            builder.RegisterModule(new CacheModule());

            // a single default host implementation is needed for bootstrapping a web app domain
            builder.RegisterType<DefaultOrchardEventBus>().As<IEventBus>().SingleInstance();
            builder.RegisterType<DefaultCacheHolder>().As<ICacheHolder>().SingleInstance();

            RegisterVolatileProvider<WebSiteFolder, IWebSiteFolder>(builder);
            RegisterVolatileProvider<AppDataFolder, IAppDataFolder>(builder);
            RegisterVolatileProvider<Clock, IClock>(builder);

            builder.RegisterType<DefaultOrchardHost>().As<IOrchardHost>().As<IEventHandler>().SingleInstance();
            {
                builder.RegisterType<ShellSettingsManager>().As<IShellSettingsManager>().SingleInstance();

                builder.RegisterType<ShellContextFactory>().As<IShellContextFactory>().SingleInstance();
                {
                    builder.RegisterType<ShellDescriptorCache>().As<IShellDescriptorCache>().SingleInstance();

                    builder.RegisterType<CompositionStrategy>()
                        .As<ICompositionStrategy>()
                        .SingleInstance();
                    {
                        builder.RegisterType<ExtensionManager>().As<IExtensionManager>().SingleInstance();
                        {
                            builder.RegisterType<ModuleFolders>().As<IExtensionFolders>()
                                .WithParameter(new NamedParameter("paths", new[] { "~/Core", "~/Modules" }))
                                .SingleInstance();
                            builder.RegisterType<AreaFolders>().As<IExtensionFolders>()
                                .WithParameter(new NamedParameter("paths", new[] { "~/Areas" }))
                                .SingleInstance();
                            builder.RegisterType<ThemeFolders>().As<IExtensionFolders>()
                                .WithParameter(new NamedParameter("paths", new[] { "~/Core", "~/Themes" }))
                                .SingleInstance();

                            builder.RegisterType<AreaExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                            builder.RegisterType<CoreExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                            builder.RegisterType<ReferencedExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                            builder.RegisterType<PrecompiledExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                            builder.RegisterType<DynamicExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                        }
                    }

                    builder.RegisterType<ShellContainerFactory>().As<IShellContainerFactory>().SingleInstance();
                }
            }

            builder.RegisterType<RunningShellTable>().As<IRunningShellTable>().SingleInstance();
            builder.RegisterType<DefaultOrchardShell>().As<IOrchardShell>().InstancePerMatchingLifetimeScope("shell");

            // The container provider gives you access to the lowest container at the time, 
            // and dynamically creates a per-request container. The EndRequestLifetime method
            // still needs to be called on end request, but that's the host component's job to worry about
            //builder.RegisterType<ContainerProvider>().As<IContainerProvider>().InstancePerLifetimeScope();



            registrations(builder);


            var autofacSection = ConfigurationManager.GetSection(ConfigurationSettingsReader.DefaultSectionName);
            if (autofacSection != null)
                builder.RegisterModule(new ConfigurationSettingsReader());

            var optionalHostConfig = HostingEnvironment.MapPath("~/Config/Host.config");
            if (File.Exists(optionalHostConfig))
                builder.RegisterModule(new ConfigurationSettingsReader(ConfigurationSettingsReader.DefaultSectionName, optionalHostConfig));

            builder
                .Register(ctx => new LifetimeScopeContainer(ctx.Resolve<ILifetimeScope>()))
                .As<IContainer>()
                .InstancePerMatchingLifetimeScope("shell");

            return builder.Build();
        }

        private static void RegisterVolatileProvider<TRegister, TService>(ContainerBuilder builder) where TService : IVolatileProvider {
            builder.RegisterType<TRegister>()
                .As<TService>()
                .As<IVolatileProvider>()
                .SingleInstance();
        }

        public static IOrchardHost CreateHost(Action<ContainerBuilder> registrations) {
            var container = CreateHostContainer(registrations);
            return container.Resolve<IOrchardHost>();
        }
    }
}
