using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public interface ITrackingService
    {
        IEnumerable<TrackingResult> Track(string trackingNum);
    }

    public class TrackingResult
    {
        public string Carrier { get; set; }
        public string TrackingUrl { get; set; }
    }
}
