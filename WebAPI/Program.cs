
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Business.Abstract;
using Business.Concrete;
using Business.DependencyResolvers.Autofac;
using Core.DependencyResolvers;
using Core.Utilities.IoC;
using Core.Utilities.Security.Encryption;
using Core.Utilities.Security.JWT;
using Core.Extensions;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Autofac, Ninject, CastleWindsor, StructureMap, LightInject, DryInject --> IoC Container yokken altyapý sunuyordu
            // Autofac bize AOP imkaný sunuyor. Biz .Net'in IoC Container'ýna Autofac'i injecte edicez.
            // Postsharp
            builder.Services.AddControllers();
            //builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //JWT 
            var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = tokenOptions.Issuer,
                        ValidAudience = tokenOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
                    };
                });

            builder.Services.AddDependencyResolvers(new ICoreModule[]
            {
                new CoreModule(),
            });



            //Autofac
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builderr=>
                {
                    builderr.RegisterModule(new AutofacBusinessModule());   
                });

            //IoC Container
            //builder.Services.AddSingleton<IProductDal, EfProductDal>();
            //builder.Services.AddSingleton<IProductService, ProductManager>();

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

            app.UseAuthentication();


            app.UseAuthorization();
            

            app.MapControllers();

            app.Run();
        }


    }
}
