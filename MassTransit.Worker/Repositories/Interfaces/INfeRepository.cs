using MassTransit.Worker.Data.Entities;

namespace MassTransit.Worker.Repositories.Interfaces
{
    public interface INfeRepository
    {
        Task<Nfe> GetNfeByIdAsync(int id);
        Task<IEnumerable<Nfe>> GetAllNfesAsync();
        Task AddNfeAsync(Nfe nfe);
        Task UpdateNfeAsync(Nfe nfe);
        Task DeleteNfeAsync(int id);
    }
}
