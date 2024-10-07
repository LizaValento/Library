using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Data.Context
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.Nickname)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Nickname)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Books)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Book>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Book>()
                .Property(b => b.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Book>()
                .Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Book>()
                .Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();

            modelBuilder.Entity<Book>()
                .Property(b => b.Genre)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Book>()
                .Property(b => b.Description)
                .IsRequired();

            modelBuilder.Entity<Book>()
                .HasOne(b => b.User)
                .WithMany(u => u.Books)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(u => u.Books)
                .HasForeignKey(b => b.AuthorId);

            modelBuilder.Entity<Book>()
                .Property(b => b.IssueDate);

            modelBuilder.Entity<Book>()
                .Property(b => b.ReturnDate);

            modelBuilder.Entity<Book>()
                .Property(b => b.BookImage)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("default-image.png");

            modelBuilder.Entity<Author>()
             .HasKey(u => u.Id);

            modelBuilder.Entity<Author>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Author>()
                .Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Author>()
                .Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Author>()
                .Property(u => u.DateOfBirth);

            modelBuilder.Entity<Author>()
                .Property(u => u.Country)
                .HasMaxLength(50);

            modelBuilder.Entity<Author>()
                .HasMany(a => a.Books)
                .WithOne(c => c.Author);

            modelBuilder.Entity<RefreshToken>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<RefreshToken>()
                .Property(b => b.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<RefreshToken>()
                .Property(b => b.Token)
                .IsRequired();

            modelBuilder.Entity<RefreshToken>()
                .Property(b => b.ExpiresAt);
        }
    }
}
