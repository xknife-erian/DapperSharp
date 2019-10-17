using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Overt.User.Application;
using Overt.User.Application.Constracts;
using System;

namespace Overt.Core.DataConsole
{
    class Program
    {
        static IServiceProvider provider;
        static Program()
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(AppContext.BaseDirectory)
                   .AddJsonFile("appsettings.json", true);

            var configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddApplicationDI();

            provider = services.BuildServiceProvider();

        }
        static void Main(string[] args)
        {
            var _userService = provider.GetService<IUserService>();

            //_userService.AddAsync();
            _userService.GetByIds();

            _userService.GetList();
        }
    }
}
