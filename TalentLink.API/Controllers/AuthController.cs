using Microsoft.AspNetCore.Mvc;
using TalentLink.Application.DTOs;
using TalentLink.Application.Interfaces;
using TalentLink.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TalentLink.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using TalentLink.Infrastructure.Services;



namespace TalentLink.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TalentLinkDbContext _context;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly GeocodingService _geocodingService;

<<<<<<< HEAD
        public AuthController(IUserService userService, IConfiguration configuration, TalentLinkDbContext context, GeocodingService geocodingService)
=======
        public AuthController(IUserService userService, IConfiguration configuration, TalentLinkDbContext context)
>>>>>>> heroku/main
        {
            _context = context;
            _userService = userService;
            _configuration = configuration;
<<<<<<< HEAD
            _geocodingService = geocodingService; 
=======
>>>>>>> heroku/main
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto input)
        {
            // 0-Student | 1-Senior | 2-Parent | 3-Admin
            User user = input.Role switch
            {
                0 => new Student(),
                1 => new Senior(),
                2 => new Parent(),
                3 => new Admin(),
                _ => throw new ArgumentException("Invalid role")
            };

            user.Name = input.Name;
            user.Email = input.Email;
            user.Role = (UserRole)input.Role;

            // ► Student braucht ein Geburtsdatum
            if (user is Student s)
            {
                if (input.DateOfBirth == null)
                    return BadRequest("Geburtsdatum erforderlich für Studenten.");

                s.DateOfBirth = input.DateOfBirth.Value;
            }

            // ► Senior braucht PLZ, Ort → Koordinaten automatisch ermitteln
            if (user is Senior senior)
            {
                if (string.IsNullOrWhiteSpace(input.ZipCode) || string.IsNullOrWhiteSpace(input.City))
                    return BadRequest("PLZ und Ort erforderlich für Senioren.");

                senior.ZipCode = input.ZipCode;
                senior.City = input.City;

                var (lat, lng) = await _geocodingService.GetCoordinatesAsync(input.ZipCode, input.City);
                senior.Latitude = lat;
                senior.Longitude = lng;
<<<<<<< HEAD
                Console.WriteLine($"Senior: {senior.Name}, {senior.ZipCode}, {senior.City}, {senior.Latitude}, {senior.Longitude}");
            }
            
=======
            }

>>>>>>> heroku/main
            // -- Benutzer anlegen (Passwort wird dort gehasht)
            var createdUser = await _userService.RegisterAsync(user, input.Password);

            /* ▼ Verifizierung: Parent → Student ---------------------------------- */
            if (createdUser is Parent parent && !string.IsNullOrWhiteSpace(input.StudentEmail))
            {
                var child = await _userService.FindByEmailAsync(input.StudentEmail);
                if (child is Student student)
                {
                    student.VerifiedByParentId = parent.Id;

                    bool alreadyVerified = await _context.VerifiedStudents
                        .AnyAsync(v => v.StudentId == student.Id && v.ParentId == parent.Id);

                    if (!alreadyVerified)
                    {
                        await _context.VerifiedStudents.AddAsync(new VerifiedStudent
                        {
                            ParentId = parent.Id,
                            StudentId = student.Id
                        });
                    }

                    await _context.SaveChangesAsync();
                }
            }

            return Ok();
        }

<<<<<<< HEAD
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userService.AuthenticateAsync(dto.Email, dto.Password, dto.ZipCode, dto.City);
            if (user == null) return Unauthorized("Invalid credentials");
=======

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userService.AuthenticateAsync(dto.Email, dto.Password);
            if (user == null)
                return Unauthorized("Invalid credentials");
>>>>>>> heroku/main

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Hole VerifiedByParentId, falls Student
            Guid? verifiedByParentId = null;
            if (user is Student student)
            {
                verifiedByParentId = student.VerifiedByParentId;
            }

            return Ok(new AuthResponseDto
            {
                Token = tokenString,
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
<<<<<<< HEAD
                VerifiedByParentId = verifiedByParentId,
               
                ZipCode = user.ZipCode,
                City = user.City // <-- HINZUGEFÜGT!
            });
        }


=======
                VerifiedByParentId = verifiedByParentId
            });
        }
>>>>>>> heroku/main
    }
}
