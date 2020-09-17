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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
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
                entity.HasIndex(e => e.AspUserId);

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

                entity.HasOne(d => d.AspUser)
                    .WithMany(p => p.InverseAspUser)
                    .HasForeignKey(d => d.AspUserId);
            });

            modelBuilder.Entity<CardsTable>(entity =>
            {
                entity.ToTable("cardsTable");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Black).HasColumnName("black");

                entity.Property(e => e.Blue).HasColumnName("blue");

                entity.Property(e => e.CardArtUrl).HasColumnName("cardArtUrl");

                entity.Property(e => e.CardId).HasColumnName("cardId");

                entity.Property(e => e.Cmc).HasColumnName("cmc");

                entity.Property(e => e.DecksTableKey).HasColumnName("decksTableKey");

                entity.Property(e => e.Green).HasColumnName("green");

                entity.Property(e => e.IsCommander).HasColumnName("isCommander");

                entity.Property(e => e.ManaCost).HasColumnName("mana_cost");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.OracleText).HasColumnName("oracleText");

                entity.Property(e => e.Power).HasColumnName("power");

                entity.Property(e => e.Red).HasColumnName("red");

                entity.Property(e => e.Toughness).HasColumnName("toughness");

                entity.Property(e => e.TypeLine).HasColumnName("type_line");

                entity.Property(e => e.White).HasColumnName("white");

                entity.HasOne(d => d.DecksTableKeyNavigation)
                    .WithMany(p => p.CardsTable)
                    .HasForeignKey(d => d.DecksTableKey)
                    .HasConstraintName("FK__cardsTabl__decks__787EE5A0");
            });

            modelBuilder.Entity<DecksTable>(entity =>
            {
                entity.ToTable("decksTable");

                entity.HasIndex(e => e.AspUserId);

                entity.HasIndex(e => e.CardId);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DeckName)
                    .HasColumnName("DECK_NAME")
                    .HasMaxLength(50);

                entity.HasOne(d => d.AspUser)
                    .WithMany(p => p.DecksTable)
                    .HasForeignKey(d => d.AspUserId);

                entity.HasOne(d => d.Card)
                    .WithMany(p => p.DecksTable)
                    .HasForeignKey(d => d.CardId);
            });

            modelBuilder.Entity<QuizTable>(entity =>
            {
                entity.ToTable("quizTable");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Color).HasColumnName("color");

                entity.Property(e => e.Word).HasColumnName("word");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
