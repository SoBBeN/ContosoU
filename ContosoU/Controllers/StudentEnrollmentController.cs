using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoU.Data;
using ContosoU.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ContosoU.Controllers
{
    //[Authorize]//simple need to be logged in
    [Authorize(Roles = "student")] //need to be logged in and part of student role
    public class StudentEnrollmentController : Controller
    {
        private readonly SchoolContext _context;
        private readonly UserManager<ApplicationUser> _userManager; //BPoirier: need Identity users
        public StudentEnrollmentController(SchoolContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: StudentEnrollment

        public async Task<IActionResult> Index()
        {
            //BPoirier: retrieve currently logged in student
            var user = await GetCurrentUserAsync();

            //if user is not loged in
            if (user == null)
            {
                //not logged in
                return NotFound();//TODO: return Error View
                //return View("Error");
            }

            //Locate logged in user (student) within the Student Entity
            var student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .AsNoTracking()
                //assiciate identity user and username are the same
                //Note: by default email and username are the same
                .SingleOrDefaultAsync(m => m.Email == user.Email);//associate identity user -> student
                                                                  //You may also want to assciate by id instead of email:(m => m.Id = user.Id)

            //1. =============Course Enrolled: (student is enrolled in these) =================
            var studendEnrollments = _context.Enrollments
                .Include(c => c.Course)
                //.Include(c => c.Student)
                .Where(c => c.StudentID == student.ID)
                .AsNoTracking();

            ViewData["StudentName"] = student.FullName;

            //2. =============Course Available:: (student is NOT enrolled in these) =================
            string query = "SELECT CourseID, Credits, Title, DepartmentID " +
                           "FROM Course " +
                           "WHERE CourseID NOT IN (SELECT DISTINCT CourseID " +
                                                  "FROM ENrollment " +
                                                  "WHERE StudentID = {0})";
            //Building a RAW SQL Query using linQ (Language Integrated Query)
            var courses = _context.Courses
                .FromSql(query, student.ID)
                .AsNoTracking();

            //ViewData["Courses"] = courses.Tolist(); //this is the same as ViewBag.Courses = courses.ToList();
            ViewBag.Courses = courses.ToList(); //The (Courses) name is what you want it to be, it could be johnny or anything else

            return View(await studendEnrollments.ToListAsync());
        }


        //Method we created
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        // GET: Enroll
         public async Task<IActionResult> Enroll(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //get currently logged in student
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
                    //to do return error view
            }
            //Locate the logged in user (student) within the Student Entity
            var student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(s => s.Course)
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Email == user.Email);

            //Return student ID to view using ViewData
            ViewData["StudentID"] = student.ID; // for hidden field in form (so we know who they are)

            //retrieve this student's current enrollment
            //(for comparison with course they want to enroll)
            var studentEnrollments = new HashSet<int>(_context.Enrollments
                .Include(e => e.Course)
                //.Include(e => e.Student)
                .Where(e => e.StudentID == student.ID)
                .Select(e => e.CourseID));//Only select CourseID

            //convertsion from int? to int is not possible (need int)
            int currentCourseID;
            if (id.HasValue)//id is the method parameter
            {
                currentCourseID = (int)id;
            }
            else
            {
                currentCourseID = 0;
            }
            //end conversion fix

            //Situation where student tries to enroll in same course
            if (studentEnrollments.Contains(currentCourseID))
            {
              
                //Student is trying to enroll in a course, already enrolled in
                // send a model state error back to view
                ModelState.AddModelError("AlreadyErolled", "You are already Enrolled in this course!");
            }

            //situation were student tries to enroll in a none existing course
            var course = await _context.Courses.SingleOrDefaultAsync(c => c.CourseID == id);
            //if course was not instantiated because no course id was found based on param coming in
            //for example Studentenrollment/Enroll/9000
            //9000 is not a valid course
            //return not found
            if (course == null)
            {
                return NotFound();
            }
            //otherwise return the view (with course entity)
            return View(course);
        }

        // POST: Enroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll([Bind("CourseID,CourseID")] Enrollment enrollment)
        {
            _context.Add(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.EnrollmentID == id);
        }
    }
}
