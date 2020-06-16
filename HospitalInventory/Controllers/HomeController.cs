﻿using HospitalInventory.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace HospitalInventory.Controllers
{
    public class HomeController : Controller
    {
        HospitalInventoryEntities db = new HospitalInventoryEntities();
        public ActionResult Index()
        {
            var totalbedsbyGW = db.Equipments.Where(x => x.EquipmentCategoryId == 4 
                                    && x.PatientId == null).Count();
            var totalbedsbyICU = db.Equipments.Where(x => x.EquipmentCategoryId == 5 
                                    && x.PatientId == null).Count();

            var totalAlcoholicsanitizers = db.Equipments
                                            .Where(x => x.EquipmentName
                                            .Contains("Alcoholic Sanitizer"))
                                            .Select(x => x.EquipmentTotalQuantity)
                                            .FirstOrDefault();

            var totalNonalcoholicsanitizers = db.Equipments
                                                .Where(x => x.EquipmentName
                                                .Contains("Non-Alcoholic Sanitizer"))
                                                .Select(x => x.EquipmentTotalQuantity)
                                                .FirstOrDefault();
            
            var totalVentilators = db.Equipments
                                     .Where(x => x.EquipmentName
                                     .Contains("Ventilator") && x.PatientId == null)
                                     .Count();
            
            var totalSymptomaticmedicines = db.Equipments
                                              .Where(x => x.EquipmentName
                                              .Contains("Symptomatic"))
                                              .Select(x => x.EquipmentTotalQuantity)
                                              .FirstOrDefault();
            
            var totalAsymptomaticmedicines = db.Equipments
                                            .Where(x => x.EquipmentName
                                            .Contains("Asymptomatic"))
                                            .Select(x => x.EquipmentTotalQuantity)
                                            .FirstOrDefault();
           
            var totalN95Mask = db.Equipments
                                 .Where(x => x.EquipmentName
                                 .Contains("N-95"))
                                 .Select(x => x.EquipmentTotalQuantity)
                                 .FirstOrDefault();
            
            var totalsurgicalmask = db.Equipments
                                        .Where(x => x.EquipmentName
                                        .Contains("Surgical"))
                                        .Select(x => x.EquipmentTotalQuantity)
                                        .FirstOrDefault();
            
            Dictionary<string, int> equipmentcount = new Dictionary<string, int>();
            
            equipmentcount.Add("Ventilator", totalVentilators);
            equipmentcount.Add("Non-Alcoholic Sanitizer", totalNonalcoholicsanitizers);
            equipmentcount.Add("Alcoholic Sanitizer", totalAlcoholicsanitizers);
            equipmentcount.Add("Surgical Mask", totalsurgicalmask);
            equipmentcount.Add("N-95 Mask", totalN95Mask);
            equipmentcount.Add("Symptomatic Medicine", totalSymptomaticmedicines);
            equipmentcount.Add("Asymptomatic Medicine", totalAsymptomaticmedicines);
            equipmentcount.Add("ICU Ward Bed", totalbedsbyICU);
            equipmentcount.Add("General Ward Bed", totalbedsbyGW);

            
            var rows = db.PatientCategories.ToList();
            
            Dictionary<string, int> patientintake = new Dictionary<string, int>();
            
            foreach (var row in rows)
            {
                string[] requirements = row.EquipmentName.Split(',');
                int val=equipmentcount[requirements[0]];
                for (int j = 1; j < requirements.Length; j++)
                {
                    val = Math.Min(val, equipmentcount[requirements[j]]);
                }
                patientintake[row.PatientCategoryName] = val;
            }
            
            ViewBag.patientintake = patientintake;
            
            return View();
        }
        public ActionResult Admin() {
            ViewBag.Message = "Admin Page";
            return View();
        }
        public ActionResult Staff()
        {
            ViewBag.Message = "Staff Page";

            return View();
        }
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
            mail.To.Add("abc@gmail.com");
            mail.From = new MailAddress("xyz@gmail.com");
            mail.Subject = "EQUIPMENTS BELOW THRESHOLD VALUE ALERT";
            mail.Body = Body;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("xyz@gmail.com", "password"); // Enter seders User name and password  
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
        public ActionResult Manager()
        {
            ViewBag.Message = "Manager page";
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
                    orders.Add("Malarial Medicine", new KeyValuePair<int, int>(totalSymtomaticmedicines, equipmentthreshold["Symptomatic"]));
 
                if (totalAsymtomaticmedicines < equipmentthreshold["Paracetemol Medicine"])
                    orders.Add("Paracetemol Medicine", new KeyValuePair<int, int>(totalAsymtomaticmedicines, equipmentthreshold["Asymptomatic"]));
                
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
            return View();
        }

        public ActionResult ErrorPage() {
            ViewBag.Message = "Error !! Access Denied to Page!!";
            return View();
        }

    }
}