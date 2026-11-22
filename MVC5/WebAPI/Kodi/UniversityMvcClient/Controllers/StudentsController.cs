using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using UniversityMvcClient.Models;
using UniversityMvcClient.Services;

namespace UniversityMvcClient.Controllers
{
    public class StudentsController : Controller
    {
        private const string Url = "api/students";

        // GET: Students
        public async Task<ActionResult> Index()
        {
            var students = await ApiClient.GetAsync<List<Student>>(Url);
            return View(students ?? new List<Student>());
        }

        // GET: Students/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var student = await ApiClient.GetAsync<Student>(Url + "/" + id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        public async Task<ActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            var ok = await ApiClient.PostAsync(Url, student);
            if (!ok)
            {
                ModelState.AddModelError("", "API nuk e pranoi studentin.");
                return View(student);
            }

            return RedirectToAction("Index");
        }

        // GET: Students/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var student = await ApiClient.GetAsync<Student>(Url + "/" + id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        // POST: Students/Edit
        [HttpPost]
        public async Task<ActionResult> Edit(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            var ok = await ApiClient.PutAsync(Url + "/" + student.Id, student);
            if (!ok)
            {
                ModelState.AddModelError("", "API nuk e ruajti ndryshimin.");
                return View(student);
            }

            return RedirectToAction("Index");
        }

        // GET: Students/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var student = await ApiClient.GetAsync<Student>(Url + "/" + id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await ApiClient.DeleteAsync(Url + "/" + id);
            return RedirectToAction("Index");
        }
    }
}
