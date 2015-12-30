using System.Collections.Generic;
using System.Threading.Tasks;
using AfricanMovies.Backend.DataSources.DTO;

namespace AfricanMovies.Backend.DataSources
{
    public interface IVideosDataSource
    {
        Task<List<ChannelDTO>> FindAll();
    }
}