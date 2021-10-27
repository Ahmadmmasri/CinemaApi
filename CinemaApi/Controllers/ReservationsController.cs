using CinemaApi.Data;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;
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
    public class ReservationsController : ControllerBase
    {
        private CinemaDbContext _dbContext;

        public ReservationsController(CinemaDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody]Reservation reservation)
        {
            reservation.ReservationTime = DateTime.Now;
            _dbContext.Reservations.Add(reservation);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public IActionResult GetReservations()
        {
            var reservationDetails = from reservation in _dbContext.Reservations
                                     join User in _dbContext.Users on reservation.UserId equals User.Id
                                     join Movie in _dbContext.Movies on reservation.MovieId equals Movie.Id
                                     select new
                                     {
                                         Id = reservation.Id,
                                         ReservationTime = reservation.ReservationTime,
                                         UserName = User.Id,
                                         MovieName = Movie.Name
                                     };
            return Ok(reservationDetails);



        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetReservationsDetails(int id)
        {

            var reservationDetails = (from reservation in _dbContext.Reservations
                                     join User in _dbContext.Users on reservation.UserId equals User.Id
                                     join Movie in _dbContext.Movies on reservation.MovieId equals Movie.Id
                                     where (reservation.Id == id)
                                     select new
                                     {
                                         Id = reservation.Id,
                                         ReservationTime = reservation.ReservationTime,
                                         UserName = User.Id,
                                         MovieName = Movie.Name,
                                         Email=reservation.Qty,
                                         price= reservation.Price,
                                         phone=reservation.Phone,
                                         playingDate= Movie.PlayingDate,
                                         playingTime= Movie.PlayingTime,

                                     }).FirstOrDefault();
            return Ok(reservationDetails);

        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult ReservationsDelete(int id)
        {
          var deleteReservation= _dbContext.Reservations.Find(id);
            if (deleteReservation == null)
            {
                return NotFound("No record founf for this id");
            }
            else
            {
                _dbContext.Remove(deleteReservation);
                _dbContext.SaveChanges();
                return Ok("Record Deleted");
            }
        }





    }
}
