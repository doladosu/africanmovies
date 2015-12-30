using System;

namespace AfricanMovies.Backend.Queries
{
    public class PaginationSettings
    {
        private int page;
        private int numberOfRecords;

        public PaginationSettings(int defaultPage = 1, int defaultPerPage = 10)
        {
            Page = defaultPage;
            PerPage = defaultPerPage;
        }

        internal int NumberOfRecords
        {
            private get { return numberOfRecords; }
            set
            {
                numberOfRecords = value;

                if (NumberOfPages < Page)
                    Page = NumberOfPages;
            }
        }

        internal int NumberOfRecordsToSkip
        {
            get { return (Page - 1) * PerPage; }
        }

        public int Page
        {
            get { return page; }
            set
            {
                if (value <= 0)
                    page = 1;
                else
                    page = value;
            }
        }

        public int PerPage { get; set; }

        public int NumberOfPages
        {
            get {
                try
                {
                    if (NumberOfRecords == 0) 
                        return 1;

                    return (int) Math.Ceiling((decimal) NumberOfRecords/PerPage);
                }
                catch (DivideByZeroException)
                {
                    return 0;
                }
            }
        }
    }
}