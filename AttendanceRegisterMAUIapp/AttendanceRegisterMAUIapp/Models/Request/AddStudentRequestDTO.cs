using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceRegisterMAUIapp.Models.Request
{
    public class AddStudentRequestDTO
    {

        // Personal Information
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        // Contact Information
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        // Academic Information
        public int GradeLevel { get; set; }
        public int CourseId { get; set; }


    }
}
