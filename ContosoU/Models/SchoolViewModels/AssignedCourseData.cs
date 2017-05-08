using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU.Models.SchoolViewModels
{
    public class AssignedCourseData
    {
        /*
         * To provide a list of course checkboxed with Course id and title as well as indicator
         * that the instructor is assigned or not assigned to a particular course, we are creating this
         * ViewModel class
         */
        public int CourseID { get; set; }  //for the CourseID
        public string Title { get; set; }  //for the Course Title
        public bool Assigned { get; set; } //For (is instructor assigned or not to this course?)

    }
}
