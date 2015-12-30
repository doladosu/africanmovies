namespace AfricanMovies.Backend.Queries
{
    public interface ISortable
    {
        OrderSettings OrderBy { get; set; }
    }
}