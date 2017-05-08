using System.ComponentModel.DataAnnotations;

namespace ContosoU.Models
{
 public class Enrollment
    {
        public int EnrollmentID { get; set; }//PK
        /*
         * The CourseID property is a foreign key, and the corresponding navigation property
         * is Course. An Enrollment Entity is associated with one Course Entity
         */

        public int CourseID { get; set; }//FK
        public int StudentID { get; set; }//FK


        /*
         * The StudentID property is a foreign key, and the corresponding navigation property 
         * is Student. An Enrollment entity is accociated with one Student entity,
         * so te property can only hold a single Student entity
         * 
         * Entity Framework interprets a property as a foreign key property if it's named
         * <navigation property name><primary key property name> for example:
         * StudentID for the Student navigation property, since the Student entity's
         * primary key is ID (Inherits from Person Entity ID property in this case)
         * 
         * foreign key properties can also be named sinple <primary key property name> for enample,
         * CourseID, since the Course Entity's primary key is CourseID
         * 
         */

        //Show "No Grade" instead of blank when Grade is NULL
        [DisplayFormat(NullDisplayText = "No Grade")]
        public Grade? Grade { get; set; }//The questing mark beside the variable type makes this nullable
        //We made Grade nullable because we dont get a grade right away



        //========================== Navigation Properties ===========================//
        public virtual Course Course{ get; set; }
        public virtual Student Student { get; set; }
    }
    //Grade enumeration
    public enum Grade
    {
        A, B, C, D, F
    }
}