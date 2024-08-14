using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TasksService.Models;

public partial class TasksDbContext : DbContext
{
    private IConfiguration _configuration;
    public TasksDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TasksDbContext(DbContextOptions<TasksDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }
    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<WfdefinitionsTempl> WfdefinitionsTempls { get; set; }

    public virtual DbSet<WfedgesTempl> WfedgesTempls { get; set; }

    public virtual DbSet<WfnodesTempl> WfnodesTempls { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
#if RELEASE
        //var connstr = "Host=localhost;Database=TasksDb;Username=postgres;Password=Gfhjkm_123;Persist Security Info=True";
        //optionsBuilder.UseNpgsql(connstr);
        var connstr = _configuration.GetConnectionString("TasksDb");
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("TasksDb")); 
        Console.WriteLine($"++++++optionsBuilder.UseNpgsql({connstr});++++++++");
#else
        optionsBuilder.UseNpgsql(_configuration["ConnectionStrings:TasksDb"]);
#endif
    }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("companies_pkey");

            entity.ToTable("companies");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<WfdefinitionsTempl>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("wfdefinitions_templ_pkey");

            entity.ToTable("wfdefinitions_templ");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");

            entity.HasOne(d => d.Company).WithMany(p => p.WfdefinitionsTempls)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_wfdefinitionstempl_companies");
        });

        modelBuilder.Entity<WfedgesTempl>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("wfedges_templ_pkey");

            entity.ToTable("wfedges_templ");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.NodeFrom).HasColumnName("node_from");
            entity.Property(e => e.NodeTo).HasColumnName("node_to");

            entity.HasOne(d => d.NodeFromNavigation).WithMany(p => p.WfedgesTemplNodeFromNavigations)
                .HasForeignKey(d => d.NodeFrom)
                .HasConstraintName("FK_wfftedgestempl_nodestempl_from");

            entity.HasOne(d => d.NodeToNavigation).WithMany(p => p.WfedgesTemplNodeToNavigations)
                .HasForeignKey(d => d.NodeTo)
                .HasConstraintName("FK_wfftedgestempl_nodestempl_to");
        });

        modelBuilder.Entity<WfnodesTempl>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("wfnodes_templ_pkey");

            entity.ToTable("wfnodes_templ");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DefinitionId).HasColumnName("definition_id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");

            entity.HasOne(d => d.Definition).WithMany(p => p.WfnodesTempls)
                .HasForeignKey(d => d.DefinitionId)
                .HasConstraintName("FK_wfnodestempl_wfdefinitionstempl");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
