using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentLink.Domain.Entities;

namespace TalentLink.Application.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Role { get; set; } = null!;
        public JobCategory Category { get; set; }

    }
}
