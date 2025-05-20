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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().UseTptMappingStrategy();

            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<Senior>().ToTable("Seniors");
            modelBuilder.Entity<Parent>().ToTable("Parents");

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
        }
    }
}
