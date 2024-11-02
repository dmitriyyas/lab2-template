using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TicketService.Models;

namespace TicketService;

public partial class TicketsContext : DbContext
{
    public TicketsContext()
    {
    }

    public TicketsContext(DbContextOptions<TicketsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ticket_pkey");

            entity.ToTable("ticket");

            entity.HasIndex(e => e.TicketUid, "ticket_ticket_uid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FlightNumber)
                .HasMaxLength(20)
                .HasColumnName("flight_number");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.TicketUid).HasColumnName("ticket_uid");
            entity.Property(e => e.Username)
                .HasMaxLength(80)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
