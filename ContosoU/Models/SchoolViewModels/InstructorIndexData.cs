using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU.Models.SchoolViewModels
{
    public class InstructorIndexData
    {
        /*
         *  The Instructors Index page will show data from  three different tables.
         *  reason we are create The InstructorIndexData ViewModel. It will include three
         *  properties each holding the data for one of the follow Tables:
         *  -Instructor
         *  -Course
         *  -Enrollment
         */

        public IEnumerable<Instructor> Instructors{ get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }








    }
}
