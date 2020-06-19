using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HospitalInventory.Models;
namespace HospitalInventory.Controllers
{
    public class EquipmentCheckoutController : Controller
    {
        // GET: EquipmentCheckout
        private HospitalInventoryEntities db = new Models.HospitalInventoryEntities();
        public ActionResult Index()
        {
            var equipmentcategories = db.EquipmentCategories.ToList();
            SelectList categories = new SelectList(equipmentcategories, "EquipmentCategoryId", "EquipmentCategoryName");
            ViewBag.CategoryList = categories;
            return View();
        }
        public ActionResult GetEquipment(int EquipmentCategoryId)
        {
                var equipments = db.Equipments.Where(x => x.EquipmentCategoryId == EquipmentCategoryId).ToList();
                var EquipmentNames = new SelectList(equipments, "EquipmentId","EquipmentName");
                ViewBag.EquipmentList = EquipmentNames;

            return PartialView("DisplayEquipments");
        }

        [HttpPost]
        public ActionResult Withdraw(EquipmentModel equipment)
        {
            using (db)
            {
                var equipments = db.Equipments.Where(equip => equip.EquipmentId == equipment.EquipmentId).SingleOrDefault();
                var equipmentcategory = db.EquipmentCategories.Where(equip => equip.EquipmentCategoryId == equipment.EquipmentCategoryId).SingleOrDefault();
                if (equipmentcategory.EquipmentCategoryIsReusable)
                {
                    equipments.PatientId = null;
                    equipments.EquipmentInUseCount = 0;
                    db.SaveChanges();
                    TempData["msg"] = "<script>alert('Withdraw Successfull');</script>";
                }
                else if (equipment.quantity >= 0 && equipments.EquipmentTotalQuantity >= equipment.quantity)
                {
                    equipments.EquipmentTotalQuantity = equipments.EquipmentTotalQuantity - equipment.quantity;
                    db.SaveChanges();

                    TempData["msg"] = "<script>alert('Withdraw Successfull');</script>";

                }
                else
                {
                    
                    TempData["msg"] = "<script>alert('Withdraw Unsuccessfull. Not Enough Quantity');</script>";
                }
            }
            return RedirectToAction("Index","EquipmentCheckout");
        }
    }
}