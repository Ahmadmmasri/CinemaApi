using CinemaApi.Data;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CinemaApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private CinemaDbContext _dbContext;

        public MoviesController(CinemaDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        //normal statement from database

        // GET: api/<MoviesController>
        [HttpGet]
        [Authorize]
        public IActionResult AllMovies(string sort,int? pageNumber, int? pageSize)
        {
            var CurrentPageNumber = pageNumber ?? 1;
            var CurrentPageSize = pageSize ?? 5;
            switch (sort)
            {
                case "desc":
                    return Ok(_dbContext.Movies.Skip((CurrentPageNumber - 1) * CurrentPageSize).Take(CurrentPageSize).OrderByDescending(x=>x.Rating).ToList());
                case "asc":
                    return Ok(_dbContext.Movies.Skip((CurrentPageNumber - 1) * CurrentPageSize).Take(CurrentPageSize).OrderBy(x => x.Rating).ToList());
                default:
                    return Ok(_dbContext.Movies.Skip((CurrentPageNumber - 1)* CurrentPageSize).Take(CurrentPageSize).ToList());
            }
        }

        [HttpGet("[action]")]
        //api/Movies/FindMovies?movieName=something
        [Authorize]
        public IActionResult FindMovies(string movieName)
        {
            return Ok(_dbContext.Movies.Where(x => x.Name.StartsWith(movieName)).ToArray());
        }


        // GET api/<MoviesController>/5
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id)
        {
            var movie = _dbContext.Movies.Find(id);
            if (movie == null)
                return NotFound("this record not found for this id");
            else
                return Ok(movie);

        }

        // POST api/<MoviesController>
        //Without file uploading (Normal)
        /* 
         [HttpPost]
         public IActionResult Post([FromBody] Movie movie)
         {
             _dbContext.Movies.Add(movie);
             _dbContext.SaveChanges();
             return StatusCode(StatusCodes.Status201Created);
         }
        */


        //With file Uploading
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public IActionResult Post([FromForm] Movie movie)
        {
            var randomIdImage = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", randomIdImage + ".jpg");
            if (movie.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movie.Image.CopyTo(fileStream);
            }
            //Remove wwwroot before save in db
            movie.ImageUrl = filePath.Remove(0, 7);
            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);

        }

        // PUT api/<MoviesController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Put(int id, [FromForm] Movie movie)
        {
            var Updatemovie = _dbContext.Movies.Find(id);
            if (Updatemovie == null)
            {
                return NotFound("this record not found for this id");
            }
            else
            {
                var randomIdImage = Guid.NewGuid();
                var filePath = Path.Combine("wwwroot", randomIdImage + ".jpg");
                if (movie.Image != null)
                {
                    var fileStream = new FileStream(filePath, FileMode.Create);
                    movie.Image.CopyTo(fileStream);
                    //Remove wwwroot before save in db
                    Updatemovie.ImageUrl = filePath.Remove(0, 7);
                }
                Updatemovie.Name = movie.Name;
                Updatemovie.Description = movie.Description;
                Updatemovie.Language = movie.Language;
                Updatemovie.Duration = movie.Duration;
                Updatemovie.PlayingDate = movie.PlayingDate;
                Updatemovie.PlayingTime = movie.PlayingTime;
                Updatemovie.TicketPrice = movie.TicketPrice;
                Updatemovie.Rating = movie.Rating;
                Updatemovie.TrailorUrl = movie.TrailorUrl;
                _dbContext.SaveChanges();
                return Ok("Record Updated Successfully");
            }
        }

        // DELETE api/<MoviesController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var movie = _dbContext.Movies.Find(id);
            if (movie == null)
            {
                return NotFound("this record not found for this id");
            }
            _dbContext.Movies.Remove(movie);
            _dbContext.SaveChanges();
            return Ok("Record Deleted Successfully");
        }

        //api/Movies//Test/5
        [HttpGet("[action]/{id}")]
        public int Test(int id)
        {
            return id;
        }


        //using httpState code

        //public IActionResult Get() 
        //{
        //    //  return Ok(_dbContext.Movies);
        //    return StatusCode(StatusCodes.Status200OK);
        //}
    }
}
