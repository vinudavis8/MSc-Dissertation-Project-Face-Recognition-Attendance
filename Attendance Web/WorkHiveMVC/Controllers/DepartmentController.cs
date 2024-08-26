using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Domain;
using System.Runtime.Intrinsics.Arm;

namespace WorkHiveMVC.Controllers
{
    public class DepartmentController : Controller
    {
        // GET: DepartmentController
        public async Task<ActionResult> DepartmentList()
        {
            var departments = await ApiHelper.GetAsync<List<Department>>("api/Department");
            return View(departments);
        }


        // GET: DepartmentController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DepartmentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Department dep)
        {
            var result = await ApiHelper.PostAsync<int>("api/Department", dep);
            return RedirectToAction("DepartmentList");
        }



        // GET: DepartmentController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var result = await ApiHelper.DeleteAsync("api/Department/" + id);
            return RedirectToAction("DepartmentList");
        }
    }
}
