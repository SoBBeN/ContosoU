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
using ContosoU.Models.SchoolViewModels;

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

            /*
                rewrite the one of the coalescing if
                if (sortOrder == "fname")
                {
                    sortOrder = "fname_desc";
                }
                else
                {
                    sortOrder = "fname";
                }
            */


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
        public async Task<IActionResult> Details (int? id)//the question mark make the int nullable
        {
            if (id == null)
            {
                return NotFound();
                /*
                 * NotFound() is a status Code
                 * 
                 * Statuc Codes
                 * Success:
                 * return OK() <- http statuc code 200
                 * return created() <- http status code 201
                 * return NoContent() <- http status code 204
                 * 
                 * Client Error:
                 * return BadRequest() <- http status 400
                 * return Unauthorized() <- http status 401
                 * return NotFound() <- http status 404
                 */
            }
            //Bpoirier: update to include related data (enrollment)
            //var student = await _context.Students
            //    .SingleOrDefaultAsync(m => m.ID == id);

            //querying the database again
            var student = await _context.Students
                .Include(s=>s.Enrollments).ThenInclude(c=>c.Course)//This is the join in SQL
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);

            /*
            * ============================================================== No-tracking queries ============================================================
            * When a database context retrieves table rows and creates entity objects that represent them, by default it keeps track of whether 
            * the entities in memory are in sync with what's in the database. The data in memory acts as a cache and is used when you update an entity. 
            * This caching is often unnecessary in a web application because context instances are typically short-lived 
            * (a new one is created and disposed for each request) and the context that reads an entity is typically disposed before that entity is used again.
            * 
            * Ref:  https://docs.microsoft.com/en-us/ef/core/querying/tracking
            */


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
        public async Task<IActionResult> Create([Bind("EnrollmentDate,LastName,FirstName,Email")] Student student)
        {
            //Bpoirier: remove the id from the bind attributes: ID is the PK as well as IDENTITY
            try
            {
                if (ModelState.IsValid)
                {
                 _context.Add(student);
                 await _context.SaveChangesAsync();
                 return RedirectToAction("Index");
                 }
            }
            catch (Exception /*ex*/)
            {
                //Log the error (by uncommenting the ex variable) and write to a log file
                //return a ModelStateError back to user
                ModelState.AddModelError("", "Unable to save changes. Please try again");
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
        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Find the student to be updated
            var studentToUpdate = await _context.Students.SingleOrDefaultAsync(s => s.ID == id);

            //try to update this student
            if (await TryUpdateModelAsync<Student>(
                studentToUpdate, "", s => s.FirstName, s => s.LastName, s => s.Email, s => s.EnrollmentDate))
            {
                try
                {
                    await _context.SaveChangesAsync();//Save changes back to database
                    return RedirectToAction("Index");//redirect user back to index route
                }
                catch (DbUpdateException /*ex*/)
                {
                    //Log the error (by uncommenting the ex variable) and write to a log file
                    //return a ModelStateError back to user
                    ModelState.AddModelError("", "Unable to save changes. " + "Please try again");
                }
            }

            //return the View and attach the studentToupdate model
            return View(studentToUpdate);

        }

        // GET: Student/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError=false)
        {
            //Bpoirier
            /*
                 * This code accept an optional boolean parameter that indicates whether the method was called after a 
                 * delete failure (failure saving changes back to database)
                 * When it is called by the HTTPpost Delete method in response to a database update error, this parameter
                 * will be passed in as set to true
             */
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

            //Bpoirier: return update error if necessary
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete failed! Try again later.";
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.AsNoTracking().SingleOrDefaultAsync(m => m.ID == id);

            //Bpoirier: check if student exists
            if (student == null)
            {
                return RedirectToAction("Index");
            }


            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateException/*ex*/)
            {
                //Return user to the Delete GET Method passing it the current student(ID)
                // and a Flag argument set to TRUE for reprensenting and saving record
                return RedirectToAction("Delete", new { id = id, saveChangesError = true } );
            }
        }
        //Bpoirier
        //GEt Student/Stats
        public async Task<IActionResult> Stats()
        {
            //Populate the EnrollmentDateGroup ViewModel with Student Statistics
            IQueryable<EnrollmentDateGroup> data =
                from student in _context.Students //FROM Students
                group student by student.EnrollmentDate into dateGroup //GROUP BY Enrollmentdate
                select new EnrollmentDateGroup //SELECT EnrollmentDate COUNT(*) as StudentCount
                {
                    EnrollmentDate = dateGroup.Key,
                    StudentCount = dateGroup.Count()
                };

            return View(await data.AsNoTracking().ToListAsync());
        }


        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}
