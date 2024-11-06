using FlightService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
/*
INSERT INTO public.airport(id, name, city, country) VALUES 
(1, 'Шереметьево', 'Москва', 'Россия'),
(2, 'Пулково', 'Санкт-Петербург', 'Россия');

INSERT INTO public.flight(
	id, flight_number, datetime, from_airport_id, to_airport_id, price)
	VALUES (1, "AFL031", "2021-10-08 20:00", 2, 1, 1500);
*/

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FlightsContext>(opt =>
            opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")),
                ServiceLifetime.Transient, ServiceLifetime.Transient);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseAuthorization();

app.MapControllers();

app.Run();
