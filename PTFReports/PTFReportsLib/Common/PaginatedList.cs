/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace PTF.Reports.Common
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex
        {
            get;
            private set;
        }

        public int PageSize
        {
            get;
            private set;
        }

        public int TotalCount
        {
            get;
            private set;
        }

        public int TotalPages
        {
            get;
            private set;
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex + 1 < TotalPages);
            }
        }

        public string IndexOfTotal
        {
            get
            {
                return string.Format("{0} of {1}", PageIndex, TotalPages - 1);
            }
        }

        public PaginatedList(IList<T> source, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            this.AddRange(source.Skip(PageIndex * PageSize).Take(PageSize));
        }

        public PaginatedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            this.AddRange(source.Skip(PageIndex * PageSize).Take(PageSize));
        }
    }
}
