using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoU.Data;
using ContosoU.Models;
using ContosoU.Models.SchoolViewModels;

namespace ContosoU.Controllers
{
    public class InstructorController : Controller
    {
        private readonly SchoolContext _context;

        public InstructorController(SchoolContext context)
        {
            _context = context;    
        }

        // GET: Instructor
        public async Task<IActionResult> Index(int? id, int? courseID) //Add a optional param for Selected Instructor(id)
                                                                       //Add param for selected Course (courseID)
        {
            var viewModel = new InstructorIndexData();
            viewModel.Instructors = await _context.Instructors
                .Include(i => i.OfficeAssignment)//Include Office assigned to instructor
                //=============== Enrollment =============================
                .Include(i => i.Courses) //within courses property load the enrollments
                .ThenInclude(i => i.Course) // have to get the course entity out of the Courses Join entity
                .ThenInclude(i => i.Department)
                .OrderBy(i => i.LastName) // Orders by last Name
                //.AsNoTracking()//optional but improve preformence
                .ToListAsync();

            //==========================Instructor Selected==============================//
            if (id != null)//if instructor param (id) is passed in
            {
                //Get the instructor data
                Instructor instructor = viewModel.Instructors.Where(i => i.ID == id.Value).Single();//returns a Single Entity
               
                //Now get instructor courses
                viewModel.Courses = instructor.Courses.Select(s => s.Course);
                
                //Get instructor name
                @ViewData["InstructorName"] = instructor.FullName;

                //Return the Instructor id(id) back to the view for Highlighting Selected Row
                @ViewData["InstructorID"] = id.Value; //passed in the URL parameter
                //OR
                //@ViewData["InstructorID"] = instructor.ID;
            }

            //==========================End Instructor Selected ============================//

            //==========================Course Selected =====================================//
            if (courseID != null)
            {
                //Get all enrollments for this course (explicit loadingL loading only if requested)
                _context.Enrollments.Include(i => i.Student)
                    .Where(c => c.CourseID == courseID.Value).Load();

                viewModel.Enrollments = viewModel.Courses.Where(x => x.CourseID == courseID).Single().Enrollments; //one Enrollments
                //only enrollment for a single selected course (courseid = ****)

                //we do not want all enrollments
                //viewModel.Enrollments = _context.Enrollments;// ALL Enrollments

                ViewData["CourseID"] = courseID.Value; // Pass the CourseID view data back to view
            }



            //=======================  End Course Selected ==================================//

            return View(viewModel);

            //Original Scchforded code
            //return View(await _context.Instructors.ToListAsync());


        }

        // GET: Instructor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.OfficeAssignment) //BPoirier: include office
                .SingleOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructor/Create
        public IActionResult Create()
        {
            var instructor = new Instructor();
            instructor.Courses = new List<CourseAssignment>();
            //Populate the AssignedCourseData View Model
            PopulateAssignedCourseData(instructor);

            return View();
        }
        //Method I Created BPoirier
        private void PopulateAssignedCourseData(Instructor instructor)
        {
            //get all courses
            var allCourses = _context.Courses;

            //create a hashset of instructor courses
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));

            //Create and populate the AssignedCourseData ViewModel
            var viewModel = new List<AssignedCourseData>();//create

            //populate it once for each of the courses within allCourses
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });
            }

            //Save the viewModel within the ViewData object for use within View
            ViewData["Courses"] = viewModel;
        }

        // POST: Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HireDate,LastName,FirstName,Email,OfficeAssignment")] Instructor instructor,string[] selectedCourses)
        {
            //BPoirier: added string[] selectedCourses method argument for course assignment
            if (selectedCourses != null)
            {
                //selectedCourses checkboxes have been check - Create a new list of CourseAssignment
                instructor.Courses = new List<CourseAssignment>();
                //Loop the selectedCourses array
                foreach (var course in selectedCourses)
                {
                    //populate the CourseAssignment (InstructorID,CourseID)
                    var courseToAdd = new CourseAssignment
                    {
                        InstructorID = instructor.ID,
                        CourseID = int.Parse(course)
                    };
                    instructor.Courses.Add(courseToAdd);
                }
            }


            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(instructor);
        }

        // GET: Instructor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.OfficeAssignment)//include OfficdAssignment
                .Include(i => i.Courses) //include courses for AssignedCourseDate viewModel
                .SingleOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }
            //Populate the AssignedCourseData View Model
            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedCourses/* [Bind("HireDate,ID,LastName,FirstName,Email")] Instructor instructor*/)
        {
            //BPoirier: Take care of overposting
            //          Added selectedCourse string array argument

            if (id == null)
            {
                return NotFound();
            }
            //Find instructor to update

            var instructorToUpdate = await _context.Instructors
                .Include(i => i.OfficeAssignment)//include office assignment
                .Include(i => i.Courses)//incluse Courses for course assigment
                .ThenInclude(i => i.Course)//for update course
                .SingleOrDefaultAsync(i => i.ID == id); //only one instructor to update (based on id)


            if (await TryUpdateModelAsync<Instructor>(
                instructorToUpdate, "", i => i.FirstName, i => i.LastName, i => i.OfficeAssignment)) //This part replaces the bind!!!
            {
                //check for empty string on office location
                if (string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                {
                    instructorToUpdate.OfficeAssignment = null;//remove the complete record
                }

                //TO DO: Update Courses
                UpdateInstructorCourses(selectedCourses, instructorToUpdate);
                //TO DO: Save changes (try...catch)
                if(ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(instructorToUpdate);
                        await _context.SaveChangesAsync();
                    }
                    catch(DbUpdateException /*es*/)
                    {
                        //We could lodthe error using the ex argument
                        //Let's simply return a model state error back to the view
                        ModelState.TryAddModelError("", "Enable to save changes.");
                    }
                    return RedirectToAction("Index");
                    
                }
                
            }
            return View(instructorToUpdate);
        }//End of Post

        //Method that we created
        private void UpdateInstructorCourses(string[] selectedCourses,Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {
                //If no checkboxes were selected, initialize the Courses navagation property
                //with an empty collection and return
                instructorToUpdate.Courses = new List<CourseAssignment>();
                return;
            }

            //To facilitate  efficient lookups, 2 collection will be stored in HashSet objects
            //: selectedCourseHS -> select course hashset of checkboxe selections
            //: instructorCourses -> instructor courses (hashset of courses assigned to instructor)
            var selectedCourseHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>
                (instructorToUpdate.Courses.Select(c => c.Course.CourseID));

            //Loop through all courses in the database and check each course against ones
            //currently assigned to the instructor versus the ones that were selected in the
            //view
            foreach (var course in _context.Courses) //Loop all courses
            {
                //CONDITION 1:
                //If the checkbox for a course was selected but the course isn't in the
                //Instructor.Couses navigation property, the course is added to the collection
                //in the navigation property
                if (selectedCourseHS.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Add(new CourseAssignment
                        {
                            InstructorID = instructorToUpdate.ID,
                            CourseID = course.CourseID
                        });
                    }

                }
                //CONDITION 2:
                //If the check box for a course wasn't selected, but the course is in the
                //Instructor.Courses navigation property, the course is removed
                //from the navigation property.
                else
                {
                    if (instructorCourses.Contains(course.CourseID))
                    {
                        CourseAssignment courseToRemove =
                            instructorToUpdate.Courses
                            .SingleOrDefault(i => i.CourseID == course.CourseID);
                        _context.Remove(courseToRemove);
                    }
                }
            }//end foreach


        }

        // GET: Instructor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .SingleOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.SingleOrDefaultAsync(m => m.ID == id);
            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.ID == id);
        }
    }
}
