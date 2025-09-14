using Newtonsoft.Json.Linq;
using WeatherNow.Models;

namespace WeatherNow.Services
{
    public class DataService
    {
        public static async Task<Weather?> getForecast(string city)
        {
            Weather? weather = null;

            string apiKey = "221c20a1cd3d395cfe5792f7a79f418f";

            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                            $"q={city}&units=metric&appid={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage message = await client.GetAsync(url);

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
                        temp_max = (double)sketch["main"]["temp_min"],
                        speed = (double)sketch["wind"]["temp_max"],
                        visibility = (int)sketch["visibility"],
                        sunrise = sunrise.ToString(),
                        sunset = sunset.ToString()

                    };
                }
            }

            return weather;
        }
    }
}
