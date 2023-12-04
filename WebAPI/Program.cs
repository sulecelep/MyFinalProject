
using Business.Abstract;
using Business.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Autofac, Ninject, CastleWindsor, StructureMap, LightInject, DryInject --> IoC Container yokken altyapý sunuyordu
            // Autofac bize AOP imkaný sunuyor. Biz .Net'in IoC Container'ýna Autofac'i injecte edicez.
            builder.Services.AddControllers();

            //IoC Container
            builder.Services.AddSingleton<IProductDal, EfProductDal>();
            builder.Services.AddSingleton<IProductService, ProductManager>();
            
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
