using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace MemoryDbSet
{
    public class InMemoryDbSet<T> : IDbSet<T> where T : class
    {
        private int InsertCount { get; set; }

        public IList<T> List { get; }

        public InMemoryDbSet()
        {
            List = new List<T>();
        }
        public T Add(T entity)
        {
            var propertyInfo = GetIdProperty();
            if (propertyInfo != null && (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?)))
            {
                InsertCount++;
                propertyInfo.SetValue(entity, InsertCount);
            }
            List.Add(entity.Clone());
            return entity;
        }

        public T Attach(T entity)
        {
            return Add(entity);
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public T Find(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<T> Local => new ObservableCollection<T>(List);

        public T Remove(T entity)
        {
            var property = GetIdProperty();

            var entityToDelete = List.FirstOrDefault(e => property.GetValue(e).Equals(property.GetValue(entity)));
            if (entityToDelete != null)
            {
                List.Remove(entityToDelete);
            }

            return entityToDelete;
        }

        public IEnumerator<T> GetEnumerator() => List.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();

        public Type ElementType => typeof(T);

        public Expression Expression => Expression.Constant(List.AsQueryable());

        public IQueryProvider Provider => List.AsQueryable().Provider;

        public void Update(T entity)
        {
            var property = GetIdProperty();

            for(var i = 0; i < List.Count; i++)
            {
                if(property.GetValue(List[i]).Equals(property.GetValue(entity)))
                {
                    List[i] = entity.Clone();
                    break;
                }
            }
        }

        private PropertyInfo GetIdProperty()
        {
            return typeof (T).GetProperty("Id");
        }
    }
}

