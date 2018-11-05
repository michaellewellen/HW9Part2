using Haukcode.DotNetTracking;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace Data
{
    public class DefaultTrackingService : ITrackingService
    {
        public IEnumerable<TrackingResult> Track(string trackingNum)
        {
            return from t in new TrackingNumber(trackingNum)?.TrackingTypes
                   select new TrackingResult
                   {
                       Carrier = t.Carrier,
                       TrackingUrl = t.TrackingURL
                   };
        }
    }
}
