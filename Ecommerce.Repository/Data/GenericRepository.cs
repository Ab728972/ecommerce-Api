using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Specifications;
using System.Linq;

namespace Ecommerce.Repository.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _context; 


public async Task<T> GetEntityWithSpec(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).CountAsync();
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
    }

    public void Add(T entity) => _context.Set<T>().Add(entity);
        public void Update(T entity) => _context.Set<T>().Update(entity);
        public void Delete(T entity) => _context.Set<T>().Remove(entity);
    }
}