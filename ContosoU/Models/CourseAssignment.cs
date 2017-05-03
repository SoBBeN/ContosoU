using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU.Models
{
    public class CourseAssignment
    {


        //[Key]
        //[ForeignKey("Instructor")]
        public int InstructorID { get; set; }//Composite PK, FK to Instructor Entity

        //[Key]
        //[ForeignKey("Course")]
        public int CourseID { get; set; } //Composite PK, FK to Course Entity

        /*
         * We could-label both properties with the [Key] attribute to create a composite PK
         * but we will do it using fluent-API within the SchoolContext Class
         * (Check SchoolContext ====MyNote 1==== for were we did this)
         */


        //===========================NAVIGATION PROPERTY=========================//
        //Many-to-Many(this is the junction or join table) between Instructor and Course
        //Many Instructors teaching many Courses
        //1 course many Course Assignments
        //11 Instructor many Course Assignments
        public virtual Instructor Instructor { get; set; }
        public virtual Course Course { get; set; }

    }
}
