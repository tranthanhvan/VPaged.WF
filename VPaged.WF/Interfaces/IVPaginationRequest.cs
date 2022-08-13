namespace VPaged.WF.Interfaces
{
    public interface IVPaginationRequest
    {
        int pageIndex { get; set; }
        int pageSize { get; set; }
    }
}