using System;
using com.b_velop.stack.DataContext;
using com.b_velop.stack.DataContext.Abstract;
using com.b_velop.Stack.Air.Server.Bl;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Stack.Air.Server
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(
            IServiceCollection services)
        {
            var server = Environment.GetEnvironmentVariable("SERVER");
            var password = Environment.GetEnvironmentVariable("PASSWORD");
            var dbName = Environment.GetEnvironmentVariable("DATABASE");
            var user = Environment.GetEnvironmentVariable("USER");

            var connectionString = $"Server={server},1433;Database={dbName};User Id={user};Password={password}";
#if DEBUG
            connectionString = "Server=localhost,1433;Database=Measure;User Id=sa;Password=foo123bar!";
#endif

            services.AddDbContext<MeasureContext>(options =>
            {
                options.EnableDetailedErrors(Env.IsDevelopment());
                options.EnableSensitiveDataLogging(Env.IsDevelopment());
                options.UseSqlServer(connectionString);
            });

            services.AddControllers();

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddStackRepositories();
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
