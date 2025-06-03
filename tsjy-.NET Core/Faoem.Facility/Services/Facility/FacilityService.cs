using Faoem.Common.Exceptions;
using Faoem.Facility.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Faoem.Facility.Services.Facility;

internal class FacilityService(FacilityDbContext facilityDbContext) : IFacilityService
{
    public async Task<List<Models.Facility>> GetFacilitiesAsync()
    {
        return await facilityDbContext.Facilities.ToListAsync();
    }

    public async Task<Models.Facility> GetFacilityAsync(long id)
    {
        var facility = await facilityDbContext.Facilities.FindAsync(id);
        if (facility is null)
        {
            throw new AppException("The facility does not exist.", 404);
        }

        return facility;
    }

    public async Task<Models.Facility> AddFacilityAsync(Models.Facility facility)
    {
        var exist = await facilityDbContext.Facilities.FirstOrDefaultAsync(model =>
            facility.LowerName == model.LowerName);

        if (exist is not null)
        {
            throw new AppException("The facility already exists.", 400);
        }

        await facilityDbContext.Facilities.AddAsync(facility);
        await facilityDbContext.SaveChangesAsync();

        return facility;
    }

    public async Task UpdateFacilityAsync(Models.Facility facility)
    {
        var exist = await facilityDbContext.Facilities.FindAsync(facility.Id);
        if (exist is null)
        {
            throw new AppException("The facility does not exist.", 404);
        }

        exist.Name = facility.Name;
        exist.IsEnabled = facility.IsEnabled;
        exist.SerialNumber = facility.SerialNumber;
        exist.Description = facility.Description;

        await facilityDbContext.SaveChangesAsync();
    }

    public async Task DeleteFacilityAsync(long id)
    {
        var exist = await facilityDbContext.Facilities.FindAsync(id);
        if (exist is null)
        {
            throw new AppException("The facility does not exist.", 404);
        }

        facilityDbContext.Facilities.Remove(exist);
        await facilityDbContext.SaveChangesAsync();
    }
}