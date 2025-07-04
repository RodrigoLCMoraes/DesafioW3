
using ApiCartao.Services;
using ApiCartao.Settings;

namespace ApiCartao
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<MongoDBSettings>(
            builder.Configuration.GetSection("MongoDBSettings"));

            builder.Services.AddSingleton<CartaoService>();


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseSwagger();

            app.UseSwaggerUI();


            app.MapControllers();

            app.Run();
        }
    }
}
