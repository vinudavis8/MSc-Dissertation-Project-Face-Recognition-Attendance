using AttendanceApi.Models.Domain;

namespace AttendanceApi.Models.DTO
{
    public class AttendanceDTO
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseModuleId { get; set; }
        public bool IsPresent { get; set; }

        public DateTime AttendanceDate { get; set; }

        //navigation property
        public Student Student { get; set; }
        public CourseModule CourseModule { get; set; }
    }
}
