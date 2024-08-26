namespace AttendanceApi.Models.Domain
{
    public class Attendance
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseModuleId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public bool IsPresent { get; set; }


        //navigation property
        public Student Student { get; set;}
        public CourseModule CourseModule { get; set; }
    }
}
