using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentLink.Application.Interfaces;
using TalentLink.Domain.Entities;
using TalentLink.Infrastructure.Persistence;

namespace TalentLink.Infrastructure.Services
{
    public class JobService : IJobService
    {
        private readonly TalentLinkDbContext _context;

        public JobService(TalentLinkDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Job>> GetAllJobsAsync()
        {
            return await _context.Jobs.ToListAsync();
        }

        public async Task<Job> CreateJobAsync(Job job)
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }
    }
}
