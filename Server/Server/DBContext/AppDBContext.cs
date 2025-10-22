using Microsoft.EntityFrameworkCore;
using Server.Games.Entities;
using Server.Users.Entities;

namespace Server.DBContext;

public class AppDBContext: DbContext,IDataContext
{
    public DbSet<GamesEntity> Games { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<GamesUserRelationEntity> GamesUserRelations { get; set; }

    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options){}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GamesEntity>().ToTable("Games");
        modelBuilder.Entity<GamesEntity>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e=>e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Title).IsUnique();
            entity.Property(e => e.Genre).IsRequired();
            entity.Property(e => e.ReleaseDate).IsRequired();
        }
        );
        modelBuilder.Entity<UserEntity>().ToTable("Users");
        modelBuilder.Entity<UserEntity>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e=>e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
        }
        );
        modelBuilder.Entity<GamesUserRelationEntity>().ToTable("GamesUserRelations");
        modelBuilder.Entity<GamesUserRelationEntity>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e=>e.Id).ValueGeneratedOnAdd();
            entity.HasOne(e => e.User).WithMany().IsRequired().OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Games).WithMany().IsRequired().OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.Status).IsRequired();
        }
        );
        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

}
