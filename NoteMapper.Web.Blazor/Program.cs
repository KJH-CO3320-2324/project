using BlazorStrap;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using NoteMapper.Core.IO;
using NoteMapper.Infrastructure;
using NoteMapper.Services.Users;
using NoteMapper.Services.Web;
using NoteMapper.Web.Blazor.Services;
using NoteMapper.Web.Blazor.Services.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IServiceCollection services = builder.Services;
services.AddDefaultIdentity<IdentityUser>();
services.AddRazorPages();
services.AddServerSideBlazor();
services.AddBlazorStrap();

IDependencyContainer container = new DependencyContainer(services)
    .AddScoped<IFilePathResolver, FilePathResolver>()
    .AddScoped<IUrlEncoder, UrlEncoder>()
    .AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>()
    .AddScoped<IUserLocator, UserLocator>();
DependencyConfig.RegisterDependencies(container, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{    
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapRazorPages();
app.MapFallbackToPage("/_Host");

app.Run();
