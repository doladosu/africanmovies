using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace AfricanMovies.Tests.Integration.Extensions
{
    public static class FixtureExtensions
    {
        public static TResult Resolve<TResult>(this IFixture fixture)
            where TResult: class 
        {
            var context = new SpecimenContext(fixture);
            return context.Resolve(typeof(TResult)) as TResult;
        }
    }
}
