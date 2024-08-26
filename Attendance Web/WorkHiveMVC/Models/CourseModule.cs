namespace Models.Domain
{
    public class CourseModule
    {

            public int Id { get; set; }
            public string Name { get; set; }
            public int CourseId { get; set; }

            //navigation property
            public Course Course { get; set; }
            // Navigation property
            //public List<StudentModule> StudentModules { get; set; }
        }
}
