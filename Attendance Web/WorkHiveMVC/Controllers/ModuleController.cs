using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Domain;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;

namespace WorkHiveMVC.Controllers
{
    public class ModuleController : Controller
    {
        // GET: DepartmentController
        public async Task<ActionResult> ModuleList()
        {
            var course = await ApiHelper.GetAsync<List<CourseModule>>("api/CourseModule");
            return View(course);
        }

        // GET: DepartmentController
        public async Task<ActionResult> CourseScheduleList()
        {
            var departments = await ApiHelper.GetAsync<List<Department>>("api/Department");
            var course = await ApiHelper.GetAsync<List<Course>>("api/Course");
            ViewBag.Departments = departments;
            ViewBag.CourseList = course;
            var courseSchedule = await ApiHelper.GetAsync<List<Schedule>>("api/CourseModule/GetSchedule?courseId=1");
            return View(courseSchedule);
        }
  
            public async Task<ActionResult> LoadSchedule(int courseId)
        
        {
            var courseSchedule = await ApiHelper.GetAsync<List<Schedule>>("api/CourseModule/GetSchedule?courseId="+ courseId);
            return PartialView("_PartialScheduleList", courseSchedule);
        }

        // GET: DepartmentController/Create
        public async Task<ActionResult> Create()
        {
            var courseList = await ApiHelper.GetAsync<List<Course>>("api/Course");
            ViewBag.CourseList = courseList;
            return View();
        }

        // POST: DepartmentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CourseModule module)
        {
            var result = await ApiHelper.PostAsync<int>("api/CourseModule", module);
            return RedirectToAction("ModuleList");
        }



        // GET: DepartmentController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var result = await ApiHelper.DeleteAsync("api/CourseModule/" + id);
            return RedirectToAction("ModuleList");
        }
    }
}
