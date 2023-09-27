using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ABPTestProject.Models;

public partial class AbptestTaskDbContext : DbContext
{
    public AbptestTaskDbContext()
    {
    }

    public AbptestTaskDbContext(DbContextOptions<AbptestTaskDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SiteVisitor> SiteVisitors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DENISKRAVCHENKO\\SQLEXPRESS;Database=ABPTestTaskDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SiteVisitor>(entity =>
        {
            entity.HasKey(e => e.DeviceToken).HasName("PK__SiteVisi__5A171AE3B633681C");

            entity.Property(e => e.DeviceToken)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Device_token");
            entity.Property(e => e.ButtonColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Button_color");
            entity.Property(e => e.Price).HasColumnType("money");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
