using System;
using System.Collections.Generic;
using FlightService.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightService;

public partial class FlightsContext : DbContext
{
    public FlightsContext()
    {
    }

    public FlightsContext(DbContextOptions<FlightsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("airport_pkey");

            entity.ToTable("airport");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(255)
                .HasColumnName("country");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("flight_pkey");

            entity.ToTable("flight");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Datetime).HasColumnName("datetime");
            entity.Property(e => e.FlightNumber)
                .HasMaxLength(20)
                .HasColumnName("flight_number");
            entity.Property(e => e.FromAirportId).HasColumnName("from_airport_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ToAirportId).HasColumnName("to_airport_id");

            entity.HasOne(d => d.FromAirport).WithMany(p => p.FlightFromAirports)
                .HasForeignKey(d => d.FromAirportId)
                .HasConstraintName("flight_from_airport_id_fkey");

            entity.HasOne(d => d.ToAirport).WithMany(p => p.FlightToAirports)
                .HasForeignKey(d => d.ToAirportId)
                .HasConstraintName("flight_to_airport_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
