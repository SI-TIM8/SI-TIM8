using System;
using System.Collections.Generic;
using System.Text;
using LABsistem.Domain.Entities;

namespace LABsistem.Dal.Interfaces
{
    public interface IObjekatRepository
    {
        Task<IEnumerable<Objekat>> GetAllAsync();
        Task<Objekat?> GetByIdAsync(int id);
        Task AddAsync(Objekat objekat);
        Task UpdateAsync(Objekat objekat);
        Task DeleteAsync(int id);
    }
}