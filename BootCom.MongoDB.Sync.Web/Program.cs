using BootCom.MongoDB.Sync.Web.Hubs;
using BootCom.MongoDB.Sync.Web.Interfaces;
using BootCom.MongoDB.Sync.Web.Models.Configuration;
using BootCom.MongoDB.Sync.Web.Services;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var assembly = Assembly.GetExecutingAssembly();
var assemblyName = assembly.GetName().Name;
var stream = null as Stream;

#if DEBUG
stream = assembly.GetManifestResourceStream($"{assemblyName}.appsettings.development.json");

#else
            stream = assembly.GetManifestResourceStream($"{assemblyName}.appsettings.json");
#endif

var configurationBuilder = builder.Configuration.AddJsonStream(stream!);

var apiConfiguration = configurationBuilder.Build().Get<APIConfiguration>();

// Add services to the container.
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    return new MongoClient(apiConfiguration!.MongoConfigurationSection.Connectionstring);
});

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddScoped<InitialSyncService>();
builder.Services.AddScoped<IAppSyncService, AppSyncService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapHub<UpdateHub>("/hubs/update");

app.Run();

