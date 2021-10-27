using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CinemaApi.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Name can't be null or empty")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description can't be null or empty")]
        public string Description { get; set; }


        [Required(ErrorMessage = "language can't be null or empty")]
        public string Language { get; set; }

        [Required(ErrorMessage = "Duration can't be null or empty")]
        public double Duration { get; set; }

        [Required(ErrorMessage = "PlayingDate can't be null or empty")]
        public DateTime PlayingDate { get; set; }

        [Required(ErrorMessage = "PlayingTime can't be null or empty")]
        public DateTime PlayingTime { get; set; }

        [Required(ErrorMessage = "TicketPrice can't be null or empty")]
        public double TicketPrice { get; set; }

        [Required(ErrorMessage = "Rating can't be null or empty")]
        public double Rating { get; set; }

        public string Genre { get; set; }
        public string TrailorUrl { get; set; }
        public string ImageUrl { get; set; }


        [NotMapped]
        public IFormFile Image { get; set; }

    }
}
