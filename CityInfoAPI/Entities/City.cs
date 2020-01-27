using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class City
    {
        //!!AK1 mark entity primary key and identity for Id field.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] //!!AK1.3 data anotations
        [MaxLength(50)] //!!AK1.3 data anotations
        public string Name { get; set; }

        [MaxLength(200)]  //!!AK1.3 data anotations
        public string Description { get; set; }

        [MaxLength(200)]  
        public string Link { get; set; }

        //!!AK1.2 relation with child table
        public ICollection<PointOfInterest> PointsOfInterest { get; set; }
               = new List<PointOfInterest>();
    }
}
