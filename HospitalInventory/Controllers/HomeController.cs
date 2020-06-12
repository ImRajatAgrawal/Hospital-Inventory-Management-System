using HospitalInventory.Models;
using System;
using System.Collections.Generic;
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
            var totalbeds = db.Equipments.Where(x => x.EquipmentName.Contains("bed")).Count();
            var totalbedsbyGW = db.Equipments.Where(x => x.EquipmentName.Contains("bed") && x.EquipmentCategoryId == 4 && x.PatientId == null).Count();
            var totalbedsbyICU = db.Equipments.Where(x => x.EquipmentName.Contains("bed") && x.EquipmentCategoryId == 5 && x.PatientId == null).Count();

            var totalAlcoholicsanitizers = db.Equipments.Where(x => x.EquipmentName.Contains("Alcoholic Sanitizer")).Select(x => x.EquipmentTotalQuantity).FirstOrDefault();
            var totalNonalcoholicsanitizers = db.Equipments.Where(x => x.EquipmentName.Contains("Non-Alcoholic Sanitizer")).Select(x => x.EquipmentTotalQuantity).FirstOrDefault();
            var totalsanitizers = totalNonalcoholicsanitizers + totalAlcoholicsanitizers;
            var totalVentilators = db.Equipments.Where(x => x.EquipmentName.Contains("Ventilator") && x.PatientId == null).Count();
            var totalSymtomaticmedicines = db.Equipments.Where(x => x.EquipmentName.Contains("Symptomatic")).Select(x => x.EquipmentTotalQuantity).FirstOrDefault();
            var totalAsymtomaticmedicines = db.Equipments.Where(x => x.EquipmentName.Contains("Asymptomatic")).Select(x => x.EquipmentTotalQuantity).FirstOrDefault();
            var totalMedicines = totalAsymtomaticmedicines + totalSymtomaticmedicines;
            var totalN95Mask = db.Equipments.Where(x => x.EquipmentName.Contains("N-95")).Select(x => x.EquipmentTotalQuantity).FirstOrDefault();
            var totalsurgicalmask = db.Equipments.Where(x => x.EquipmentName.Contains("Surgical")).Select(x => x.EquipmentTotalQuantity).FirstOrDefault();
            var totalMask = totalN95Mask + totalsurgicalmask;

            var mildpatient = Math.Min(totalbedsbyGW, Math.Min(totalsurgicalmask, totalAlcoholicsanitizers));
            var severepatient = Math.Min(totalVentilators, Math.Min(totalbedsbyICU, Math.Min(totalN95Mask, totalNonalcoholicsanitizers)));
            var symptomaticpatient = Math.Min(mildpatient, totalSymtomaticmedicines);
            var asymptomaticpatient = Math.Min(mildpatient, totalAsymtomaticmedicines);
            var mildpercent = (mildpatient * 100) / totalbeds;
            var severepercent = (severepatient * 100) / totalbeds;
            var symptomaticpercent = (symptomaticpatient * 100) / totalbeds;
            var asymptomaticpercent = (asymptomaticpatient * 100) / totalbeds;

            ViewBag.arr = new int[8];
            
            ViewBag.arr[0] = (440 - (440 * mildpercent) / 100);
            ViewBag.arr[1] = (440 - (440 * severepercent) / 100);
            ViewBag.arr[2] = (440 - (440 * symptomaticpercent) / 100);
            ViewBag.arr[3] = mildpatient;
            ViewBag.arr[4] = severepatient;
            ViewBag.arr[5] = symptomaticpatient;
            ViewBag.arr[6]= (440 - (440 * asymptomaticpercent) / 100);
            ViewBag.arr[7] = asymptomaticpatient;

            return View();
        }
        public ActionResult Admin() {
            ViewBag.Message = "Admin Page is under construction";
            return View();
        }
        public ActionResult Staff()
        {
            ViewBag.Message = "Staff Page is under construction";
            return View();
        }

        public ActionResult Manager()
        {
            return RedirectToAction("Index", "EquipmentCategories");
        }

        public ActionResult ErrorPage() {
            ViewBag.Message = "Error !! Access Denied to Page!!";
            return View();
        }
    }
}