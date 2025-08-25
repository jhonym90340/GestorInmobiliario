using GestorInmobiliario.Backend.Interfaces;
using GestorInmobiliario.Backend.Repositories;
using GestorInmobiliario.Backend.Settings;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using GestorInmobiliario.Backend.Services;
using GestorInmobiliario.Backend.Settings;




var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IOwnerImageRepository, OwnerImageRepository>();

var ownerImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "owners");
if (!Directory.Exists(ownerImagePath))
{
    Directory.CreateDirectory(ownerImagePath);
}

builder.Services.Configure<ImageSettings>(
    builder.Configuration.GetSection("ImageSettings"));

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));

// Registrar servicios
builder.Services.AddScoped<ImageSettings>();


// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllLocalhostOrigins",
        builder => builder.WithOrigins(
                                "http://localhost:3000", // Puerto común de React/Vite dev
                                "https://localhost:3000",
                                "http://localhost:5173", //  puerto HTTP de frontend
                                "https://localhost:5173", // Aunque se este en HTTP, es buena práctica tenerlo
                                "http://localhost:5000", // Puertos comunes de .NET si no se usa 50000/50001
                                
                                "http://localhost:50000" //  puerto HTTP de backend
                              
                            )
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});

// 1. Configurar y registrar las opciones de MongoDB (MongoDBSettings)
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));

// 2. Registrar IMongoClient como un singleton.
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionURI);
});

// 3. Registrar IMongoDatabase como un singleton.
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return client.GetDatabase(settings.DatabaseName);
});

// 4. Registrar  repositorios existentes con inyección de dependencia.
builder.Services.AddSingleton<IOwnerRepository, OwnerRepository>();
builder.Services.AddSingleton<IPropertyRepository, PropertyRepository>();
builder.Services.AddSingleton<IPropertyTraceRepository, PropertyTraceRepository>();
// ¡Registra el nuevo repositorio de imágenes!
builder.Services.AddSingleton<IPropertyImageRepository, PropertyImageRepository>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IImageService, ImageService>();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:5174")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});


var app = builder.Build();


app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/*
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
*/


//app.UseHttpsRedirection();

// ¡IMPORTANTE! Habilitar el uso de archivos estáticos.
app.UseStaticFiles();

// Aplicar la política CORS
app.UseCors("AllowAllLocalhostOrigins");


app.UseAuthorization();

app.MapControllers();

app.Run();