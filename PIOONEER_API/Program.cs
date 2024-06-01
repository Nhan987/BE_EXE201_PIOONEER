using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PIOONEER_Model.Mapper;
using PIOONEER_Repository.Entity;
using PIOONEER_Repository.Repository;
using Tools;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<Firebases>();
var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperProfile());
});
builder.Services.AddSingleton<IMapper>(config.CreateMapper());

var serverVersion = new MySqlServerVersion(new Version(8, 0, 23)); // Replace with your actual MySQL server version
builder.Services.AddDbContext<MyDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("MyDB");
    options.UseMySql(connectionString, serverVersion, options => options.MigrationsAssembly("PIOONEER_API"));
}
);
builder.Services.AddControllers();

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
