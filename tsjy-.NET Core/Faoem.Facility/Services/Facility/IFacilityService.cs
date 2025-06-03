namespace Faoem.Facility.Services.Facility;

public interface IFacilityService
{
    public Task<List<Models.Facility>> GetFacilitiesAsync();
    public Task<Models.Facility> GetFacilityAsync(long id);
    public Task<Models.Facility> AddFacilityAsync(Models.Facility facility);
    public Task UpdateFacilityAsync(Models.Facility facility);
    public Task DeleteFacilityAsync(long id);
}