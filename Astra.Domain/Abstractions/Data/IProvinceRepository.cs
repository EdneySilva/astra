namespace Astra.Domain.Abstractions.Data
{
    public interface IProvinceRepository
    {
        Task<Province> FindByNameAsync(string name);

        Task<Province> AddAsync(Province province);
    }
}
