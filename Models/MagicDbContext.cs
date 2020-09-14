using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MagicTheGatheringFinal.Models
{
    public partial class MagicDbContext : DbContext
    {
        
        public MagicDbContext()
        {
        }

        public MagicDbContext(DbContextOptions<MagicDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<CardsTable> CardsTable { get; set; }
        public virtual DbSet<DecksTable> DecksTable { get; set; }
        public virtual DbSet<QuizTable> QuizTable { get; set; }
        public virtual DbSet<UsersTable> UsersTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=tcp:weatherlight.database.windows.net,1433;Initial Catalog=MagicDb;Persist Security Info=False;User ID=Gideon;Password=5WUBRG2W!N;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<CardsTable>(entity =>
            {
                entity.ToTable("cardsTable");

                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<DecksTable>(entity =>
            {
                entity.ToTable("decksTable");

                entity.HasIndex(e => e.CardId);

                entity.HasIndex(e => e.UserTableId);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CardId).HasColumnName("cardId");

                entity.HasOne(d => d.Card)
                    .WithMany(p => p.DecksTable)
                    .HasForeignKey(d => d.CardId);

                entity.HasOne(d => d.UserTable)
                    .WithMany(p => p.DecksTable)
                    .HasForeignKey(d => d.UserTableId);
            });

            modelBuilder.Entity<QuizTable>(entity =>
            {
                entity.ToTable("quizTable");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Color).HasColumnName("color");

                entity.Property(e => e.Word).HasColumnName("word");
            });

            modelBuilder.Entity<UsersTable>(entity =>
            {
                entity.ToTable("usersTable");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AspUserId)
                    .HasColumnName("aspUserId")
                    .HasMaxLength(450);

                entity.Property(e => e.Playertype).HasColumnName("playertype");

                entity.HasOne(d => d.AspUser)
                    .WithMany(p => p.UsersTable)
                    .HasForeignKey(d => d.AspUserId)
                    .HasConstraintName("FK__usersTabl__aspUs__7A672E12");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
