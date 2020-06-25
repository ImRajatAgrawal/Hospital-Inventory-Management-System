using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HospitalInventory.Models;
using HospitalInventory.Controllers;
using System.Net.Mail;
using System.Text;

namespace HospitalInventory.Controllers
{
    public class EquipmentsController : Controller
    {
        private HospitalInventoryEntities db = new HospitalInventoryEntities();

        // GET: Equipments?searchBy=key&Search=keyvalue
        public ActionResult Index(string searchBy, string search)
        {
            if (search == null)
            {
                return View(db.Equipments.ToList());
            }
            else
            {
                if (searchBy == "EquipmentName")
                {
                    ViewBag.SearchString = search;
                    ViewBag.SearchByString = "Equipment Name";
                    var NameSearchResult = db.Equipments.Where(x => x.EquipmentName.Contains(search)).Include(e => e.EquipmentCategory);
                    return View(NameSearchResult.ToList());
                }
                else
                {
                    ViewBag.SearchString = search;
                    ViewBag.SearchByString = "Equipment Category Name";
                    var equipmentCategoryId = db.EquipmentCategories.Where(x => x.EquipmentCategoryName.Contains(search)).Select(y => y.EquipmentCategoryId).ToList();
                    List<List<Equipment>> equipmentList = new List<List<Equipment>>();
                    foreach (var item in equipmentCategoryId)
                    {
                        equipmentList.Add(db.Equipments.Where(x => x.EquipmentCategoryId.Equals(item)).Include(e => e.EquipmentCategory).ToList());
                    }
                    List<Equipment> FinalEquipmentList = new List<Equipment>();
                    foreach (var list in equipmentList)
                    {
                        foreach (var element in list)
                        {
                            FinalEquipmentList.Add(element);
                        }
                    }
                    return View(FinalEquipmentList);
                }
            }
        }

