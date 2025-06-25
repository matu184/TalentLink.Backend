using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink.Application.DTOs
{
    public class UserRegisterDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Role { get; set; } // 0 = Student, 1 = Senior, 2 = Parent
        public string? StudentEmail { get; set; }
<<<<<<< HEAD
        public DateOnly? DateOfBirth { get; set; }
=======
        public DateTime? DateOfBirth { get; set; }
>>>>>>> heroku/main
        public string? ZipCode { get; set; }
        public string? City { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }


    }
}
