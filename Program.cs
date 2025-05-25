

using Microsoft.EntityFrameworkCore;
using ClaimsSystems_DAL.Models;
using ClaimsSystems_DAL;


namespace ClaimsSystems_WebServiceLayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            //builder.Services.AddTransient<ClaimsSystemsDbContext>();
            //builder.Services.AddTransient<CustomerRepository>(
            //    c => new CustomerRepository(c.GetRequiredService<ClaimsSystemsDbContext>()));

            builder.Services.AddDbContext<ClaimsSystemsDbContext>();
            builder.Services.AddScoped<CustomerRepository>();
            builder.Services.AddScoped<InsuranceRepository>();
            builder.Services.AddScoped<ScoreTeamRepository>();
            builder.Services.AddScoped<PaymentsRepository>();
            builder.Services.AddScoped<ClaimHistoryRepository>();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
