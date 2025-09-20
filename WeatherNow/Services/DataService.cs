using System.Net;
using Newtonsoft.Json.Linq;
using WeatherNow.Models;

namespace WeatherNow.Services
{
    public class DataService
    {
        public static async Task<Weather?> GetForecast(string city)
        {
            Weather? weather = null;

            string apiKey = "221c20a1cd3d395cfe5792f7a79f418f";

            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                            $"q={city}&units=metric&appid={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage message;

                try
                {
                    message = await client.GetAsync(url);
                }
                catch (HttpRequestException ex)
                {
                    throw new NoInternetConnectionException("No internet connection.", ex);
                }

                if (message.IsSuccessStatusCode)
                {
                    string json = await message.Content.ReadAsStringAsync();

                    var sketch = JObject.Parse(json);

                    DateTime time = new();
                    DateTime sunrise = time.AddSeconds((double)sketch["sys"]["sunrise"]).ToLocalTime();
                    DateTime sunset = time.AddSeconds((double)sketch["sys"]["sunset"]).ToLocalTime();

                    weather = new()
                    {
                        lat = (double)sketch["coord"]["lat"],
                        lon = (double)sketch["coord"]["lon"],
                        description = (string)sketch["weather"][0]["description"],
                        main = (string)sketch["weather"][0]["main"],
                        temp_min = (double)sketch["main"]["temp_min"],
                        temp_max = (double)sketch["main"]["temp_max"],
                        speed = (double)sketch["wind"]["speed"],
                        visibility = (int)sketch["visibility"],
                        sunrise = sunrise.ToString(),
                        sunset = sunset.ToString()

                    };
                }
                else if(message.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new CityNotFoundException("City not found.");
                }
                else
                {
                    throw new HttpRequestException($"Unexpected error: {message.StatusCode}");
                }
            }

            return weather;
        }
    }

    public class CityNotFoundException : Exception
    {
        public CityNotFoundException(string message) : base(message) { }
    }

    public class NoInternetConnectionException : Exception
    {
        public NoInternetConnectionException(string message, Exception inner) : base(message, inner) { }
    }

}
