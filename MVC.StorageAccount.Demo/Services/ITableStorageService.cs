using MVC.StorageAccount.Demo.Data;

namespace MVC.StorageAccount.Demo.Services
{
    public interface ITableStorageService
    {
        Task DeleteAttendeeAsync(string key, string id);
        Task<AttendeeEntity> GetAttendeeAsync(string key, string id);
        Task<List<AttendeeEntity>> GetAttendeesAsync();
        Task UpsertAttendeeAsync(AttendeeEntity entity);
    }
}