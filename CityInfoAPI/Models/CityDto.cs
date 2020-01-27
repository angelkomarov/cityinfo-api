using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Models
{
    public class CityDto : CityWithoutPointsOfInterestDto
    {
        public int NumberOfPointsOfInterest { get
            {
                return PointsOfInterest.Count;
            }
        }

        public ICollection<PointOfInterestDto> PointsOfInterest { get; set; }
        = new List<PointOfInterestDto>();
    }
}
