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
    public class PatientsController : Controller
    {
        private HospitalInventoryEntities db = new HospitalInventoryEntities();
        public static int patientCategoryBeforeEdit = 0;
        // GET: Patients
        public ActionResult Index()
        {
            var patients = db.Patients.Include(p => p.PatientCategory).Include(p => p.Employee);
            return View(patients.ToList());
        }

        // GET: Patients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            patient.BedId = db.Equipments.Where(x => x.PatientId == patient.PatientId && x.EquipmentCategory.EquipmentCategoryName.Contains("bed")).Select(x => x.EquipmentName).SingleOrDefault();
            patient.BedCategory = db.Equipments.Where(x => x.PatientId == patient.PatientId && x.EquipmentCategory.EquipmentCategoryName.Contains("bed")).Select(x => x.EquipmentCategory.EquipmentCategoryName).SingleOrDefault();
            return View(patient);
        }

        // GET: Patients/Create
        public ActionResult Create()
        {
            ViewBag.PatientCategoryId = new SelectList(db.PatientCategories, "PatientCategoryId", "PatientCategoryName");
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "employeeName");
            return View();
        }

        public void AllotEquipment(int patientId, int patientcategoryId)
        {
            var EquipmentRequiredList = db.PatientCategories.Where(x => x.PatientCategoryId.Equals(patientcategoryId)).Select(x => x.EquipmentName).SingleOrDefault();
            string[] EquipmentArray = EquipmentRequiredList.Split(',').Select(name => name.Trim()).ToArray();
            foreach (var item in EquipmentArray)
            {
                var requiredEquipment = db.Equipments.Where(eqipName => eqipName.EquipmentName.Equals(item)).SingleOrDefault();
                if (requiredEquipment != null)
                {
                    requiredEquipment.EquipmentInUseCount += 1;
                    requiredEquipment.EquipmentTotalQuantity -= 1;
                    db.Entry(requiredEquipment).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var requiredReusableEquipment = db.Equipments.Where(equipName => equipName.EquipmentCategory.EquipmentCategoryName.Equals(item) && equipName.PatientId == null).FirstOrDefault();
                    if (requiredReusableEquipment != null)
                    {
                        requiredReusableEquipment.EquipmentInUseCount = 1;
                        requiredReusableEquipment.PatientId = patientId;
                        db.Entry(requiredReusableEquipment).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }

        //Release the equipment after patient category is modified or patient is deleted
        public void ReleaseEquipment(int patientId, int patientcategoryId)
        {
            var EquipmentOccupiedList = db.PatientCategories.Where(x => x.PatientCategoryId.Equals(patientcategoryId)).Select(x => x.EquipmentName).SingleOrDefault();
            string[] EquipmentArray = EquipmentOccupiedList.Split(',').Select(name => name.Trim()).ToArray();
            foreach (var item in EquipmentArray)
            {
                var occupiedEquipment = db.Equipments.Where(eqipName => eqipName.EquipmentName.Equals(item)).SingleOrDefault();
                if (occupiedEquipment != null)
                {
                    occupiedEquipment.EquipmentInUseCount -= 1;
                    occupiedEquipment.EquipmentTotalQuantity += 1;
                    db.Entry(occupiedEquipment).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var occupiedReusableEquipment = db.Equipments.Where(x => x.EquipmentCategory.EquipmentCategoryName.Equals(item) && x.PatientId == patientId).SingleOrDefault();
                    if (occupiedReusableEquipment != null)
                    {
                        occupiedReusableEquipment.EquipmentInUseCount = 0;
                        occupiedReusableEquipment.PatientId = null;
                        db.Entry(occupiedReusableEquipment).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }

        // POST: Patients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PatientId,PatientCategoryId,EmployeeId")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Patients.Add(patient);
                db.SaveChanges();
                var patientIdList = db.Patients.Select(x => x.PatientId).ToList();
                AllotEquipment(patientIdList.LastOrDefault(), patient.PatientCategoryId);
                return RedirectToAction("Index");
            }

            ViewBag.PatientCategoryId = new SelectList(db.PatientCategories, "PatientCategoryId", "PatientCategoryName", patient.PatientCategoryId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "employeeName", patient.EmployeeId);
            return View(patient);
        }

        // GET: Patients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            patientCategoryBeforeEdit = patient.PatientCategoryId;
            ViewBag.PatientCategoryId = new SelectList(db.PatientCategories, "PatientCategoryId", "PatientCategoryName", patient.PatientCategoryId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "employeeName", patient.EmployeeId);
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PatientId,PatientCategoryId,EmployeeId")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patient).State = EntityState.Modified;
                db.SaveChanges();
                int patientCategoryAfterEdit = patient.PatientCategoryId;
                if (patientCategoryBeforeEdit != patientCategoryAfterEdit)
                {
                    ReleaseEquipment(patient.PatientId, patientCategoryBeforeEdit);
                    AllotEquipment(patient.PatientId, patientCategoryAfterEdit);
                }
                return RedirectToAction("Index");
            }
            ViewBag.PatientCategoryId = new SelectList(db.PatientCategories, "PatientCategoryId", "PatientCategoryName", patient.PatientCategoryId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "employeeName", patient.EmployeeId);
            return View(patient);
        }
        // GET: Patients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patient patient = db.Patients.Find(id);
            ReleaseEquipment(id, patient.PatientCategoryId);
            db.Patients.Remove(patient);
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