        //SEND MAIL
        public void sendemail(Dictionary<string, KeyValuePair<int, int>> orders)
        {
            string Body =
                   "<!DOCTYPE html>" +
                   "<html>" +
                   "<head>" +
                   "<style>" +
                   "table, th, td {  border: 1px solid black;}" +
                   "</style>" +
                   "</head>" +
                   "<body>" +
                   "<p>The following equipments should be ordered as their " +
                   "current quantity is below threshold value</p>" +
                   "<h2>Equipments to be ordered are:</h2>" +
                   "<table>" +
                   "<tr>" +
                   "<th>Equipment Name</th>" +
                   "<th>Current Quantity</th>" +
                   "<th>Threshold Quantity</th>" +
                   "</tr>";

            foreach (var item in orders)
            {
                Body += "<tr>" +
                        "<td>" + item.Key + "</td>" +
                        "<td>" + item.Value.Key + "</td>" +
                        "<td>" + item.Value.Value + "</td>" +
                       "</tr>";
            }

            Body +=
            "</table>" +
            "</body>" +
            "</html>";

            MailMessage mail = new MailMessage();
            mail.To.Add("your_email@gmail.com");
            mail.From = new MailAddress("your_email@gmail.com");
            mail.Subject = "EQUIPMENTS BELOW THRESHOLD VALUE ALERT";
            mail.Body = Body;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("your_email@gmail.com", "your_password"); // Enter senders User name and password  
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
        
        //SEND MAIL FUNCTION TO call sendemail
        public void SendMailFunction(Equipment equipment)
        {
            try
            {
                var totalVentilators = db.Equipments.Where(x => x.EquipmentName.Contains("Ventilator") && x.PatientId == null).Count();
                var totalbedsbyGW = db.Equipments.Where(x => x.EquipmentName.Contains("bed") && x.EquipmentCategoryId == 4 && x.PatientId == null).Count();
                var totalbedsbyICU = db.Equipments.Where(x => x.EquipmentName.Contains("bed") && x.EquipmentCategoryId == 5 && x.PatientId == null).Count();
                var totalAlcoholicsanitizers = db.Equipments.Where(x => x.EquipmentName.Contains("Alcoholic Sanitizer")).Select(x => x.EquipmentTotalQuantity).FirstOrDefault();
                var totalNonalcoholicsanitizers = db.Equipments.Where(x => x.EquipmentName.Contains("Non-Alcoholic Sanitizer")).Select(x => x.EquipmentTotalQuantity).FirstOrDefault();
                var totalSymtomaticmedicines = db.Equipments.Where(x => x.EquipmentName.Contains("Symptomatic")).Select(x => x.EquipmentTotalQuantity).FirstOrDefault();
                var totalAsymtomaticmedicines = db.Equipments.Where(x => x.EquipmentName.Contains("Asymptomatic")).Select(x => x.EquipmentTotalQuantity).FirstOrDefault();
                var totalN95Mask = db.Equipments.Where(x => x.EquipmentName.Contains("N-95")).Select(x => x.EquipmentTotalQuantity).FirstOrDefault();
                var totalsurgicalmask = db.Equipments.Where(x => x.EquipmentName.Contains("Surgical")).Select(x => x.EquipmentTotalQuantity).FirstOrDefault();

                var rows = db.EquipmentCategories.ToList();

                Dictionary<string, int> equipmentthreshold = new Dictionary<string, int>();

                foreach (var row in rows)
                {
                    equipmentthreshold.Add(row.EquipmentCategoryName, row.EquipmentCategoryThreshold);
                }

                Dictionary<string, KeyValuePair<int, int>> orders = new Dictionary<string, KeyValuePair<int, int>>();

                if (totalAlcoholicsanitizers < equipmentthreshold["Sanitizer"])
                    orders.Add("Alcoholic Sanitizer", new KeyValuePair<int, int>(totalAlcoholicsanitizers, equipmentthreshold["Sanitizer"]));

                if (totalNonalcoholicsanitizers < equipmentthreshold["Sanitizer"])
                    orders.Add("Non-Alcoholic Sanitizer", new KeyValuePair<int, int>(totalNonalcoholicsanitizers, equipmentthreshold["Sanitizer"]));

                if (totalsurgicalmask < equipmentthreshold["Mask"])
                    orders.Add("Surgical Mask", new KeyValuePair<int, int>(totalsurgicalmask, equipmentthreshold["Mask"]));

                if (totalN95Mask < equipmentthreshold["Mask"])
                    orders.Add("N-95 Mask", new KeyValuePair<int, int>(totalN95Mask, equipmentthreshold["Mask"]));

                if (totalbedsbyGW < equipmentthreshold["General Ward Bed"])
                    orders.Add("General Ward Bed", new KeyValuePair<int, int>(totalbedsbyGW, equipmentthreshold["General Ward Bed"]));

                if (totalbedsbyICU < equipmentthreshold["ICU Ward Bed"])
                    orders.Add("ICU Ward Bed", new KeyValuePair<int, int>(totalbedsbyICU, equipmentthreshold["ICU Ward Bed"]));

                if (totalSymtomaticmedicines < equipmentthreshold["Malarial Medicine"])
                    orders.Add("Malarial Medicine", new KeyValuePair<int, int>(totalSymtomaticmedicines, equipmentthreshold["Malarial Medicine"]));

                if (totalAsymtomaticmedicines < equipmentthreshold["Paracetemol Medicine"])
                    orders.Add("Paracetemol Medicine", new KeyValuePair<int, int>(totalAsymtomaticmedicines, equipmentthreshold["Paracetemol Medicine"]));

                if (totalVentilators < equipmentthreshold["Ventilator"])
                    orders.Add("Ventilator", new KeyValuePair<int, int>(totalVentilators, equipmentthreshold["Ventilator"]));

                if (orders.Count() > 0)
                    sendemail(orders);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in sending mail");
                Debug.WriteLine(e.Message.ToString());
            }
        }

        // GET: Equipments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipments.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // GET: Equipments/Create
        public ActionResult Create()
        {
            ViewBag.EquipmentCategoryId = new SelectList(db.EquipmentCategories, "EquipmentCategoryId", "EquipmentCategoryName");
            return View();
        }

        // POST: Equipments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EquipmentId,EquipmentName,EquipmentCategoryId,EquipmentTotalQuantity,EquipmentInUseCount,EquipmentSellerName,PatientId")] Equipment equipment)
        {

            //CHECK if the equipment name already exists
            string EquipmentNameCheck = "";
            EquipmentNameCheck = db.Equipments.Where(x => x.EquipmentName.Equals(equipment.EquipmentName)).Select(x => x.EquipmentName).SingleOrDefault();
            if (EquipmentNameCheck == null)
            {
                if (ModelState.IsValid)
                {
                    ViewBag.message = null;
                    db.Equipments.Add(equipment);
                    SendMailFunction(equipment);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.EquipmentCategoryId = new SelectList(db.EquipmentCategories, "EquipmentCategoryId", "EquipmentCategoryName", equipment.EquipmentCategoryId);
                return View(equipment);
            }
            else
            {
                ViewBag.EquipmentCategoryId = new SelectList(db.EquipmentCategories, "EquipmentCategoryId", "EquipmentCategoryName", equipment.EquipmentCategoryId);
                ViewBag.message = String.Format("Creation Failed! Equipment with name as {0} already exists", equipment.EquipmentName);
                return View(equipment);
            }
        }

        // GET: Equipments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipments.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            ViewBag.EquipmentCategoryId = new SelectList(db.EquipmentCategories, "EquipmentCategoryId", "EquipmentCategoryName", equipment.EquipmentCategoryId);
            return View(equipment);
        }

        // POST: Equipments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EquipmentId,EquipmentName,EquipmentCategoryId,EquipmentTotalQuantity,EquipmentInUseCount,EquipmentDateOfExpiry,EquipmentSellerName,PatientId")] Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipment).State = EntityState.Modified;
                db.SaveChanges();
                SendMailFunction(equipment);
                return RedirectToAction("Index");
            }
            ViewBag.EquipmentCategoryId = new SelectList(db.EquipmentCategories, "EquipmentCategoryId", "EquipmentCategoryName", equipment.EquipmentCategoryId);
            return View(equipment);
        }

        // GET: Equipments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipments.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // POST: Equipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Equipment equipment = db.Equipments.Find(id);
            db.Equipments.Remove(equipment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
