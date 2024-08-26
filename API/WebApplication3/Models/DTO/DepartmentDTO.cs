using AttendanceApi.Models.Domain;

namespace AttendanceApi.Models.DTO
{
    public class DepartmentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Course> Courses { get; set; }
    }
}
