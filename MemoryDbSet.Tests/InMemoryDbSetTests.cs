using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace MemoryDbSet.Tests
{
    public class InMemoryDbSetTests
    {
        public InMemoryDbSetTests()
        {
            InMemoryDbSet = new InMemoryDbSet<MyEntity>();
        }

        private class MyEntity
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }

        private InMemoryDbSet<MyEntity> InMemoryDbSet { get; }

        [Fact]
        public void Update_ShouldUpdateTheEntityWithTheSameId()
        {
            GivenADbSetWithEntities(new List<MyEntity>
            {
                new MyEntity {Value = "MyFirstValue"},
                new MyEntity {Value = "MySecondValue"},
                new MyEntity {Value = "MyThirdValue"}
            });

            var entityToUpdate = InMemoryDbSet.AsQueryable().Single(x => x.Value == "MySecondValue");
            entityToUpdate.Value = "MyValueUpdated";
            InMemoryDbSet.Update(entityToUpdate);

            ThenEntitiesShouldBe(new Collection<MyEntity>
            {
                new MyEntity {Id = 1, Value = "MyFirstValue"},
                new MyEntity {Id = 2, Value = "MyValueUpdated"},
                new MyEntity {Id = 3, Value = "MyThirdValue"}
            });
        }

        private void ThenEntitiesShouldBe(Collection<MyEntity> expectedEntities)
        {
            var actualEntities = InMemoryDbSet.AsEnumerable();

            for (var entityId = 1; entityId < actualEntities.Count(); entityId++)
            {
                var actualEntity = actualEntities.Single(x => x.Id == entityId);
                var expectedEntity = expectedEntities.Single(x => x.Id == entityId);

                actualEntity.ShouldBeEquivalentTo(expectedEntity);
            }
        }

        private void GivenADbSetWithEntities(IEnumerable<MyEntity> entities)
        {
            foreach (var entity in entities)
            {
                InMemoryDbSet.Add(entity);
            }

            return;
        }
    }
}