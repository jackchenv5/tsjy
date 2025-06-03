using Faoem.FacilityStatus.Models;

namespace Faoem.FacilityStatus.Services.StatusBindingService;

public interface IStatusBindingService
{
    public Task<List<StatusBinding>> GetStatusBindingsAsync(long facilityId);
    public Task<List<StatusBinding>> GetStatusBindingsAsync();
    public Task UpdateStatusBindingAsync(StatusBinding statusBinding);
}