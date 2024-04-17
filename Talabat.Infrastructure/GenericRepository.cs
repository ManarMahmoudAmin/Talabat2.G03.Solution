using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;
using Talabat.Infrastructure.Data;

namespace Talabat.Infrastructure
{
   public class GenericRepository<T> : IGenaricRepository<T> where T : BaseEntity
    {
        private readonly TalabatStoreContext _dbContext;

        public GenericRepository(TalabatStoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T?> GetAsync(int id)
        {
            return await _dbContext.Set<Product>().Where(p => p.Id == id).AsNoTracking().Include(p => p.Brand).Include(p => p.Category).FirstOrDefaultAsync() as T;
            //return await dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecifications(spec).AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecifications(spec).FirstOrDefaultAsync();
        }

        private IQueryable<T> ApplySpecifications(ISpecifications<T> spec)
            => SpecificationsEvalutor<T>.GetQuery(_dbContext.Set<T>(), spec);
    }
}
