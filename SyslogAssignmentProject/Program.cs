using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SyslogAssignmentProject.Data;
using Syncfusion.Blazor;
using SyslogAssignmentProject.Services;
using SyslogAssignmentProject.Classes;

var builder = WebApplication.CreateBuilder(args);
BackgroundRunner _listenerController = new BackgroundRunner();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSyncfusionBlazor();

builder.Services.AddSingleton<ListServicer>();
builder.Services.AddScoped<ListServicer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();