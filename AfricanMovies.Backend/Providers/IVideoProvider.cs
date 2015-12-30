using System.Collections.Generic;
using System.Threading.Tasks;
using AfricanMovies.Backend.DataSources.DTO;

namespace AfricanMovies.Backend.Providers
{
    public interface IVideoProvider
    {
        Task<List<ChannelDTO>> ChannelsData();
    }
}