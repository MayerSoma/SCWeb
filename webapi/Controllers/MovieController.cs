using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using webapi.Database;
using webapi.Services.OMDB;
using webapi.Services.OMDB.Models;
using webapi.Services.OMDB.Interfaces;
using webapi.BL;
using Microsoft.EntityFrameworkCore;
using webapi.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.ComponentModel;
using System.Linq.Dynamic.Core;
using static webapi.BL.MovieBusinessLogic;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System.Web;

namespace webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly ILogger<MovieController> _logger;
        private readonly DataContext _ctx;
        private readonly IAsyncOmdbClient _asyncOmdbClient;
        private readonly MovieBusinessLogic _movieBusinessLogic;

        public MovieController(ILogger<MovieController> logger, DataContext dataContext, IAsyncOmdbClient asyncOmdbClient)
        {
            _asyncOmdbClient = asyncOmdbClient;
            _ctx = dataContext;
            _logger = logger;
            _movieBusinessLogic = new MovieBusinessLogic();
        }

        [HttpPost(Name = "PostMovies")]
        public async Task<ActionResult<IEnumerable<Movie>>> PostMovies(string title)
        {
            try
            {
                if (string.IsNullOrEmpty(title))
                {
                    return StatusCode(400, "Title is empty!");
                }
                var item = await _asyncOmdbClient.GetItemByTitleAsync(title);

                if (item == null) 
                {
                    return StatusCode(500, $"Could not find a film with a title: {title}" );
                }
                Movie movie = _movieBusinessLogic.MapMovies(item);
                
                _ctx.Set<Movie>().AddIfNotExists(movie, x => x.Title == title);
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                // LOG ex
                // TODO Handle or throw
                return StatusCode(500, "Something went wrong! :-) ");
            }
            
            return Ok();
        }

        // Using Post because:
        // 1. Complex object fetching from URI is a nightmare also OpenAPI still does not support it
        // 2. No column name expose in the URI
        // Shall we violate CRUD?
        // Another option -> Base64 encode in uri then fetch
        [Route("GetMovies")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies(string? columnName = null, string? orderType = null,
            [FromBody] IList<FilterEntity>? filterEntities = null)
        {
            var baseQuery = _ctx.Set<Movie>().Include(r => r.Ratings).AsNoTracking().AsQueryable();

            if (baseQuery != null && !string.IsNullOrEmpty(columnName) && !string.IsNullOrEmpty(orderType))
            {
                baseQuery = _movieBusinessLogic.ApplyDynamicOrderByAsQueryable(baseQuery, columnName, orderType);
            }
            if (baseQuery != null && filterEntities != null && filterEntities.Any()) 
            {
                baseQuery = _movieBusinessLogic.ApplyFiltersAsQueryable(baseQuery, filterEntities);
            }

            if (baseQuery == null)
            {
                return StatusCode(500, $"Could not find a film");
            }

            var result = await baseQuery.ToListAsync();

            return Ok(result);
        }
    }
}
