using MassTransit.Worker.Data;
using MassTransit.Worker.Data.Entities;
using MassTransit.Worker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MassTransit.Worker.Repositories
{
    internal class NfeRepository : INfeRepository
    {
        private readonly MassTransitDbContext _context;

        public NfeRepository(MassTransitDbContext context)
        {
            _context = context;
        }

        public async Task AddNfeAsync(Nfe nfe)
        {
            await _context.NFes.AddAsync(nfe);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNfeAsync(int id)
        {
            var nfeEntity = await GetNfeByIdAsync(id);
            _context.NFes.Remove(nfeEntity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Nfe>> GetAllNfesAsync()
        {
            return await _context.NFes.ToListAsync();
        }

        public Task<Nfe> GetNfeByIdAsync(int id)
        {
            return _context.NFes.FirstAsync(n => n.Id == id);
        }

        public async Task UpdateNfeAsync(Nfe nfe)
        {
            _context.Update(nfe);
            await _context.SaveChangesAsync();
        }
    }
}
