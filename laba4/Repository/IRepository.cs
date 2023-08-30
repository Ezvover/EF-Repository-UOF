using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laba4.Repository
{
    public interface IRepository<T> where T : class
    {
        T Get(int id);
        IEnumerable<T> GetAll();
        void Add(params T[] entities);
        void Update(T entity);
        void Remove(T entity);
        T GetById(int id);

        List<Goods> GetGoodsFromStoredProcedure();

    }
}
