using System.Linq.Expressions;

namespace DAL.Abstract
{
    public interface IGenericDal<T> where T : class
    {
        void Add(T item);
        void Update(T item);
        void Delete(T item);
        T GetById(int id);
        List<T> GetAll();
        List<T> GetAll(Expression<Func<T, bool>> filter);
        int Count(Expression<Func<T, bool>> filter = null);
    }
}
