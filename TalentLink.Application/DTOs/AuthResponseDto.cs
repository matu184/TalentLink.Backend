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
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public String Role { get; set; } = null!;
        public Guid? VerifiedByParentId { get; set; } 

    }
}
