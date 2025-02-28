using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using MVC.StorageAccount.Demo.Configurations;
using MVC.StorageAccount.Demo.Data;

namespace MVC.StorageAccount.Demo.Services;

public class TableStorageService : ITableStorageService
{
    private readonly TableClient _tableClient;
    public TableStorageService(TableClient tableClient)
    {
        _tableClient = tableClient;
    }

    public async Task<AttendeeEntity> GetAttendeeAsync(string key, string id)
    {
        await EnsureTableExistsAsync();
        return await _tableClient.GetEntityAsync<AttendeeEntity>(key, id);
    }

    public async Task<List<AttendeeEntity>> GetAttendeesAsync()
    {
        await EnsureTableExistsAsync();
        Pageable<AttendeeEntity> attendeeEntities = _tableClient.Query<AttendeeEntity>();
        return attendeeEntities.ToList();
    }

    public async Task UpsertAttendeeAsync(AttendeeEntity entity)
    {
        await EnsureTableExistsAsync();
        await _tableClient.UpsertEntityAsync<AttendeeEntity>(entity);
    }

    public async Task DeleteAttendeeAsync(string key, string id)
    {
        await EnsureTableExistsAsync();
        await _tableClient.DeleteEntityAsync(key, id);
    }

    private async Task EnsureTableExistsAsync() => await _tableClient.CreateIfNotExistsAsync();
}
