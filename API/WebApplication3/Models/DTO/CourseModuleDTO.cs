using AttendanceApi.Models.Domain;

namespace AttendanceApi.Models.DTO
{
    public class CourseModuleDTO
    {
            public string Name { get; set; }
        public int Id { get; set; }

        public int CourseId { get; set; }

            //navigation property
            public Course course { get; set; }

        }
}
