﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using NHibernate;
using NUnit.Framework;
using Orchard.Data;
using Orchard.Services;
using Orchard.Tests.Data;
using Orchard.Tests.Stubs;

namespace Orchard.Tests.Modules {
    public abstract class DatabaseEnabledTestsBase {

        protected IContainer _container;

        protected ISession _session;
        protected string _databaseFilePath;
        protected ISessionFactory _sessionFactory;
        protected StubClock _clock;


        [TestFixtureSetUp]
        public void InitFixture() {
            _databaseFilePath = Path.GetTempFileName();
        }

        [TestFixtureTearDown]
        public void TearDownFixture() {
            File.Delete(_databaseFilePath);
        }

        [SetUp]
        public virtual void Init() {
            _sessionFactory = DataUtility.CreateSessionFactory(_databaseFilePath, DatabaseTypes.ToArray());
            _session = _sessionFactory.OpenSession();
            _clock = new StubClock();

            var builder = new ContainerBuilder();
            //builder.RegisterModule(new ImplicitCollectionSupportModule());
            builder.RegisterInstance(new StubLocator(_session)).As<ISessionLocator>();
            builder.RegisterInstance(_clock).As<IClock>();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            Register(builder);
            _container = builder.Build();
        }

        [TearDown]
        public void Cleanup() {
            _container.Dispose();
            _session.Close();
        }

        public abstract void Register(ContainerBuilder builder);

        protected virtual IEnumerable<Type> DatabaseTypes {
            get {
                return Enumerable.Empty<Type>();
            }
        }

        protected void ClearSession() {
            _session.Flush();
            _session.Clear();
        }
    }
}