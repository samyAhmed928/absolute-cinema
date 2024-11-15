using AutoMapper;
using MoviesApi.Models;

namespace MoviesApi.Helpers
{
	public class MappingProfile:Profile
	{
        public MappingProfile()
        {
            CreateMap<Movie, MovieDetailsDto>();
            CreateMap<CreateMovieDto, Movie>()
                .ForMember(src => src.Poster, opt => opt.Ignore());
        }
    }
}
