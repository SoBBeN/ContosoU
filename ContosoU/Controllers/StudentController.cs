using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoU.Data;
using ContosoU.Models;
using ContosoU.Helpers;

namespace ContosoU.Controllers
{
    public class StudentController : Controller
    {
        private readonly SchoolContext _context;

        public StudentController(SchoolContext context)
        {
            _context = context;    
        }

        // GET: Student
        public async Task<IActionResult> Index(string sortOrder, 
                                               string searchString,
                                               string currentFilter,
                                               int?  page)
        {
            /*NOTES
                   -sortOrder: for Sorting
                   -searchString: for Searching
                   -currentFilter: to keep current search
                   -page: for paging (optional argument)
             */

            ViewData["CurrentSort"] = sortOrder;


            //return View(await _context.Students.ToListAsync());      //********(this is what we had before)*********
            //BPoirier: add paging, sorting and filtering functionality
            /*
             * This is how you query the datebase STUDY THIS!!!!
             */
            var students = from s in _context.Students
                           select s;//SELECT FirstName, LastName, Email, EnrollmentDate FROM Students

            //Part 1: For Sorting (working with ViewData Object)

            //Default Sort LastName
            ViewData["LNmaeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "lname_desc" : "";

            //Other Sort Order // Toggle between ascending and descending
            ViewData["FNmaeSortParm"] = sortOrder == "fname" ? "fname_desc" : "fname";
            ViewData["EmailSortParm"] = sortOrder == "email" ? "email_desc" : "email";  //NOTICE THE email_acending ---->>>>>sortOrder == "email" ? "email_desc" : "email_acending";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";


            //Part 2: filtering (When User Search for a specific thingBP)
            if (searchString == null)
            {
                searchString = currentFilter;
            }
            else
            {
                page = 1;//Start on first page
                /*
                 * If the searchstring is changed durent paging, the page has to be reset to 1
                 * because the new filter can result in different data to display
                 * 
                 */
            }
            
            ViewData["CurrentFilter"] = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                //User entered a search criteria(Request)-> search by last name or first name
                students = students.Where(s => s.LastName.Contains(searchString) || s.FirstName.Contains(searchString));
            }


            //Apply the Sorting
            switch (sortOrder)
            {
                case "lname_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                    //FirstName
                case "fname":
                    students = students.OrderBy(s => s.FirstName);
                    break;
                case "fname_desc":
                    students = students.OrderByDescending(s => s.Email);
                    break;
                //email
                case "email":
                    students = students.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    students = students.OrderByDescending(s => s.FirstName);
                    break;
                //Date
                case "date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }
            //Bpoirier: changed to use paginated list
            //return View(await students.ToListAsync());
            int pageSize = 5;
            return View(await PaginatedList<Student>.CreateAsync(students,page ?? 1,pageSize));
            //double question(??) is the null-coalescing operator
            //page the value of page unless page is null, in which case it is assigned the value of 1
        }

        // GET: Student/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .SingleOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Student/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentDate,ID,LastName,FirstName,Email")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Student/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.SingleOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentDate,ID,LastName,FirstName,Email")] Student student)
        {
            if (id != student.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.ID))
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
            return View(student);
        }

        // GET: Student/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .SingleOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.SingleOrDefaultAsync(m => m.ID == id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}
