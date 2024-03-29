﻿using System.Collections.Generic;
using Autofac;
using NUnit.Framework;
using Orchard.Events;
using System;

namespace Orchard.Tests.Events {
    [TestFixture]
    public class EventTests {
        private IContainer _container;
        private IEventBus _eventBus;
        private StubEventBusHandler _eventBusHandler;
        private StubEventHandler _eventHandler;

        [SetUp]
        public void Init() {
            var builder = new ContainerBuilder();
            _eventBusHandler = new StubEventBusHandler();
            _eventHandler = new StubEventHandler();
            builder.RegisterInstance(_eventBusHandler).As<IEventBusHandler>();
            builder.RegisterInstance(_eventHandler).As<IEventHandler>();
            builder.RegisterType<DefaultOrchardEventBus>().As<IEventBus>();
            _container = builder.Build();
            _eventBus = _container.Resolve<IEventBus>();
        }

        [Test]
        public void EventsAreCorrectlyDispatchedToEventHandlers() {
            Assert.That(_eventHandler.Count, Is.EqualTo(0));
            _eventBus.Notify("ITestEventHandler.Increment", new Dictionary<string, object>());
            Assert.That(_eventHandler.Count, Is.EqualTo(1));
        }

        [Test]
        public void EventParametersAreCorrectlyPassedToEventHandlers() {
            Assert.That(_eventHandler.Result, Is.EqualTo(0));
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments["a"] = 5200;
            arguments["b"] = 2600;
            _eventBus.Notify("ITestEventHandler.Substract", arguments);
            Assert.That(_eventHandler.Result, Is.EqualTo(2600));
        }

        [Test]
        public void EventParametersArePassedInCorrectOrderToEventHandlers() {
            Assert.That(_eventHandler.Result, Is.EqualTo(0));
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments["a"] = 2600;
            arguments["b"] = 5200;
            _eventBus.Notify("ITestEventHandler.Substract", arguments);
            Assert.That(_eventHandler.Result, Is.EqualTo(-2600));
        }

        [Test]
        public void EventParametersAreCorrectlyPassedToMatchingMethod() {
            Assert.That(_eventHandler.Summary, Is.Null);
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments["a"] = "a";
            arguments["b"] = "b";
            arguments["c"] = "c";
            _eventBus.Notify("ITestEventHandler.Concat", arguments);
            Assert.That(_eventHandler.Summary, Is.EqualTo("abc"));
        }

        [Test]
        public void EventParametersAreCorrectlyPassedToExactlyMatchingMethod() {
            Assert.That(_eventHandler.Result, Is.EqualTo(0));
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments["a"] = 1000;
            arguments["b"] = 100;
            arguments["c"] = 10;
            _eventBus.Notify("ITestEventHandler.Sum", arguments);
            Assert.That(_eventHandler.Result, Is.EqualTo(1110));
        }

        [Test]
        public void EventParametersAreCorrectlyPassedToBestMatchingMethodAndExtraParametersAreIgnored() {
            Assert.That(_eventHandler.Result, Is.EqualTo(0));
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments["a"] = 1000;
            arguments["b"] = 100;
            arguments["c"] = 10;
            arguments["e"] = 1;
            _eventBus.Notify("ITestEventHandler.Sum", arguments);
            Assert.That(_eventHandler.Result, Is.EqualTo(1110));
        }

        [Test]
        public void EventParametersAreCorrectlyPassedToBestMatchingMethodAndExtraParametersAreIgnored2() {
            Assert.That(_eventHandler.Result, Is.EqualTo(0));
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments["a"] = 1000;
            arguments["e"] = 1;
            _eventBus.Notify("ITestEventHandler.Sum", arguments);
            Assert.That(_eventHandler.Result, Is.EqualTo(3000));
        }

        [Test]
        public void EventParametersAreCorrectlyPassedToExactlyMatchingMethodWhenThereIsOne() {
            Assert.That(_eventHandler.Result, Is.EqualTo(0));
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments["a"] = 1000;
            arguments["b"] = 100;
            _eventBus.Notify("ITestEventHandler.Sum", arguments);
            Assert.That(_eventHandler.Result, Is.EqualTo(2200));
        }

        [Test]
        public void EventParametersAreCorrectlyPassedToExactlyMatchingMethodWhenThereIsOne2() {
            Assert.That(_eventHandler.Result, Is.EqualTo(0));
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments["a"] = 1000;
            _eventBus.Notify("ITestEventHandler.Sum", arguments);
            Assert.That(_eventHandler.Result, Is.EqualTo(3000));
        }

        [Test]
        public void EventHandlerWontBeCalledWhenNoParameterMatchExists() {
            Assert.That(_eventHandler.Result, Is.EqualTo(0));
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments["e"] = 1;
            _eventBus.Notify("ITestEventHandler.Sum", arguments);
            Assert.That(_eventHandler.Result, Is.EqualTo(0));
        }

        [Test]
        public void EventHandlerWontBeCalledWhenNoParameterMatchExists2() {
            Assert.That(_eventHandler.Result, Is.EqualTo(0));
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            _eventBus.Notify("ITestEventHandler.Sum", arguments);
            Assert.That(_eventHandler.Result, Is.EqualTo(0));
        }

        [Test]
        public void EventHandlerWontThrowIfMethodDoesNotExists() {
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            Assert.DoesNotThrow(() => _eventBus.Notify("ITestEventHandler.NotExisting", arguments));
        }

        [Test]
        public void EventBusThrowsIfMessageNameIsNotCorrectlyFormatted() {
            Assert.Throws<ArgumentException>(() => _eventBus.Notify("StubEventHandlerIncrement", new Dictionary<string, object>()));
        }

        public interface ITestEventHandler : IEventHandler {
            void Increment();
            void Sum(int a);
            void Sum(int a, int b);
            void Sum(int a, int b, int c);
            void Substract(int a, int b);
            void Concat(string a, string b, string c);
        }

        public class StubEventHandler : ITestEventHandler {
            public int Count { get; set; }
            public int Result { get; set; }
            public string Summary { get; set; }

            public void Increment() {
                Count++;
            }

            public void Sum(int a) {
                Result = 3 * a;
            }

            public void Sum(int a, int b) {
                Result = 2 * ( a + b );
            }

            public void Sum(int a, int b, int c) {
                Result = a + b + c;
            }

            public void Substract(int a, int b) {
                Result = a - b;
            }

            public void Concat(string a, string b, string c) {
                Summary = a + b + c;
            }
        }

        [Test]
        public void EventsAreCorrectlyDispatchedToHandlers_Obsolete() {
            Assert.That(_eventBusHandler.LastMessageName, Is.Null);
            _eventBus.Notify_Obsolete("Notification", new Dictionary<string, string>());
            Assert.That(_eventBusHandler.LastMessageName, Is.EqualTo("Notification"));
        }

        public class StubEventBusHandler : IEventBusHandler {
            public string LastMessageName { get; set; }

            #region Implementation of IEventBusHandler

            public void Process(string messageName, IDictionary<string, string> eventData) {
                LastMessageName = messageName;
            }

            #endregion
        }
    }
}
