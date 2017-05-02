using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU.Models
{
    public abstract class Person
    {
        //mwilliams: Create the data models
        public int ID { get; set; }
        [Display(Name ="Last Name")]
        [Required]
        [StringLength(65, ErrorMessage = "Last name cannot be longer then 65 characters. ")]
        public string LastName { get; set; }
        [Display(Name = "First Name")]
        [Required]
        [StringLength(50,ErrorMessage ="First name cannot be longer then 50 characters. ")]
        public string FirstName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        //FullName: Calculated property with a get accessor only 
        //        - will not get generated in database
        
            [Display(Name ="Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
        
            }
        }
    }
}
