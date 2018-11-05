using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IDataService
    {
        Task<IEnumerable<Person>> GetPeopleFromGedcomAsync(string gedcomFile);
        bool FileExists(string gedcomFile);
        Task<string> FindFileAsync();
        Task<WeatherResponse> GetConditionsForLocationAsync(double lat, double lng);
    }
}
