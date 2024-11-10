using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TasksService.DataAccess.Entities;

namespace TasksService.DataAccess.EntityFramework;

public partial class TasksDbContext : DbContext
{
    private IConfiguration _configuration;
    public virtual DbSet<TasksCompany> Companies { get; set; }
    public virtual DbSet<CompanyEmployee> CompanyEmployees { get; set; }
    public virtual DbSet<CompanyProject> CompanyProjects { get; set; }
    public virtual DbSet<ProjectArea> ProjectAreas { get; set; }
    public virtual DbSet<Entities.Task> Tasks { get; set; }
    public virtual DbSet<TaskAction> TaskActions { get; set; }
    public virtual DbSet<TaskDoer> TaskDoers { get; set; }
    public virtual DbSet<TaskEdge> TaskEdges { get; set; }
    public virtual DbSet<TaskHistory> TaskHistories { get; set; }
    public virtual DbSet<TaskNode> TaskNodes { get; set; }
    public virtual DbSet<Urgency> Urgencies { get; set; }

    public virtual DbSet<WfDefinitionsTemplate> WfdefinitionsTempls { get; set; }
    public virtual DbSet<WfEdgesTemplate> WfedgesTempls { get; set; }
    public virtual DbSet<WfNodesTemplate> WfnodesTempls { get; set; }

    public IConfiguration Configuration => _configuration;

    public TasksDbContext(IConfiguration configuration)
        :base()
    {
        _configuration = configuration;
    }

    public TasksDbContext(DbContextOptions<TasksDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
#if DEBUG
        var connstr = _configuration.GetConnectionString("TasksDb");
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("TasksDb")); 
        Console.WriteLine($"++++++optionsBuilder.UseNpgsql({connstr});++++++++");
#else
        optionsBuilder.UseNpgsql(_configuration["ConnectionStrings:TasksDb"]);
#endif
    }
      
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TasksCompany>(entity =>
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

        modelBuilder.Entity<WfDefinitionsTemplate>(entity =>
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
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_wfdefinitionstempl_companies");
        });

        modelBuilder.Entity<WfEdgesTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("wfedges_templ_pkey");

            entity.ToTable("wfedges_templ");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
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

        modelBuilder.Entity<WfNodesTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("wfnodes_templ_pkey");

            entity.ToTable("wfnodes_templ");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DefinitionId).HasColumnName("definition_id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.IconId).HasColumnName("icon_id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Terminating).HasColumnName("terminating");

            entity.HasOne(d => d.Definition).WithMany(p => p.WfnodesTempls)
                .HasForeignKey(d => d.DefinitionId)
                .HasConstraintName("FK_wfnodestempl_wfdefinitionstempl");
        });

        modelBuilder.Entity<CompanyEmployee>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("company_employees");

            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");

            entity.HasOne(d => d.Company).WithMany()
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_companyemployees_companies");
        });

        modelBuilder.Entity<CompanyProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("company_projects_pkey");

            entity.ToTable("company_projects");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyProjects)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_company_projects_companies");
        });

        modelBuilder.Entity<ProjectArea>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("project_areas_pkey");

            entity.ToTable("project_areas");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectAreas)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_project_areas_project");
        });

        modelBuilder.Entity<Entities.Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tasks_pkey");

            entity.ToTable("tasks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AreaId).HasColumnName("area_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("creation_date");
            entity.Property(e => e.CreatorId).HasColumnName("creator_id");
            entity.Property(e => e.CurrentNode).HasColumnName("current_node");
            entity.Property(e => e.DeadlineDate).HasColumnName("deadline_date");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.RowVersion).HasColumnName("row_version");
            entity.Property(e => e.TemplateId).HasColumnName("template_id");
            entity.Property(e => e.Urgency).HasColumnName("urgency");

            entity.HasOne(d => d.Area).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.AreaId)
                .HasConstraintName("FK_Tasks_project_areas");

            entity.HasOne(d => d.Company).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tasks_companies");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_Tasks_projects");

            entity.HasOne(d => d.Template).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.TemplateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tasks_wfdefinitions_templ_fk");

            entity.HasOne(d => d.UrgencyNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.Urgency)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tasks_urgencies");
        });


        modelBuilder.Entity<TaskAction>(entity =>
        {
            entity.HasKey(e => e.ActionId).HasName("task_actions_pk");

            entity.ToTable("task_actions");

            entity.Property(e => e.ActionId)
                .ValueGeneratedNever()
                .HasColumnName("action_id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.FieldType).HasColumnName("field_type");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });


        modelBuilder.Entity<TaskDoer>(entity =>
        {
            entity.HasKey(e => new { e.NodeId, e.EmpoyeeId }).HasName("task_doers_pk");

            entity.ToTable("task_doers");

            entity.Property(e => e.NodeId).HasColumnName("node_id");
            entity.Property(e => e.EmpoyeeId).HasColumnName("empoyee_id");

            entity.HasOne(d => d.Node).WithMany(p => p.TaskDoers)
                .HasForeignKey(d => d.NodeId)
                .HasConstraintName("task_doers_task_nodes_fk");
        });


        modelBuilder.Entity<TaskEdge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("task_transitions_pkey");

            entity.ToTable("task_edges");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnType("character varying");
            entity.Property(e => e.NodeFrom).HasColumnName("node_from");
            entity.Property(e => e.NodeTo).HasColumnName("node_to");

            entity.HasOne(d => d.NodeFromNavigation).WithMany(p => p.TaskEdgeNodeFromNavigations)
                .HasForeignKey(d => d.NodeFrom)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tasktransitions_nodes_from");

            entity.HasOne(d => d.NodeToNavigation).WithMany(p => p.TaskEdgeNodeToNavigations)
                .HasForeignKey(d => d.NodeTo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_task_transitions_nodes_to");
        });

        modelBuilder.Entity<TaskHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("task_history");

            entity.Property(e => e.ActionDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("action_date");
            entity.Property(e => e.ActionId).HasColumnName("action_id");
            entity.Property(e => e.ActionValue)
                .HasColumnType("character varying")
                .HasColumnName("action_value");
            entity.Property(e => e.NodeId).HasColumnName("node_id");
            entity.Property(e => e.OldValue)
                .HasColumnType("character varying")
                .HasColumnName("old_value");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Action).WithMany()
                .HasForeignKey(d => d.ActionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("task_history_task_actions_fk");

            entity.HasOne(d => d.Task).WithMany()
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("task_history_tasks_fk");
        });

        modelBuilder.Entity<TaskNode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("task_nodes_pk");

            entity.ToTable("task_nodes");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.IconId).HasColumnName("icon_id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.Terminating).HasColumnName("terminating");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.TaskNode)
                .HasForeignKey<TaskNode>(d => d.Id)
                .HasConstraintName("task_nodes_tasks_fk");
        });

        modelBuilder.Entity<Urgency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("urgencies_pkey");

            entity.ToTable("urgencies");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
