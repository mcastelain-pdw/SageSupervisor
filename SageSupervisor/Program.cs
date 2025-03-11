using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using SageSupervisor.Components;
using SageSupervisor.Models;
using SageSupervisor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();

// Enregistrer le service Service Broker en tant que service d'arrière-plan
builder.Services.AddDbContextFactory<DataContext>(opt => 
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ProdwareConnection"))
    .EnableSensitiveDataLogging());
builder.Services.AddSingleton<ServiceBrokerMonitor>(new ServiceBrokerMonitor(builder.Configuration.GetConnectionString("DefaultConnection")!));
builder.Services.AddSingleton<ServiceBrokerService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<ServiceBrokerService>());
builder.Services.AddDataGridEntityFrameworkAdapter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
