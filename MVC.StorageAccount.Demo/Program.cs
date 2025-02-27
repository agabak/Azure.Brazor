using MVC.StorageAccount.Demo.Configurations;
using MVC.StorageAccount.Demo.Services;

var builder = WebApplication.CreateBuilder(args);

var appSetting = new AppSettings();

builder.Configuration.Bind(appSetting);
builder.Services.AddSingleton(appSetting);
builder.Services.Configure<AccountStorageSettings>(builder.Configuration.GetSection("AccountStorageSettings"));

builder.Services.AddScoped<ITableStorageService, TableStorageService>();

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
