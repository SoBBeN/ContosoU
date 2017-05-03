using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU.Models
{
    public class Instructor : Person
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        //=================== NAVIGATION PROPERTIES ===================//
        //An Insructor can teach any number of courses, so Courses is defined as a collection
        //pf the Course Assignment Entity.
        public virtual ICollection<CourseAssignment> Courses { get; set; }

        //An instructor can only have at most one office, so the office assignment property holds a single
        //Office assignment Entity(which may be null of not office is assigned)
        public virtual OfficeAssignment OfficeAssignment { get; set; }


    }
}
