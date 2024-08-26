using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModel;
using System.Security.Claims;
using WorkHiveServices.Interface;

namespace WorkHiveMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userservice;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userservice = userService;
            _httpContextAccessor = httpContextAccessor;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Users()
        {
            try
            {
                var usersList = _userservice.GetUsers();
                return View(usersList);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { ex = ex });
            }
        }
        public async Task<IActionResult> UserList()
        {
            try
            {
                var usersList = await _userservice.GetUsers();
                return View(usersList);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { ex = ex });
            }
        }
        public async Task<string> Login(string username, string password)
        {
            try
            {
                //var user = await _userservice.Login(username, password);
                if (username=="admin" && password=="abcd1234")
                {
                    //setting session values
                    HttpContext.Session.SetString("loggedInUserId", "1");
                    HttpContext.Session.SetString("loggedInUserType", "Admin");
                    HttpContext.Session.SetString("loggedInUserName", "Admin");


                    return "Admin";
                }
                else
                    return "Failed";
            }
            catch (Exception ex)
            {
                return "Failed";
            }

        }
        public async Task<bool> CheckIfEmailExists(string email)
        {
            try
            {
                var result = await _userservice.CheckIfEmailExists(email);
                return result;
            }
            catch (Exception ex)
            {
                return true;
            }
        }
        public IActionResult Logout()
        {

            HttpContext.Session.SetString("loggedInUserId", String.Empty);
            HttpContext.Session.SetString("loggedInUserType", String.Empty);
            HttpContext.Session.SetString("loggedInUserName", String.Empty);
            return RedirectToAction("Index", "Home");

        }
        [HttpPost]
        public async Task<bool> Register([FromBody] RegisterRequest user)
        {
            try
            {
                var result = await _userservice.Register(user);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var result = await _userservice.ForgotPassword(email);
            if (result)
                ViewBag.result = "Please check your email to reset password";
            else
                ViewBag.result = "Could not find user with this email";
            return View();
        }


        [HttpGet]
        public async Task<ActionResult> ResetPassword()
        {
            try
            {
                string code = !String.IsNullOrEmpty(Request.Query["resetCode"]) ? Request.Query["resetCode"] : "";
                string email = !String.IsNullOrEmpty(Request.Query["email"]) ? Request.Query["email"] : "";
                ViewBag.code = code;
                ViewBag.email = email;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { ex = ex });
            }
        }
        [HttpPost]
        public async Task<ActionResult> ResetPassword([FromForm] IFormCollection form)
        {
            try
            {
                ResetPasswordRequest password = new ResetPasswordRequest();
                password.Email = form["Email"];
                password.Code = form["Code"];
                password.Password = form["Password"];
                var result = await _userservice.ResetPassword(password);
                if (result)
                    ViewBag.result = "Password updated successfully.";
                else
                    ViewBag.result = "Password update failed.";

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { ex = ex });
            }
        }
    }
}
