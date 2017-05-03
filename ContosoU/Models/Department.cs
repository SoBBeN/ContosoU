using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [DataType(DataType.Currency)]//This is for clientSide
        [Column(TypeName = "money")]//for the database turn this decimal in a money datatype
        public decimal Budget { get; set; }

        public int MyProperty { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Created")]
        public DateTime CreatedDate { get; set; }

        //Relationship to the Instructor
        //A department MAY have an Administrator (Instructor), and an
        //an Administrator is always and Instructor

        public int? InstructorID { get; set; }//Question mark make this nullable

        //================ NAVIGATION PROPERTIES ==================== //

        //Administrator is always an Instructor
        public virtual Instructor Administrator { get; set; }

        //One department with many courses (1:many)
        public virtual ICollection<Course> Courses { get; set; }

        //TO DO : Handle Concurrency conflicts (Add optimistic Concurrency)



    }
}
