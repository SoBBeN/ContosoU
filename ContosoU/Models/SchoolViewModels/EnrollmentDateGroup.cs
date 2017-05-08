using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU.Models.SchoolViewModels
{
    public class EnrollmentDateGroup
    {
        /*
         * This Class is considered to be a ViewModel, A ViewModel allows you to shape
         * multiple entities into a single object, optimized for consumption and rendering
         * by the VIew. The purpose of the View Model is for the view to have a single object
         * to render
         */

            [DataType(DataType.Date)] 
            /*
                  Data Annotations DataType property
                 -for Formatting date like 9/1/2005 within view
                 -without it: 9/1/2005 12:00:00 AM
             */
        public DateTime EnrollmentDate { get; set; }
        public int StudentCount { get; set; }
    }
}
