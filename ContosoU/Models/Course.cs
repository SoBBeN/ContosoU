﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoU.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //You can turn off IDENTITY(auto increments) propery by using the DatabaseGeneratedOption.None
        //You have the following 3 option:
        // Computed: Database generates a value when a row is inserted or updated.
        // Identity: Database generates a value when a row is inserted.
        // None: Database generates a value.
        [Display(Name ="Course Number")]
        public int CourseID { get; set; }//PK
        [StringLength(50,MinimumLength = 3)]
        public string Title { get; set; }
        [Range(0,5)]
        public int Credits { get; set; }

        //================ NAVIGATION PROPERTIES ==================== //
        //1 course: many enrollments
        public virtual ICollection<Enrollment> Enrollments { get; set; }


    }

}