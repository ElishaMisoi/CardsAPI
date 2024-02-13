namespace Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string GetCurrentUserId();
        string GetCurrentUserRole();
    }
}