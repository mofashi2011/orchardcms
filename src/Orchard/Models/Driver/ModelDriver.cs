using System;
using Orchard.Logging;

namespace Orchard.Models.Driver {
    public abstract class ModelDriver : IModelDriver {
        protected ModelDriver() {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        void IModelDriver.New(NewModelContext context) { New(context); }
        void IModelDriver.Newed(NewedModelContext context) { Newed(context); }
        void IModelDriver.Create(CreateModelContext context) { Create(context); }
        void IModelDriver.Created(CreateModelContext context) { Created(context); }
        void IModelDriver.Load(LoadModelContext context) { Load(context); }
        void IModelDriver.Loaded(LoadModelContext context) { Loaded(context); }
        void IModelDriver.GetEditors(GetModelEditorsContext context) { GetEditors(context); }
        void IModelDriver.UpdateEditors(UpdateModelContext context) { UpdateEditors(context); }

        protected virtual void New(NewModelContext context) { }
        protected virtual void Newed(NewedModelContext context) { }

        protected virtual void Load(LoadModelContext context) { }
        protected virtual void Loaded(LoadModelContext context) { }

        protected virtual void Create(CreateModelContext context) { }
        protected virtual void Created(CreateModelContext context) { }

        protected virtual void GetEditors(GetModelEditorsContext context) {}

        protected virtual void UpdateEditors(UpdateModelContext context) {}
    }
}