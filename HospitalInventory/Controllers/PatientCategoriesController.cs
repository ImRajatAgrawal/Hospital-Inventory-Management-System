﻿using System;
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
    public class PatientCategoriesController : Controller
    {
        private HospitalInventoryEntities db = new HospitalInventoryEntities();

        // GET: PatientCategories
        public ActionResult Index()
        {
            return View(db.PatientCategories.ToList());
        }

        // GET: PatientCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientCategory patientCategory = db.PatientCategories.Find(id);
            if (patientCategory == null)
            {
                return HttpNotFound();
            }
            return View(patientCategory);
        }

        // GET: PatientCategories/Create
        public ActionResult Create()
        {
            var items = db.Equipments.ToList();
            if (items != null)
            {
                ViewBag.data = items;
            }
            return View();
        }

        public JsonResult GetEquipment(string searchTerm)
        {
            var Equipment = db.Equipments.ToList();

            if (searchTerm != null)
            {
                Equipment = db.Equipments.Where(x => (x.EquipmentCategory.EquipmentCategoryIsReusable.Equals(1) && x.EquipmentCategory.EquipmentCategoryName.Contains(searchTerm))
                                  || (x.EquipmentCategory.EquipmentCategoryIsReusable.Equals(0) && x.EquipmentName.Contains(searchTerm))).ToList();
            }

            var modifiedequipent = Equipment.Select(x => new
            {
                id = x.EquipmentId,
                text = x.EquipmentName
            });
            return Json(modifiedequipent, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddEquipement(string EquipmentIds)
        {
            List<int> EquipmentIdList = new List<int>();
            EquipmentIdList = EquipmentIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

            // List<Equipment> EquipmentNameList = new List<Equipment>();
            List<string> EquipmentNameList = new List<string>();
            foreach (int Equipmentid in EquipmentIdList)
            {
                db.Configuration.ProxyCreationEnabled = false;
                var equipmentList = db.Equipments.Where(x => x.EquipmentId == Equipmentid).ToList();
                foreach (var item in equipmentList)
                {
                    EquipmentNameList.Add(item.EquipmentName);
                }
            }
            var names = String.Join(",", EquipmentNameList);
            ViewData["eqplist"] = names;

            return Json(names, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveRecord(PatientCategory model, string eqplist)
        {
            try
            {
                PatientCategory pc = new PatientCategory();

                pc.EquipmentName = eqplist;
                pc.PatientCategoryName = model.PatientCategoryName;

                db.PatientCategories.Add(pc);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("Create");
        }

        // POST: PatientCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PatientCategoryId,PatientCategoryName,EquipmentName")] PatientCategory patientCategory)
        {
            if (ModelState.IsValid)
            {
                db.PatientCategories.Add(patientCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View(patientCategory);
        }

        // GET: PatientCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientCategory patientCategory = db.PatientCategories.Find(id);
            if (patientCategory == null)
            {
                return HttpNotFound();
            }
            return View(patientCategory);
        }

        // POST: PatientCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PatientCategoryId,PatientCategoryName,EquipmentName")] PatientCategory patientCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patientCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(patientCategory);
        }

        // GET: PatientCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientCategory patientCategory = db.PatientCategories.Find(id);
            if (patientCategory == null)
            {
                return HttpNotFound();
            }
            return View(patientCategory);
        }

        // POST: PatientCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PatientCategory patientCategory = db.PatientCategories.Find(id);
            db.PatientCategories.Remove(patientCategory);
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
