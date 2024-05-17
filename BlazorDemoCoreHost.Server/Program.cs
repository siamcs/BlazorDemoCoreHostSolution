using BlazorDemoCoreHost.Server.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>
  (op => op.UseSqlServer(builder.Configuration.GetConnectionString("con")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}
app.UseAuthorization();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
