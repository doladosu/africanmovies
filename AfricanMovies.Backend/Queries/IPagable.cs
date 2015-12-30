namespace AfricanMovies.Backend.Queries
{
    public interface IPagable
    {
        PaginationSettings Pagination { get; set; }
    }
}