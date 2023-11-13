using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace webapi.Models
{
    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Source { get; set; }
        public string? Value { get; set; }
        public required int RatingsRefId { get; set; }
        public Movie? Movie { get; set; }
    }
}
