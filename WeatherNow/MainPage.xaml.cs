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

        private async void Button_Clicked(object sender, EventArgs e)
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
                                        $"Minimum temperature: {forecast.temp_min} \n";

                        lbl_res.Text = forecast_data;
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
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
    }

}
