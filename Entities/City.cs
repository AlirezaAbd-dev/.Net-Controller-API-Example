using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cityinfo.API.Entities
{
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] [MaxLength(50)] public string Name { get; set; }

        [MaxLength(200)] public string? Description { get; set; }

        public City(string name)
        {
            Name = name;
        }

        public ICollection<PointOfInterest> PointOfInterest { get; set; } = [];

        [ForeignKey("UserId")]
        public User User { get; set; }

        public int UserId { get; set; }
    }
}
