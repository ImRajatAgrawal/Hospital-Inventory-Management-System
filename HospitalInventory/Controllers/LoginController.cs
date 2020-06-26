using HospitalInventory.Models;
using System;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace HospitalInventory.Controllers
{
    public class LoginController : Controller
    {
        private HospitalInventoryEntities db = new HospitalInventoryEntities();
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


            using (db)
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
            using (db)
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
        public void mailtorecipient(string Body, string subject, string recipient = "your_email@gmail.com")
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(recipient);
            mail.From = new MailAddress("your_email@gmail.com");
            mail.Subject = subject;
            mail.Body = Body;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("your_email@gmail.com", "your_password"); // Enter seders User name and password  
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }

        public ActionResult ForgotPassword() {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            string resetCode = Guid.NewGuid().ToString();
            var verifyUrl = "/Login/ResetPassword/" + resetCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            using (db)
            {
                var getUser = (from s in db.Employees where s.emailAddress.Equals(EmailID) select s).FirstOrDefault();
                if (getUser != null)
                {
                    getUser.resetPasswordCode = resetCode;

                    

                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();

                    var subject = "Password Reset Request";
                    var body = "Hi " + getUser.employeeName + ", <br/> You recently requested to reset your password for your account. Click the link below to reset it. " +

                         " <br/><br/><a href='" + link + "'>" + link + "</a> <br/><br/>" +
                         "If you did not request a password reset, please ignore this email or reply to let us know.<br/><br/> Thank you";

                    mailtorecipient(body,subject);

                    ViewBag.Message = "Reset password link has been sent to your email id.";
                }
                else
                {
                    ViewBag.Message = "User doesn't exists.";
                    return View();
                }
            }

            return View();
        }
        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (db)
            {
                var user = db.Employees.Where(a => a.resetPasswordCode.Equals(id)).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (db)
                {
                    var user = db.Employees.Where(a => a.resetPasswordCode.Equals(model.ResetCode)).FirstOrDefault();
                    if (user != null)
                    {
                        user.password = model.NewPassword;
                        //make resetpasswordcode empty string now
                        user.resetPasswordCode = null;
                        //to avoid validation issues, disabling it
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        message = "New password updated successfully";
                    }
                    else
                        return HttpNotFound();
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }

    }
}