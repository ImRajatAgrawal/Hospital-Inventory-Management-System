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
    public class EquipmentCategoriesController : Controller
    {
        private HospitalInventoryEntities db = new HospitalInventoryEntities();

        // GET: EquipmentCategories
        public ActionResult Index()
        {
            return View(db.EquipmentCategories.ToList());
        }

        // GET: EquipmentCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentCategory equipmentCategory = db.EquipmentCategories.Find(id);
            if (equipmentCategory == null)
            {
                return HttpNotFound();
            }
            return View(equipmentCategory);
        }

        // GET: EquipmentCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EquipmentCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EquipmentCategoryId,EquipmentCategoryName,EquipmentCategoryIsReusable,EquipmentCategoryQuantity,EquipmentCategoryThreshold")] EquipmentCategory equipmentCategory)
        {
            if (ModelState.IsValid)
            {
                db.EquipmentCategories.Add(equipmentCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(equipmentCategory);
        }

        // GET: EquipmentCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentCategory equipmentCategory = db.EquipmentCategories.Find(id);
            if (equipmentCategory == null)
            {
                return HttpNotFound();
            }
            return View(equipmentCategory);
        }

        // POST: EquipmentCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EquipmentCategoryId,EquipmentCategoryName,EquipmentCategoryIsReusable,EquipmentCategoryInUseCount,EquipmentCategoryThreshold")] EquipmentCategory equipmentCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipmentCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(equipmentCategory);
        }

        // GET: EquipmentCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentCategory equipmentCategory = db.EquipmentCategories.Find(id);
            if (equipmentCategory == null)
            {
                return HttpNotFound();
            }
            return View(equipmentCategory);
        }

        // POST: EquipmentCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EquipmentCategory equipmentCategory = db.EquipmentCategories.Find(id);
            db.EquipmentCategories.Remove(equipmentCategory);
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
