using Microsoft.Build.Framework;

namespace MovieStoreMvc.Models.Domain
{
    public class Genre
    {
        public int Id { get; set; }
        [Required]
        public string? GenreName { get; set; }
    }
}
