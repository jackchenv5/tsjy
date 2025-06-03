namespace Faoem.Common.Dtos;

public class PagedDto<T>
{
    public long Total { get; set; }
    public List<T> Items { get; set; } = [];

    public PagedDto()
    {
    }

    public PagedDto(long total, List<T> items)
    {
        Total = total;
        Items = items;
    }
}