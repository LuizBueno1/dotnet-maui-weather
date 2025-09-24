using System;
using WeatherNow.Models;
using WeatherNow.Services;

namespace WeatherNow
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked_Forecast(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_city.Text))
                {
                    Weather? forecast = await DataService.GetForecast(txt_city.Text);

                    if (forecast != null)
                    {
                        string forecast_data = "";

                        forecast_data = $"Latitude: {forecast.lat} \n" +
                                        $"Longitude: {forecast.lon} \n" +
                                        $"Sunrise: {forecast.sunrise} \n" +
                                        $"Sunset: {forecast.sunset} \n" +
                                        $"Maximum Temperature: {forecast.temp_max} \n" +
                                        $"Minimum temperature: {forecast.temp_min} \n" +
                                        $"Description: {forecast.description} \n" +
                                        $"Wind Speed: {forecast.speed} \n" +
                                        $"Visibility: {forecast.visibility} \n";

                        lbl_res.Text = forecast_data;

                        string map = $"https://embed.windy.com/embed.html?" +
                                      $"type=map&location=coordinates&metricRain=mm&metricTemp=°C" +
                                      $"&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                                      $"&lat={forecast.lat.ToString().Replace(",", ".")}&lon={forecast.lon.ToString().Replace(",", ".")}";

                        wv_map.Source = map;

                    }
                    else
                    {
                        lbl_res.Text = "No forecast data.";
                    }
                }
                else
                {
                    lbl_res.Text = "Fill in the city!";
                }
            }
            catch (CityNotFoundException ex)
            {
                await DisplayAlert("City Error", ex.Message, "OK");
                lbl_res.Text = ex.Message;
            }
            catch (NoInternetConnectionException ex)
            {
                await DisplayAlert("Connection Error", ex.Message, "OK");
                lbl_res.Text = "No internet connection.";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
                lbl_res.Text = "Unexpected error.";
            }
        }

        private async void Button_Clicked_Location(object sender, EventArgs e)
        {
            try
            {
                GeolocationRequest geolocation = new GeolocationRequest(
                                                        GeolocationAccuracy.Medium,
                                                        TimeSpan.FromSeconds(10));

                Location? local = await Geolocation.Default.GetLocationAsync(geolocation);

                if (local != null)
                {
                    string local_device = $"Latitude: {local.Latitude} \n" +
                                          $"Longitude: {local.Longitude}";

                    lbl_coords.Text = local_device;

                    GetCity(local.Latitude, local.Longitude);

                }
                else
                {
                    lbl_coords.Text = "No location";
                }

            }
            catch(FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Error: Device not supported", fnsEx.Message, "OK");
            }
            catch(FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Error: Location disabled", fneEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Error: Location permission", pEx.Message, "OK");
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void GetCity(double lat, double lon)
        {
            try
            {
                IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);

                Placemark? place = places.FirstOrDefault();

                if (place != null)
                {
                    txt_city.Text = place.Locality;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error: Getting city name", ex.Message, "Ok");
            }
        }

    }

}
