using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    //!!AK4 dbcontext extension class - do initial seeding to the database.
    public static class CityInfoExtensions
    {
        //!!AK4.1 Implement EnsureSeedDataForContext method, 
        //get param the context
        //this tells the compiler that it extends  CityInfoContext
        public static void EnsureSeedDataForContext(this CityInfoContext context)
        {
            //check if there are allredy data in DB.
            if (context.Cities.Any())
            {
                return;
            }

            // init seed data
            var cities = new List<City>()
            {
                new City()
                {
                     Name = "New York City",
                     Description = "The one with that big park.",
                     Link = "www.nycgo.com/",
                     PointsOfInterest = new List<PointOfInterest>()
                     {
                         new PointOfInterest() {
                             Name = "Central Park",
                             Description = "The most visited urban park in the United States."
                         },
                          new PointOfInterest() {
                             Name = "Empire State Building",
                             Description = "A 102-story skyscraper located in Midtown Manhattan."
                          },
                     }
                },
                new City()
                {
                    Name = "Antwerp",
                    Description = "The one with the cathedral that was never really finished.",
                    Link = "www.visitantwerpen.be/en/home",
                    PointsOfInterest = new List<PointOfInterest>()
                     {
                         new PointOfInterest() {
                             Name = "Cathedral",
                             Description = "A Gothic style cathedral, conceived by architects Jan and Pieter Appelmans."
                         },
                          new PointOfInterest() {
                             Name = "Antwerp Central Station",
                             Description = "The the finest example of railway architecture in Belgium."
                          },
                     }
                },
                new City()
                {
                    Name = "Paris",
                    Description = "The one with that big tower.",
                    Link = "en.parisinfo.com/",
                    PointsOfInterest = new List<PointOfInterest>()
                     {
                         new PointOfInterest() {
                             Name = "Eiffel Tower",
                             Description =  "A wrought iron lattice tower on the Champ de Mars, named after engineer Gustave Eiffel."
                         },
                          new PointOfInterest() {
                             Name = "The Louvre",
                             Description = "The world's largest museum."
                          },
                     }
                },
                new City()
                {
                    Name = "Sydney",
                    Description = "The one with Opera House",
                    Link = "www.sydney.com",
                    PointsOfInterest = new List<PointOfInterest>()
                     {
                         new PointOfInterest() {
                             Name = "Opera House",
                             Description =  "The Sydney Opera House is a multi-venue performing arts centre at Sydney Harbour."
                         },
                          new PointOfInterest() {
                             Name = "Harbour Bridge",
                             Description = "It is a heritage-listed steel through arch bridge across Sydney Harbour that carries rail, vehicular, bicycle, and pedestrian traffic between the Sydney central business district and the North Shore"
                          },
                          new PointOfInterest() {
                             Name = "Darling Harbour",
                             Description = "This waterside pocket of Sydney has got it all; amazing entertainment, fascinating museums, incredible wildlife and delicious dining options right on the water plus a brand-new food precinct."
                          },
                     }
                },
                new City()
                {
                    Name = "Sofia",
                    Description = "The 15th largest city in the European Union.",
                    Link = "www.visitsofia.bg/en/",
                    PointsOfInterest = new List<PointOfInterest>()
                     {
                         new PointOfInterest() {
                             Name = "National Palace of Culture",
                             Description =  "The largest, multifunctional conference and exhibition centre in south-eastern Europe."
                         },
                          new PointOfInterest() {
                             Name = "Alexander Nevsky Cathedral",
                             Description = "Built in Neo-Byzantine style, it serves as the cathedral church of the Patriarch of Bulgaria and it is believed to be one of the top 50 largest Christian church buildings in the world."
                          },
                     }
                }
            };

            //add seeded data to DB.
            context.Cities.AddRange(cities);
            context.SaveChanges();
        }
    }
}
