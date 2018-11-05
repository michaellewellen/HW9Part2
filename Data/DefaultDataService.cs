using Avalonia.Controls;
using GeneGenie.Gedcom.Parser;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Data
{
    public class DefaultDataService : IDataService
    {
        public Task<IEnumerable<Person>> GetPeopleFromGedcomAsync(string gedcomFile)
        {
            return Task.Run(() =>
            {
                var reader = GedcomRecordReader.CreateReader(gedcomFile);
                var db = reader.Database;

                return from p in db.Individuals
                       where p.Names.Any()
                       let name = p.Names.First()
                       select new Person(
                           name.Surname,
                           name.Given,
                           p.Birth?.Date?.DateTime1,
                           p.Birth?.Address?.ToString(),
                           p.Death?.Date?.DateTime1,
                           p.Death?.Address?.ToString());
            });
        }

        public bool FileExists(string gedcomFile)
        {
            return File.Exists(gedcomFile);
        }

        public async Task<string> FindFileAsync()
        {
            var openFileDialog = new OpenFileDialog()
            {
                AllowMultiple = false,
                Title = "What Gedcom file do you want to use?"
            };
            var pathArray = await openFileDialog.ShowAsync();

            if ((pathArray?.Length ?? 0) > 0)
                return pathArray[0];
            return null;
        }


        private const string GeolookupAndCurrentConditionsUri = "http://api.worldweatheronline.com/free/v2/weather.ashx?key={0}&q={1},{2}&format=json&num_of_days=1";
        private const string GeolookupCurrentConditionsAndForecastUri = "http://api.wunderground.com/api/{0}/geolookup/conditions/forecast/q/{1},{2}.json";
        private const string GeolookupHourlyForecastUri = "http://api.wunderground.com/api/27d9503963b27155/geolookup/hourly/q/{1},{2}.json";

        public static async Task<WeatherResponse> GetConditionsForLocationAsync(double lat, double lng)
        {
            string uri = string.Format(GeolookupAndCurrentConditionsUri, Config.ApiKey, lat, lng);

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(content);

                    if (weatherResponse.results != null && weatherResponse.results.error != null)
                        throw new WeatherServiceException(weatherResponse.results.error.message);

                    return weatherResponse;
                }
            }
            return null;
        }
        /// <summary>
        /// The configuration for the WorldWeatherOnline Client
        /// </summary>
        public static class Config
        {
            /// <summary>
            /// The API Key for the WorldWeatherOnline API. Get yours at http://www.worldweatheronline.com
            /// </summary>
            public static string ApiKey { get; set; }
        }
    }

    /// <summary>
    /// An exception thrown by the weather service
    /// </summary>
    public class WeatherServiceException : Exception
    {
        /// <summary>
        /// Creates a new exception with the specified message
        /// </summary>
        /// <param name="message">The message</param>
        public WeatherServiceException(string message) : base(message) { }
    }


    
}
