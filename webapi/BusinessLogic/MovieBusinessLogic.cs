using AutoMapper;
using webapi.Models;
using webapi.Services.OMDB.Models;
using System.Linq.Dynamic.Core;

namespace webapi.BL
{
    public class MovieBusinessLogic
    {
        public Movie MapMovies(Item movieItem)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Services.OMDB.Models.Rating, Models.Rating>();
                cfg.CreateMap<Item, Movie>();
            });
            var mapper = new Mapper(config);
            Movie movie = mapper.Map<Movie>(movieItem);
            
            return movie;
        }

        public  IQueryable<Movie>? ApplyDynamicOrderByAsQueryable(IQueryable<Movie> query, string columnName, string orderType)
        {
            // TODO follow 39 line, SQL injection recheck
            string dynamicOrderBy = columnName + " " + orderType;

            return query.OrderBy(dynamicOrderBy);
        }

        public IQueryable<Movie>? ApplyFiltersAsQueryable(IQueryable<Movie> query, IEnumerable<FilterEntity> filterEntities)
        {
            foreach (FilterEntity filter in filterEntities) 
            {
                query = query.Where($"{filter.Column}.Contains(@0)", filter.Value);
            }
            
            return query;
        }
        public class FilterEntity
        {
            public string? Column { get; set; }
            public string? Value { get; set; }
        }
    }
}
