using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreDbContext _context;

        public GenericRepository(StoreDbContext context)
        {
            _context = context;
        }
        public void Add(T Entity)
            => _context.Set<T>().Add(Entity);

        public void Delete(T Entity)
            => _context.Set<T>().Remove(Entity);


        public async Task<T> GetByIdAsync(int id)
            =>await _context.Set<T>().FindAsync(id);

        public async Task<T> GetEntityWithSpecifications(ISpecifications<T> specifications)
            => await ApplySpecifications(specifications).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<T>> GetListAsync(ISpecifications<T> specifications)
            => await ApplySpecifications(specifications).ToListAsync();

            public async Task<IReadOnlyList<T>> ListAllAsync()
            => await _context.Set<T>().ToListAsync();

        public void Update(T Entity)
             => _context.Set<T>().Update(Entity);

        private IQueryable<T> ApplySpecifications(ISpecifications<T> specifications)
            => SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), specifications);
    }
}
