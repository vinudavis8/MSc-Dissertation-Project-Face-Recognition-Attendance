using AttendanceApi.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace AttendanceApi.Data

{
    public class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseModule> CourseModule { get; set; }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Schedule> Schedule { get; set; }

        //public DbSet<StudentModule> StudentModules { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Attendances)
                .WithOne(a => a.Student)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Attendance>()
                .HasOne(s => s.Student)
                .WithMany(a => a.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

        //    modelBuilder.Entity<StudentModule>()
        //.HasOne(sm => sm.Student)
        //.WithMany(s => s.StudentModules)
        //.HasForeignKey(sm => sm.StudentId)
        //.OnDelete(DeleteBehavior.NoAction);


        }
    }
}
