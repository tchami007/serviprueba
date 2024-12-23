using Microsoft.Extensions.Options;
using ServiPrueba.Application.Services;
using ServiPrueba.Infraestructure.Configurations;
using ServiPrueba.Infraestructure.Events;
using ServiPrueba.Infraestructure.Externals.ApiClientCore;
using ServiPrueba.Infraestructure.Externals.Interfaces;
using ServiPrueba.Shared.Log;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "ServiPruebas - Aplicacion para pruebas de integracion de Apis de terceros",
        Description = "Aplicacion para implementar integraciones de varios proveedores",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Cesar Romano (tchami007)",
            Email = "cesarromano2007@gmail.com",
            Url = new Uri("https://github.com/tchami007")
        }
    });

    // Opcional: Agregar comentarios de código
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});


// Cargar configuración de appsettings.json
builder.Services.Configure<ApiClientConfig>(builder.Configuration.GetSection("ApiClients"));


// Nueva Configuracion x Event Bus
var rabbitMQConfig = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQConfiguration>();

//----------------------------------------------

// Registrar Dependencias Services

builder.Services.AddScoped<ITransferenciaService, TransferenciaService>();

// Registrar Dependencias Rabbit

builder.Services.AddSingleton(rabbitMQConfig);
builder.Services.AddSingleton<RabbitMQProducer>();

// Registrar Dependencias Publisher

builder.Services.AddScoped<IMessagePublisher, MessagePublisher>();

// Registrar HttpClient y clientes API

builder.Services.AddHttpClient<ICreditoApiClient, CreditoApiClient>()
    .ConfigureHttpClient((sp, client) =>
    {
        var config = sp.GetRequiredService<IOptions<ApiClientConfig>>().Value;
        client.BaseAddress = new Uri(config.BaseUrlCore);
    });

builder.Services.AddHttpClient<IDebitoApiClient, DebitoApiClient>()
    .ConfigureHttpClient((sp, client) =>
    {
        var config = sp.GetRequiredService<IOptions<ApiClientConfig>>().Value;
        client.BaseAddress = new Uri(config.BaseUrlCore);
    });

//----------------------------------------------

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
