﻿using System.IO;
using System.Web.Hosting;
using NHibernate;
using Orchard.Data.Migrations;
using Orchard.Environment;
using Orchard.Environment.Configuration;

namespace Orchard.Data {
    public interface ISessionFactoryHolder : ISingletonDependency {
        ISessionFactory GetSessionFactory();
        void UpdateSchema();
    }

    public class SessionFactoryHolder : ISessionFactoryHolder {
        private readonly IShellSettings _shellSettings;
        private readonly ICompositionStrategy _compositionStrategy;
        private readonly IDatabaseMigrationManager _databaseMigrationManager;
        private readonly IAppDataFolder _appDataFolder;

        private ISessionFactory _sessionFactory;

        public SessionFactoryHolder(
            IShellSettings shellSettings,
            ICompositionStrategy compositionStrategy,
            IDatabaseMigrationManager databaseMigrationManager,
            IAppDataFolder appDataFolder) {
            _shellSettings = shellSettings;
            _compositionStrategy = compositionStrategy;
            _databaseMigrationManager = databaseMigrationManager;
            _appDataFolder = appDataFolder;
        }


        public void UpdateSchema() {
            var coordinator = GetDatabaseCoordinator();
            coordinator.UpdateSchema(_compositionStrategy.GetRecordDescriptors());
        }

        public ISessionFactory GetSessionFactory() {
            lock (this) {
                if (_sessionFactory == null) {
                    _sessionFactory = BuildSessionFactory();
                }
            }
            return _sessionFactory;
        }

        private ISessionFactory BuildSessionFactory() {
            var coordinator = GetDatabaseCoordinator();
            return coordinator.BuildSessionFactory(_compositionStrategy.GetRecordDescriptors());
        }


        private IDatabaseCoordinator GetDatabaseCoordinator() {
            var shellPath = _appDataFolder.CreateDirectory(Path.Combine("Sites", _shellSettings.Name));

            return _databaseMigrationManager.CreateCoordinator(_shellSettings.DataProvider, shellPath, _shellSettings.DataConnectionString);
        }
    }


}
