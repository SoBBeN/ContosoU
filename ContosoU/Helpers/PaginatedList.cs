﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU.Helpers
{
    public class PaginatedList<T>:List<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }
        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }
        /*
         * Th CreateAsync method tales the page size and page number and applies the appropriate Skip and Take
         * Statements to the IQueryable source object (This could be student,Instructor,course,etc)
         * 
         * The properties HasPreviousPage and HasNextPage can be uses to enable and disable Provious
         * and Next paging button in the UI
         * 
         * The CreateAsync method is used instead of a constructor to create the Paginated List
         * object because Constuctors can't run asynChronous code.
         * 
         * 
         * 
         */
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items,count,pageIndex,pageSize);
            
        }

    }
}
