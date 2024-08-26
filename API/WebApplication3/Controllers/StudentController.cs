using AttendanceApi.Data;
using AttendanceApi.Models.Domain;
using AttendanceApi.Models.DTO;
using AttendanceApi.Models.DTO.Request;
using AttendanceApi.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository studentRepository;
        private readonly IMapper mapper;

        public StudentController(IStudentRepository repository, IMapper mapper)
        {
            this.studentRepository = repository;
            this.mapper = mapper;
        }
        // GET: api/department/
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            var students = await studentRepository.GetAllAsync();
            var studentsDTO = mapper.Map<List<StudentDTO>>(students);
            return Ok(studentsDTO);
        }


        // GET api/<DepartmentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Student student = await studentRepository.GetAsync(id);
            if (student == null)
                return NotFound();
            var studentsDTO = mapper.Map<StudentDTO>(student);

            return Ok(studentsDTO);
        }

        // POST api/<DepartmentController>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddStudentRequestDTO addStudentRequestDTO)
        {
            var student = mapper.Map<Student>(addStudentRequestDTO);
            var result = await studentRepository.CreateAsync(student);
            return Ok(student.Id);
        }
        // POST api/<DepartmentController>
        [HttpPost]
        [Route("AddStudentModules")]
        public async Task<IActionResult> AddStudentModules([FromBody] AddStudentRequestDTO addStudentRequestDTO)
        {
            var student = mapper.Map<Student>(addStudentRequestDTO);
            var result = await studentRepository.CreateAsync(student);
            return Ok(student.Id);
        }

        //// PUT api/<DepartmentController>/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, [FromBody] UpdateModuleRequestDTO updateModuleRequestDTO)
        //{
        //    CourseModule module = new Department { Id = id, Name = updateModuleRequestDTO.Name };
        //    var result = await moduleRepository.UpdateAsync(module);
        //    if (module == null)
        //        return NotFound();
        //    return Ok(module.Id);
        //}

        // DELETE api/<DepartmentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Student student = await studentRepository.DeleteAsync(id);
            if (student == null)
                return NotFound();
            return Ok();
        }
    }
}

