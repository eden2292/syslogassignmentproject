using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SyslogAssignmentProject.Data;
using Syncfusion.Blazor;
using SyslogAssignmentProject.Services;
using SyslogAssignmentProject.Classes;

// Testing stuff for regex, delete this before merging w/ master branch

SyslogMessage testMessage = new SyslogMessage("192.168.1.5", DateTimeOffset.Now, "<2>1 2023-10-26T09:55:13.578Z Park Air Systems LTD, test app. - - - - V4ZOM86", "UDP");
testMessage.ParseMessage();
testMessage = null;

// End of testing

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