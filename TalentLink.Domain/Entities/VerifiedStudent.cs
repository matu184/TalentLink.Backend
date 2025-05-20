using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink.Domain.Entities
{
    public class VerifiedStudent
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Parent Parent { get; set; } = null!;
        public Guid StudentId { get; set; }
    }

}
