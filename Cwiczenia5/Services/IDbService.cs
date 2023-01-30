using Cwiczenia5.Models;
using System.Threading.Tasks;

namespace Cwiczenia5.Services
{
    public interface IDbService
    {
        Task<int> RegisterProductInWarehouseAsync(Warehouse warehouse);
        Task<int> RegisterProductInWarehouseWithProcedureAsync(Warehouse warehouse);
    }
}
