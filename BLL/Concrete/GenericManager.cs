using BLL.Abstract;
using BLL.Common;
using DAL.Abstract;
using FluentValidation;

namespace BLL.Concrete
{
    /// <summary>Shared CRUD logic: validation before every write, then the DAL call.</summary>
    public class GenericManager<T> : IGenericService<T> where T : class
    {
        protected readonly IGenericDal<T> Dal;
        private readonly IValidator<T> _validator;

        public GenericManager(IGenericDal<T> dal, IValidator<T> validator = null)
        {
            Dal = dal;
            _validator = validator;
        }

        protected Result Validate(T item)
        {
            if (_validator == null) return Result.Ok();
            var v = _validator.Validate(item);
            return v.IsValid ? Result.Ok() : Result.Fail(v.Errors.Select(e => e.ErrorMessage).Distinct().ToArray());
        }

        public virtual Result Add(T item)
        {
            var r = Validate(item);
            if (r.Success) Dal.Add(item);
            return r;
        }

        public virtual Result Update(T item)
        {
            var r = Validate(item);
            if (r.Success) Dal.Update(item);
            return r;
        }

        public virtual void Delete(T item) => Dal.Delete(item);
        public T GetById(int id) => Dal.GetById(id);
        public List<T> GetAll() => Dal.GetAll();
    }

    public class ContentManager<T> : GenericManager<T>, IContentService<T> where T : class
    {
        public ContentManager(IGenericDal<T> dal, IValidator<T> validator = null) : base(dal, validator) { }

        /// <summary>All items, optionally only published ones (Status = true), sorted by "Order" if the entity has one.</summary>
        public virtual List<T> GetList(bool onlyActive)
        {
            var list = Dal.GetAll();
            var status = typeof(T).GetProperty("Status");
            if (onlyActive && status != null) list = list.Where(x => (bool)status.GetValue(x)).ToList();
            var order = typeof(T).GetProperty("Order");
            return order != null ? list.OrderBy(x => (int)order.GetValue(x)).ToList() : list;
        }
    }
}
