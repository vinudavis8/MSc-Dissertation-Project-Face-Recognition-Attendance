using AttendanceApi.Models.Domain;

namespace AttendanceApi.Models.DTO
{
    public class CourseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //navigation property
        public Department Department { get; set; }
        // Navigation property
        //public List<StudentModule> StudentModules { get; set; }
    }
}
