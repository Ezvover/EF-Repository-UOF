using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laba4.Repository
{
    public class UnitOfWork : IDisposable
    {
        private readonly DbContext _context;

        public IRepository<Categories> CategoryRepository { get; }
        public IRepository<Goods> GoodsRepository { get; }

        public UnitOfWork(DbContext context)
        {
            _context = context;
            CategoryRepository = new Repository<Categories>(_context);
            GoodsRepository = new Repository<Goods>(_context);
        }
        public IRepository<T> Repository<T>() where T : class
        {
            return new Repository<T>(_context);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }


}
