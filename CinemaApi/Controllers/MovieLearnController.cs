using CinemaApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieLearnController : ControllerBase
    {
        private static List<Movie> movies = new List<Movie>()
        {
           new Movie(){Id=1, Name="Something 1", Language="En" } ,
           new Movie(){Id=2, Name="Something 2", Language="En" } ,
           new Movie(){Id=3, Name="Something 3", Language="En" } ,
        };

        [HttpGet]
        public List<Movie> Get() 
        {
            return movies;
        }

        [HttpPost]
        public void Post([FromBody] Movie movie) 
        {
            movies.Add(movie);  
        }

        [HttpPut("{id}")]
        public void Put(int id,[FromBody]Movie movie)
        {
            movies[id] = movie;    
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            movies.RemoveAt(id);
        }
    }
}
