using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentLink.Domain.Entities;

namespace TalentLink.Infrastructure.Persistence
{
    public class TalentLinkDbContext : DbContext
    {
        public TalentLinkDbContext(DbContextOptions<TalentLinkDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Senior> Seniors => Set<Senior>();
        public DbSet<Parent> Parents => Set<Parent>();
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<VerifiedStudent> VerifiedStudents => Set<VerifiedStudent>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Tip> Tips { get; set; }
        public DbSet<JobComment> JobComments { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<Admin> Admins => Set<Admin>();



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().UseTptMappingStrategy();

            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<Senior>().ToTable("Seniors");
            modelBuilder.Entity<Parent>().ToTable("Parents");
            modelBuilder.Entity<Admin>().ToTable("Admins");

            modelBuilder.Entity<Job>()
                .HasOne(j => j.CreatedBy)
                .WithMany(u => u.CreatedJobs)
                .HasForeignKey(j => j.CreatedById);

            modelBuilder.Entity<JobApplication>()
                .HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId);

            modelBuilder.Entity<JobApplication>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Applications)
                .HasForeignKey(a => a.StudentId);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.FromUser)
                .WithMany(u => u.GivenRatings)
                .HasForeignKey(r => r.FromUserId)
                .OnDelete(DeleteBehavior.Restrict); // wichtig: keine Kettenlöschung

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.ToUser)
                .WithMany(u => u.ReceivedRatings)
                .HasForeignKey(r => r.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobComment>()
                .HasOne(c => c.Job)
                .WithMany(j => j.Comments)
                .HasForeignKey(c => c.JobId);

            modelBuilder.Entity<JobComment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.WrittenComments)
                .HasForeignKey(c => c.AuthorId);
            
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Category)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.CategoryId);

            modelBuilder.Entity<Tip>()
                .HasOne(t => t.CreatedBy)
                .WithMany()
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
