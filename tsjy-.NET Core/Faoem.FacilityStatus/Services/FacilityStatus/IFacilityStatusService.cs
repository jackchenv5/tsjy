using Faoem.Common.Dtos;
using Faoem.FacilityStatus.Dtos;
using Faoem.FacilityStatus.Inputs;

namespace Faoem.FacilityStatus.Services.FacilityStatus;

public interface IFacilityStatusService
{
    /// <summary>
    /// 获取当前班组指定设备的状态数据
    /// </summary>
    /// <param name="facilityId"></param>
    /// <returns></returns>
    public Task<StatusDto> GetCurrentShiftStatusAsync(long facilityId);

    /// <summary>
    /// 获取指定设备在一段时间内指定页数的状态数据
    /// </summary>
    /// <param name="statusQuery"></param>
    /// <returns></returns>
    public Task<PagedDto<StatusRecordDto>> GetStatusAsync(StatusQueryInput statusQuery);

    /// <summary>
    /// 获取指定设备在一段时间内的状态数据
    /// </summary>
    /// <param name="statusQuery"></param>
    /// <returns></returns>
    public Task<List<StatusRecordDto>> GetAllStatusAsync(StatusQueryInput statusQuery);
}