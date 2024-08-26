using AttendanceApi.Data;
using AttendanceApi.Models.Domain;
using AttendanceApi.Models.DTO;
using AttendanceApi.Models.DTO.Request;
using AttendanceApi.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AttendanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository courseRepository;
        private readonly IMapper mapper;

        public CourseController(ICourseRepository repository, IMapper mapper)
        {
            this.courseRepository = repository;
            this.mapper = mapper;
        }
        // GET: api/department/
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            var modules = await courseRepository.GetAllAsync();
            var moduleDTO = mapper.Map<List<CourseDTO>>(modules);
            return Ok(moduleDTO);
        }


        // GET api/<DepartmentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Course module = await courseRepository.GetAsync(id);
            if (module == null)
                return NotFound();
            var moduleDTO = mapper.Map<CourseDTO>(module);

            return Ok(moduleDTO);
        }
        // GET api/<DepartmentController>/5
        [HttpGet("GetCoursedByDepId/{departmentId}")]
        public async Task<IActionResult> GetCoursedByDepId(int departmentId)
        {
            var module = await courseRepository.GetCourseByDepIdAsync(departmentId);
            var moduleDTO = mapper.Map<List<CourseDTO>>(module);
            return Ok(moduleDTO);
        }

        // POST api/<DepartmentController>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddCourseRequestDTO addModuleRequestDTO)
        {
            var module = mapper.Map<Course>(addModuleRequestDTO);
            var result = await courseRepository.CreateAsync(module);
            return Ok(module.Id);
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
            Course module = await courseRepository.DeleteAsync(id);
            if (module == null)
                return NotFound();
            return Ok();
        }
    }
}
