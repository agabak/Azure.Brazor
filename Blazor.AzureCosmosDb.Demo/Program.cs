using Blazor.AzureCosmosDb.Demo.Configurations;
using Blazor.AzureCosmosDb.Demo.Data;
using Blazor.AzureCosmosDb.Demo.Documents.Configurations;

var builder = WebApplication.CreateBuilder(args);
var appSetting = new AppSettings();

builder.Configuration.Bind(appSetting);
builder.Configuration.GetSection("TS").Bind(appSetting);
builder.Services.AddSingleton(appSetting);

builder.Services.Configure<CosmosDbSettings>(builder.Configuration.GetSection("TS:CosmosDbSettings"));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IEngineerService, EngineerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
