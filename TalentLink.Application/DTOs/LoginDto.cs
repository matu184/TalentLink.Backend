using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink.Application.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
<<<<<<< HEAD
        public string? ZipCode { get; set; } = null!; 

        public string? City { get; set; } = null!; // Optional für Senioren, um Koordinaten zu ermitteln
=======
>>>>>>> heroku/main
    }
}
