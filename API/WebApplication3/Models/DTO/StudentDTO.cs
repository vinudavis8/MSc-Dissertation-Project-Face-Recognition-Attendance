using AttendanceApi.Models.Domain;

namespace AttendanceApi.Models.DTO
{
    public class StudentDTO
    {
        public int Id { get; set; }

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

        //navigation property
        public Course Course { get; set; }
        //public List<StudentModule> StudentModules { get; set; }


    }
}
