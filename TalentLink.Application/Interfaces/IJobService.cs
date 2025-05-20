using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentLink.Domain.Entities;

namespace TalentLink.Application.Interfaces
{
    public interface IJobService
    {
        Task<IEnumerable<Job>> GetAllJobsAsync();
        Task<Job> CreateJobAsync(Job job);
    }
}
