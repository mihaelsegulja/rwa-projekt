using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models;

public partial class DbDiyProjectPlatformContext : DbContext
{
    public DbDiyProjectPlatformContext()
    {
    }

    public DbDiyProjectPlatformContext(DbContextOptions<DbDiyProjectPlatformContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<DifficultyLevel> DifficultyLevels { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectImage> ProjectImages { get; set; }

    public virtual DbSet<ProjectMaterial> ProjectMaterials { get; set; }

    public virtual DbSet<ProjectStatus> ProjectStatuses { get; set; }

    public virtual DbSet<ProjectStatusType> ProjectStatusTypes { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comment__3214EC079A655E10");

            entity.ToTable("Comment");

            entity.Property(e => e.DateCreated).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK__Comment__ParentC__619B8048");

            entity.HasOne(d => d.Project).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__Project__60A75C0F");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__UserId__5FB337D6");
        });

        modelBuilder.Entity<DifficultyLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Difficul__3214EC07EF4CE542");

            entity.ToTable("DifficultyLevel");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Image__3214EC07A559543E");

            entity.ToTable("Image");

            entity.Property(e => e.DateAdded).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Log__3214EC07821204C2");

            entity.ToTable("Log");

            entity.Property(e => e.Level).HasMaxLength(50);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Material__3214EC0791888B52");

            entity.ToTable("Material");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Project__3214EC07B1D24B29");

            entity.ToTable("Project");

            entity.Property(e => e.DateCreated).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DateModified).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.DifficultyLevel).WithMany(p => p.Projects)
                .HasForeignKey(d => d.DifficultyLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Project__Difficu__46E78A0C");

            entity.HasOne(d => d.Topic).WithMany(p => p.Projects)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Project__TopicId__44FF419A");

            entity.HasOne(d => d.User).WithMany(p => p.Projects)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Project__UserId__45F365D3");
        });

        modelBuilder.Entity<ProjectImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectI__3214EC07341FDD13");

            entity.ToTable("ProjectImage");

            entity.HasOne(d => d.Image).WithMany(p => p.ProjectImages)
                .HasForeignKey(d => d.ImageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectIm__Image__4CA06362");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectImages)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectIm__Proje__4BAC3F29");
        });

        modelBuilder.Entity<ProjectMaterial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectM__3214EC077B934E95");

            entity.ToTable("ProjectMaterial");

            entity.HasOne(d => d.Material).WithMany(p => p.ProjectMaterials)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectMa__Mater__534D60F1");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectMaterials)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectMa__Proje__52593CB8");
        });

        modelBuilder.Entity<ProjectStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectS__3214EC0756FCB247");

            entity.ToTable("ProjectStatus");

            entity.Property(e => e.DateModified).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.StatusTypeId).HasDefaultValue(10);

            entity.HasOne(d => d.Approver).WithMany(p => p.ProjectStatuses)
                .HasForeignKey(d => d.ApproverId)
                .HasConstraintName("FK__ProjectSt__Appro__5BE2A6F2");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectStatuses)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectSt__Proje__5812160E");

            entity.HasOne(d => d.StatusType).WithMany(p => p.ProjectStatuses)
                .HasForeignKey(d => d.StatusTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectSt__Statu__59FA5E80");
        });

        modelBuilder.Entity<ProjectStatusType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectS__3214EC079F08A224");

            entity.ToTable("ProjectStatusType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Topic__3214EC079E8B0B6D");

            entity.ToTable("Topic");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC071EDE0ED4");

            entity.ToTable("User");

            entity.Property(e => e.DateCreated).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PasswordSalt).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(255);
            entity.Property(e => e.SecurityToken).HasMaxLength(255);
            entity.Property(e => e.UserRoleId).HasDefaultValue(10);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.UserRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__UserRoleId__3C69FB99");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC07A8BD6B54");

            entity.ToTable("UserRole");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
