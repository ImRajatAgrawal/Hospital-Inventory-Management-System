using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HospitalInventory.Models;

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
            if (ModelState.IsValid)
            {
                db.Equipments.Add(equipment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EquipmentCategoryId = new SelectList(db.EquipmentCategories, "EquipmentCategoryId", "EquipmentCategoryName", equipment.EquipmentCategoryId);
            return View(equipment);
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
