
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Repositories.Contract;
using Talabat.Infrastructure;
using Talabat.Infrastructure.Data;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var webApplicationBuilder = WebApplication.CreateBuilder(args);

            #region Configure Service method from dot net 5 
            // Add services to the container.

            webApplicationBuilder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            webApplicationBuilder.Services.AddEndpointsApiExplorer();
            webApplicationBuilder.Services.AddSwaggerGen();

            webApplicationBuilder.Services.AddDbContext<TalabatStoreContext>(option =>
            {
                option.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
            });

            webApplicationBuilder.Services.AddScoped(typeof(IGenaricRepository<>), typeof(GenericRepository<>));

            webApplicationBuilder.Services.AddAutoMapper(typeof(MappingProfiles));

            #endregion

            webApplicationBuilder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                    .SelectMany(P => P.Value.Errors)
                    .Select(E => E.ErrorMessage)
                    .ToList();
                    var response = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(response);
                };
            });

            var app = webApplicationBuilder.Build();

            using var scop = app.Services.CreateScope();

            var services = scop.ServiceProvider;

            var _dbContext = services.GetRequiredService<TalabatStoreContext>();
            // ask clr for creating object from dbcontext explicitly 

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                await _dbContext.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(_dbContext);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex.StackTrace, "an error has been occured during apply migration");
            }

            #region Configure Kestrel Middleware
            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStatusCodePagesWithReExecute("errors/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.MapControllers();
            #endregion



            app.Run();
        }
    }
}
