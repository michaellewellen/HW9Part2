using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public class Person
    {
        public Person(string surname, string givenName, DateTime? bornOn = null, string bornAt = null, DateTime? diedOn = null, string diedAt = null)
        {
            Surname = surname;
            GivenName = givenName;
            BornOn = bornOn;
            BornAt = bornAt;
            DiedOn = diedOn;
            DiedAt = diedAt;
        }

        public string Surname { get; }
        public string GivenName { get; }
        public DateTime? BornOn { get; }
        public string BornAt { get; }
        public DateTime? DiedOn { get; }
        public string DiedAt { get; }
    }
}
