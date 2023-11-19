using SyslogAssignmentProject.Services;
using SyslogAssignmentProject.Classes;
using MudBlazor.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddSingleton<RadioInjection>();
builder.Services.AddSingleton<GlobalInjection>();
//builder.Services.AddSingleton<TcpSyslogReceiver>();
//builder.Services.AddSingleton<UdpSyslogReceiver>();
builder.Services.AddSingleton<BackgroundRunner>();
builder.Services.AddSingleton<RadioListServicer>();
builder.Services.AddSingleton<ListServicer>();

WebApplication app = builder.Build();

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

BackgroundRunner _backgroundRunner = app.Services.GetRequiredService<BackgroundRunner>();

app.Run();

/* Tasks that need to be done to fulfil requirements on this branch.
 * Radio page needs to fulfil requirements, needs to display protocol, ip, ip type, potentially port (see what Sam says)
 * Hiding radios on this version
 * Deleting Radios on this version
 * Suppressing warnings (unsure how)
 * Adding log export button to each radio page (back end in place buttons needed)
 * Potentially adding local delete on each radio page (back end needs to be added for this).
 * Error occurs when more than one radio is added, live feed messages for that radio does not seem to be displayed. Fix unneeeded as likely due to how messages for that radio
 * are being processed which will change.
 * Define what a radio is.
 * Make a ref when creating a tcp and udp listener, those will signify if both ports for udp and tcp work, if not
   output the bad port messsage in background runner and reset listener values to default then create new instance
   of listeners.*/