using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Domain;
using System.Runtime.Intrinsics.Arm;

namespace WorkHiveMVC.Controllers
{
    public class CourseController : Controller
    {
        // GET: DepartmentController
        public async Task<ActionResult> CourseList()
        {
            var course = await ApiHelper.GetAsync<List<Course>>("api/Course");
            return View(course);
        }




        // GET: DepartmentController/Create
        public async Task<ActionResult> Create()
        {
            var departmentList = await ApiHelper.GetAsync<List<Department>>("api/Department");
            ViewBag.Departments = departmentList;
            return View();
        }

        // POST: DepartmentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Course course)
        {
            var result = await ApiHelper.PostAsync<int>("api/Course", course);
            return RedirectToAction("CourseList");
        }



        // GET: DepartmentController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var result = await ApiHelper.DeleteAsync("api/Course/" + id);
            return RedirectToAction("CourseList");
        }
    }
}
