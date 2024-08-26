namespace AttendanceApi.Models.Domain
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }

        //navigation property
        public Department Department { get; set; }
        // Navigation property
       //public List<StudentModule> StudentModules { get; set; }
    }
}
