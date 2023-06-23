
using CampusWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AccessContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CampusContext")));
            builder.Services.AddDbContext<HouseContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CampusContext")));
            builder.Services.AddDbContext<UserContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CampusContext")));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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

// Da seguire

// https://medium.com/net-core/build-a-restful-web-api-with-asp-net-core-6-30747197e229
// https://learn.microsoft.com/it-it/aspnet/core/tutorials/first-web-api?view=aspnetcore-7.0&tabs=visual-studio

// Creazione API

//Fare clic con il pulsante destro del mouse sulla cartella Controllers.

//    Selezionare Aggiungi>nuovo elemento con scaffolding.

//    Selezionare Controller API con azioni, che usa Entity Framework e quindi selezionare Aggiungi.

//    Nella finestra di dialogo Add API Controller with actions, using Entity Framework (Aggiungi controller API con azioni, che usa Entity Framework):
//        Selezionare Models nella classe Model.
//        Selezionare Context nella classe Contesto dati.
//        Selezionare Aggiungi.

//    Se l'operazione di scaffolding non riesce, selezionare Aggiungi per provare a eseguire lo scaffolding una seconda volta.

