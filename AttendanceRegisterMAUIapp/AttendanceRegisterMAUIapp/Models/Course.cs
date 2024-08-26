using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceRegisterMAUIapp.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }

        public Department Department { get; set; }
        // Navigation property
        //public List<StudentModule> StudentModules { get; set; }

        public override string ToString()
        {
            return Name; // Return the department name
        }
    }
}
