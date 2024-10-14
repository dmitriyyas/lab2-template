var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Scaffold-DbContext "User ID=program;Password=test;Server=postgres;Port=5432;Database=tickets" Npgsql.EntityFrameworkCore.PostgreSQL
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

app.UseAuthorization();

app.MapControllers();

app.Run();
