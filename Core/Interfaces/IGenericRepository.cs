﻿using Core.Entities;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IGenericRepository<T>  where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> GetEntityWithSpecifications(ISpecifications<T> specifications);
        Task<IReadOnlyList<T>> GetListAsync(ISpecifications<T> specifications);
        Task<int> CountAsync(ISpecifications<T> specifications);
        void Add(T Entity);
        void Update(T Entity);
        void Delete(T Entity);
    }
}
