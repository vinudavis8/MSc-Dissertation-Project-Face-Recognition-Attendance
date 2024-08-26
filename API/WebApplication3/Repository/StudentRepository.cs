using AttendanceApi.Data;
using AttendanceApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AttendanceApi.Repository
{
    public class StudentRepository:IStudentRepository
    {
        private readonly AttendanceDbContext dbContext;

        public StudentRepository(AttendanceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<Student>> GetAllAsync()
        {
            return await dbContext.Students.Include(x => x.Course).ToListAsync();
        }
        public async Task<Student> GetAsync(int id)
        {
             var studentDetails= await dbContext.Students.Include(x => x.Course).ThenInclude(x => x.Department).SingleOrDefaultAsync(x => x.Id == id);
           //var studentModules= await dbContext.StudentModules.Include(x => x.CourseModule).Where(x => x.StudentId == id).ToListAsync();
           // studentDetails.StudentModules = studentModules;
            return studentDetails;

        }
        public async Task<Student> CreateAsync(Student student)
        {
            dbContext.Students.Add(student);
            var courseModules= dbContext.CourseModule.Where(x=>x.CourseId==student.CourseId).ToList();
            await dbContext.SaveChangesAsync();

            //foreach (var module in courseModules)
            //{
            //    StudentModule studentModule = new StudentModule();
            //    studentModule.StudentId = student.Id;
            //    studentModule.CourseModuleId = module.Id;
            //    dbContext.StudentModules.Add(studentModule);
            //}
            //await dbContext.SaveChangesAsync();

            return student;
        }
        public async Task<Student> UpdateAsync(Student student)
        {
            Student result = await dbContext.Students.SingleOrDefaultAsync(x => x.Id == student.Id);
            if (result == null)
                return null;

            result.FirstName = student.FirstName;
            result.LastName = student.LastName;
            result.PhoneNumber = student.PhoneNumber;
            result.LastName = student.LastName;
            result.Address = student.Address;
            result.DateOfBirth = student.DateOfBirth;
            //result.Department= student.Department;

            await dbContext.SaveChangesAsync();
            return result;
        }

        public async Task<Student> DeleteAsync(int id)
        {
            Student student = await dbContext.Students.SingleOrDefaultAsync(x => x.Id == id);
            if (student == null)
                return null;
            dbContext.Students.Remove(student);
            await dbContext.SaveChangesAsync();
            return student;
        }
    }
}
