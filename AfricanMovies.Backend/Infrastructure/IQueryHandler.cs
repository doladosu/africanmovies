namespace AfricanMovies.Backend.Infrastructure
{
    public interface IQueryHandler<out TResult, in TQuery> where TQuery : IQuery<TResult>
    {
        TResult Handle(TQuery query);
    }
}