using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    //!!AK2.1 add Context to interact with the database. 
    //It represents a session with the database, and it can be used to query and save instances of our entities.
    public class CityInfoContext : DbContext
    {
        //!!AK3.2 providing connection string - via options
        public CityInfoContext(DbContextOptions<CityInfoContext> options)
           : base(options)
        {
            //!!AK3.4 Creating DB from code
            //Database.EnsureCreated();
            //!!AK3.4.1 - execute migrations - if DB does not exist then create it
            //Database.Migrate();
        }

        //!!AK2.2. define DBSets of our entities – to query and save instances of its entity type
        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointsOfInterest { get; set; }

        //!!AK3.1 providing connection string by overriding OnConfiguring()- one way
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("connectionstring");

        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}
