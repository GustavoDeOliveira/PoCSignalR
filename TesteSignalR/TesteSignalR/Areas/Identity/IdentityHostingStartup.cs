using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TesteSignalR.Areas.Identity.Data;
using TesteSignalR.Data;

[assembly: HostingStartup(typeof(TesteSignalR.Areas.Identity.IdentityHostingStartup))]
namespace TesteSignalR.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<TesteSignalRContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("TesteSignalRContextConnection")));

                services.AddDefaultIdentity<User>(options => 
                    {
                        if (context.HostingEnvironment.EnvironmentName == "Develompent") {
                            options.Password.RequireDigit = false;
                            options.Password.RequiredLength = 3;
                        }
                        else options.SignIn.RequireConfirmedAccount = true;
                    })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<TesteSignalRContext>();
            });
        }
    }
}