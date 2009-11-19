using NUnit.Framework;
using Orchard.Models;
using Orchard.Models.Driver;

namespace Orchard.Tests.Models.Drivers {

    [TestFixture]
    public class ModelDriverTests {
        [Test]
        public void ModelDriverShouldUsePersistenceFilterToDelegateCreateAndLoad() {
            var modelDriver = new TestModelDriver();

            var contentItem = new ContentItem();
            var part = new TestModelPart();
            contentItem.Weld(part);

            ((IModelDriver)modelDriver).Creating(new CreateModelContext { ContentItem = contentItem });
            Assert.That(part.CreatingCalled, Is.True);
        }

        [Test]
        public void PartShouldBeAddedBasedOnSimplePredicate() {
            var modelDriver = new TestModelDriver();

            var builder = new ContentItemBuilder("testing");
            ((IModelDriver)modelDriver).Activating(new ActivatingModelContext { Builder = builder, ModelType = "testing" });
            var model = builder.Build();
            Assert.That(model.Is<TestModelPart>(), Is.True);
            Assert.That(model.As<TestModelPart>(), Is.Not.Null);
        }

        public class TestModelPart : ContentItemPart {
            public bool CreatingCalled { get; set; }
        }


        public class TestModelDriver : ModelDriver {
            public TestModelDriver() {
                Filters.Add(new ActivatingFilter<TestModelPart>(x => x == "testing"));
                Filters.Add(new TestModelStorageFilter());
            }
        }

        public class TestModelStorageFilter : StorageFilterBase<TestModelPart> {
            protected override void Creating(CreateModelContext context, TestModelPart instance) {
                instance.CreatingCalled = true;
            }
        }
    }
}

