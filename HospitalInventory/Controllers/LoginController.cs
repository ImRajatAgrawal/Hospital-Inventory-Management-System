using HospitalInventory.Models;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace HospitalInventory.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult LoginPage()
        {
            HttpCookie cookie = Request.Cookies.Get("Employee");
            if (cookie != null)
            {
                string EncryptedPassword = cookie["password"];
                byte[] b = Convert.FromBase64String(EncryptedPassword);
                string DecryptedPassword = ASCIIEncoding.ASCII.GetString(b);
                ViewBag.username = cookie["username"];
                ViewBag.password = DecryptedPassword;

            }

            return View();
        }
        [HttpPost]
        public ActionResult Authorize(Employee employee)
        {


            using (HospitalInventoryEntities db = new HospitalInventoryEntities())
            {

                var employees = db.Employees.Where(emp => emp.userName == employee.userName.Trim()).ToList();
                var empDetail = employees.SingleOrDefault(emp => emp.userName == employee.userName.Trim() && emp.password == employee.password);
                if (empDetail == null)
                {
                    ModelState.AddModelError("password", "Invalid UserName or Password");
                    return View("LoginPage", employee);
                }

                else
                {
                    HttpCookie cookie = new HttpCookie("Employee");
                    if (employee.RememberMe)
                    {
                        cookie["username"] = empDetail.userName;
                        byte[] b = ASCIIEncoding.ASCII.GetBytes(empDetail.password);
                        string EncryptedPassword = Convert.ToBase64String(b);
                        cookie["password"] = EncryptedPassword;
                        cookie.Expires = DateTime.Now.AddDays(2);
                        HttpContext.Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        cookie.Expires = DateTime.Now.AddDays(-1);
                        HttpContext.Response.Cookies.Add(cookie);
                    }
                    var designation = db.EmployeeDesignations.Where(desg => desg.designationId == empDetail.designationId).SingleOrDefault();
                    Session["userID"] = empDetail.EmployeeId;
                    Session["employeeName"] = empDetail.employeeName;
                    Session["userName"] = empDetail.userName;
                    Session["userRole"] = designation.designationName;
                    Session.Timeout = 60;
                    return RedirectToAction("Index", "Home");
                }

            }

        }
        public ActionResult Logout()
        {

            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ChangePassword()
        {
            ViewBag.Message = null;
            return View();
        }
        [HttpPost]
        public ActionResult ChangeThePassword(ChangePassword cpm)
        {
            using (HospitalInventoryEntities db = new HospitalInventoryEntities())
            {
                var employees = db.Employees.Where(emp => emp.password == cpm.CurrentPassword).ToList();
                var userDetail = employees.SingleOrDefault(emp =>emp.password == cpm.CurrentPassword);

                if (userDetail == null || !(userDetail.userName.Equals(Session["userName"])))
                {
                    ModelState.AddModelError("CurrentPassword", "Incorrect Current Password");
                    return View("ChangePassword", cpm);
                }
                else if (cpm.CurrentPassword.Equals(cpm.NewPassword))
                {
                    ModelState.AddModelError("NewPassword", "New password cannot be same as current password");
                    return View("ChangePassword", cpm);
                }
                else
                { 
                    ViewBag.Message = "Password changed successfully!!";
                    userDetail.password = cpm.ConfirmPassword;
                    db.SaveChanges();
                    HttpCookie cookie = Request.Cookies.Get("Employee");
                    if (cookie != null)
                    {
                        cookie.Expires = DateTime.Now.AddDays(-1);
                        HttpContext.Response.Cookies.Add(cookie);
                    }
                    return View("ChangePassword", cpm);
                }
            }

        }
    }
}