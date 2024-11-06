using System;
using System.Collections.Generic;
using BonusService.Models;
using Microsoft.EntityFrameworkCore;

namespace BonusService;

public partial class PrivilegesContext : DbContext
{
    public PrivilegesContext()
    {
    }

    public PrivilegesContext(DbContextOptions<PrivilegesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Privilege> Privileges { get; set; }

    public virtual DbSet<PrivilegeHistory> PrivilegeHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Privilege>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("privilege_pkey");

            entity.ToTable("privilege");

            entity.HasIndex(e => e.Username, "privilege_username_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Balance).HasColumnName("balance");
            entity.Property(e => e.Status)
                .HasMaxLength(80)
                .HasDefaultValueSql("'BRONZE'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(80)
                .HasColumnName("username");
        });

        modelBuilder.Entity<PrivilegeHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("privilege_history_pkey");

            entity.ToTable("privilege_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BalanceDiff).HasColumnName("balance_diff");
            entity.Property(e => e.Datetime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("datetime");
            entity.Property(e => e.OperationType)
                .HasMaxLength(20)
                .HasColumnName("operation_type");
            entity.Property(e => e.PrivilegeId).HasColumnName("privilege_id");
            entity.Property(e => e.TicketUid).HasColumnName("ticket_uid");

            entity.HasOne(d => d.Privilege).WithMany(p => p.PrivilegeHistories)
                .HasForeignKey(d => d.PrivilegeId)
                .HasConstraintName("privilege_history_privilege_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
