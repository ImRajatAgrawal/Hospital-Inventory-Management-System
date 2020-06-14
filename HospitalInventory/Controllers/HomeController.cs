using HospitalInventory.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace HospitalInventory.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HospitalInventoryEntities db = new HospitalInventoryEntities();
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

        public ActionResult Manager()
        {
            ViewBag.Message = "Manager page";

            return View();
        }

        public ActionResult ErrorPage() {
            ViewBag.Message = "Error !! Access Denied to Page!!";
            return View();
        }

    }
}