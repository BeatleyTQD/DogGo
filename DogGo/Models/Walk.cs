using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Models
{
    public class Walk
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public int WalkerId { get; set; }
        public int DogId { get; set; }

        public Dog Dog { get; set; }
        public Owner Owner { get; set; }

        /*HOW I WOULD DO IT IF I WERE SMART
        public int getDurationMin()
        {
            int min = 0;
            if (Duration != 0)
            {
                min = Duration / 60;
            }
            return min;

        }

        public int DurationMin { get { return getDurationMin(); } }
        */
    }
}
