using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink.Domain.Entities
{
    public abstract class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserRole Role { get; set; }
        public ICollection<Job> CreatedJobs { get; set; } = new List<Job>();
        public ICollection<Rating> GivenRatings { get; set; } = new List<Rating>();
        public ICollection<Rating> ReceivedRatings { get; set; } = new List<Rating>();
        public ICollection<JobComment> WrittenComments { get; set; } = new List<JobComment>();
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? ZipCode { get; set; }
        public string? City { get; set; }



    }

    public enum UserRole
    {
        Student,
        Senior,
        Parent,
        Admin
    }

    public class Student : User
    {
        public DateOnly DateOfBirth { get; set; }

        public Guid? VerifiedByParentId { get; set; }
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();

    }

    public class Senior : User
    {
        //public string ZipCode { get; set; } = null!;
        //public string City { get; set; } = null!;
        //public double Latitude { get; set; }
        //public double Longitude { get; set; }
    }


    public class Parent : User
    {
        public ICollection<VerifiedStudent> VerifiedStudents { get; set; } = new List<VerifiedStudent>();
    }
    public class Admin : User
    {
        bool isAdmin = true;
        
    }

}
