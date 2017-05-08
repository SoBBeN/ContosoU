using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoU.Data;
using ContosoU.Models;
using Microsoft.AspNetCore.Authorization;

namespace ContosoU.Controllers
{
    [Authorize] //Need to be LOG IN to see the course View Bpoirier

    public class CourseController : Controller
    {
        private readonly SchoolContext _context;

        public CourseController(SchoolContext context)
        {
            _context = context;    
        }
        //Bpoirier: Reading related data
        //Build a custom method to return a sorted list of departments for
        //our dropdown filter
        private IQueryable<Course> GetCourses(int? SelectedDepartment)
        {
            //Get all department sorted by name
            var department = _context.Departments.OrderBy(d => d.Name).ToList();

            //Add ViewData for use within View
            ViewData["SelectedDepartment"] = new SelectList(department, "DepartmentID", "Name", SelectedDepartment);//dropdown??
            //Retrieve the value of incoming parameter
            int departmentId = SelectedDepartment.GetValueOrDefault();

            //Get courses belonging to that department
            IQueryable<Course> courses = _context.Courses
                //Where()
                //Where(DepartmentID == 1)
                .Where(c => !SelectedDepartment.HasValue || c.DepartmentID == departmentId)
                .OrderBy(d => d.CourseID)
                .Include(d => d.Department);

            return courses;
        }


        // GET: Course
        //old Index Code Bpoirier
        //public async Task<IActionResult> Index()
        //{
        //    var schoolContext = _context.Courses.Include(c => c.Department);
        //    return View(await schoolContext.ToListAsync());
        //}

        public async Task<IActionResult> Index(int? SelectedDepartment)
        {
            //The SelectedDepartment refers to a Select box (dropdown) within our view
            IQueryable<Course> courses = GetCourses(SelectedDepartment);
            return View(await courses.ToListAsync());
        }

        // GET: Course/Details/5
        [AllowAnonymous]//Bpoirier Let them see without being Log In
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .SingleOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Course/Create
        public IActionResult Create()
        {
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "Name");
            return View();
        }

        // POST: Course/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseID,Title,Credits,DepartmentID")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "Name", course.DepartmentID);
            return View(course);
        }

        // GET: Course/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.SingleOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "Name", course.DepartmentID);
            return View(course);
        }

        // POST: Course/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseID,Title,Credits,DepartmentID")] Course course)
        {
            if (id != course.CourseID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "Name", course.DepartmentID);
            return View(course);
        }

        // GET: Course/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .SingleOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.SingleOrDefaultAsync(m => m.CourseID == id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }



        
        [AllowAnonymous]//Bpoirier Let them see without being Log In

        public async Task<IActionResult> Listing(int? SelectedDepartment)
        {
            var courses = GetCourses(SelectedDepartment);

            return View(await courses.ToListAsync());
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseID == id);
        }
    }
}
