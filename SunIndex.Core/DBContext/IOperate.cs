using System;

namespace SunIndex.Core.DBContext
{
    public interface IDBOperate<T> where T : class
    {
        object Add(T entity);
        int Delete(object id);

        int Update(T entity);

        int Update(T entity, string[] columns);

        T GetModelById(object id);
    }
}
