using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Azure;
using MVC.StorageAccount.Demo.Configurations;
using MVC.StorageAccount.Demo.Services;

var builder = WebApplication.CreateBuilder(args);

var appSetting = new AppSettings();

builder.Configuration.Bind(appSetting);
builder.Services.AddSingleton(appSetting);
builder.Services.Configure<AccountStorageSettings>(builder.Configuration.GetSection("AccountStorageSettings"));

builder.Services.AddAzureClients(builder =>
{
    builder.AddBlobServiceClient(appSetting?.AccountStorageSettings?.StorageAccountConnetionString);
    //builder.AddQueueServiceClient(appSetting?.AccountStorageSettings?.StorageAccountConnetionString).ConfigureOptions(c => { c.MessageEncoding = QueueMessageEncoding.Base64; });
    //builder.AddTableServiceClient(appSetting?.AccountStorageSettings?.StorageAccountConnetionString);
});

builder.Services.AddAzureClients(b =>
{
    b.AddClient<QueueClient, QueueClientOptions>((_, _, _) =>
    {
        return new QueueClient(appSetting?.AccountStorageSettings?.StorageAccountConnetionString, appSetting?.AccountStorageSettings?.QueueName, new QueueClientOptions
        {
            MessageEncoding = QueueMessageEncoding.Base64,
        });

    });

    b.AddClient<TableClient, TableClientOptions>((_, _, _) =>
    {
        return new TableClient(appSetting?.AccountStorageSettings?.StorageAccountConnetionString, appSetting?.AccountStorageSettings?.TableName);
    });
});



builder.Services.AddScoped<ITableStorageService, TableStorageService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IQueueService, QueueService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
