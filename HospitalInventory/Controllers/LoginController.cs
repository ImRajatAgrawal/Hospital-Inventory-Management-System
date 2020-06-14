using HospitalInventory.Models;
using System.Linq;
using System.Web.Mvc;

namespace HospitalInventory.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult LoginPage()
        {
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
                else
                {
                    ViewBag.Message = "Password changed successfully!!";
                    userDetail.password = cpm.ConfirmPassword;
                    db.SaveChanges();
                    return View("ChangePassword", cpm);
                }
            }

        }
    }
}