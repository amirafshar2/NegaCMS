using System.Linq.Expressions;
using DAL.Abstract;
using DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    /// <summary>Base repository. The DbContext is injected (one per HTTP request).</summary>
    public class GenericRepository<T> : IGenericDal<T> where T : class
    {
        protected readonly DB Db;

        public GenericRepository(DB db) => Db = db;

        public void Add(T item)
        {
            Db.Set<T>().Add(item);
            Db.SaveChanges();
        }

        public void Update(T item)
        {
            Db.Set<T>().Update(item);
            Db.SaveChanges();
        }

        public void Delete(T item)
        {
            if (item == null) return;
            Db.Set<T>().Remove(item);
            Db.SaveChanges();
        }

        public T GetById(int id) => Db.Set<T>().Find(id);

        public List<T> GetAll() => Db.Set<T>().AsNoTracking().ToList();

        public List<T> GetAll(Expression<Func<T, bool>> filter) =>
            Db.Set<T>().AsNoTracking().Where(filter).ToList();

        public int Count(Expression<Func<T, bool>> filter = null) =>
            filter == null ? Db.Set<T>().Count() : Db.Set<T>().Count(filter);
    }
}
