using SyslogAssignmentProject.Services;
using SyslogAssignmentProject.Classes;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddSingleton<RadioInjection>();
builder.Services.AddSingleton<GlobalInjection>();
builder.Services.AddSingleton<TcpSyslogReceiver>();
builder.Services.AddSingleton<UdpSyslogReceiver>();
builder.Services.AddSingleton<BackgroundRunner>();
builder.Services.AddSingleton<RadioListServicer>();
builder.Services.AddSingleton<ListServicer>();

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

BackgroundRunner backgroundRunner = app.Services.GetRequiredService<BackgroundRunner>();

app.Run();