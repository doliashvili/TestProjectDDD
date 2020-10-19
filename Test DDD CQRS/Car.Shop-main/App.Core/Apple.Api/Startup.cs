using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using App.Core;
using App.Core.MessageBroker;
using App.Core.Repository;
using App.Infrastructure;
using App.Infrastructure.MongoDbImplementation;
using App.Infrastructure.RabbitMq;
using App.Infrastructure.RavenDbImplementation;
using Apple.Application.CommandHandlers;
using Apple.Application.EventHandlers;
using Apple.ReadModels.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Apple.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGenNewtonsoftSupport();
            services.AddSwaggerGen(o =>
            {
                var appXmlName = AppDomain.CurrentDomain.FriendlyName + ".xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, appXmlName);
                o.IncludeXmlComments(xmlPath);
            });
        
            var rabbitConfig = new RabbitMqConfig()
            {
                UserName = "guest",
                Password = "guest",
                Host = "localhost",
                Port = 5672,
                VirtualHost = "/",
                Exchange = "test"
            };
            services.AddSingleton(rabbitConfig);
            services.AddSingleton<IRabbitConnectionWrapper, RabbitConnectionWrapper>();
            services.AddRabbitMq(typeof(AppleCreatedEventHandler).Assembly);

            services.AddCommandHandlers(typeof(AppleCommandHandlers).Assembly);
            services.AddInternalEventHandlers(typeof(AppleCommandHandlers).Assembly,
                typeof(AppleReadModel).Assembly);
            services.AddInternalEventPublisher();
            services.AddCommandSender();

            var eventStoreConfig = new EventStoreConfig();
            Configuration.GetSection(nameof(EventStoreConfig)).Bind(eventStoreConfig);
            var mongoConfig = new MongoConfig();
            Configuration.GetSection(nameof(MongoConfig)).Bind(mongoConfig);

            services.AddSingleton<IEventProducer, RabbitMqProducer>();
            services.AddEventStoreConfig(eventStoreConfig);
            services.AddMongoDb(mongoConfig);
            services.ConfigureMongoEventStore();
            services.ConfigureRepositories();
            
            var ravenSettings = new RavenSettings();
            Configuration.GetSection(nameof(RavenSettings)).Bind(ravenSettings);
            services.AddSingleton(ravenSettings);
            services.AddRavenDb(ravenSettings);
            services.AddQueryHandlers(typeof(AppleReadModel).Assembly);
            services.AddQueryProcessor();

            services
                .AddTransient<IReadModelRepository<AppleReadModel, string>,
                    RavenReadModelRepository<AppleReadModel, string>>();

            services.AddControllers().AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            });
        }
        
        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var consumers = app.ApplicationServices.GetServices<IMessageConsumer>().Distinct();
            var subscriber = app.ApplicationServices.GetService<ISubscriber>();

            foreach (var consumer in consumers)
            {
                var t = consumer.GetType();
                subscriber.Subscribe(consumer);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint("/swagger/v1/swagger.json", "Apple Api");
                o.RoutePrefix = string.Empty;
            });
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}