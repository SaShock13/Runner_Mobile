using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherApiDialog : MonoBehaviour
{
    private const string apiKey = "9512422b16904b3db6442231250506 ";
    private const string city = "Togliatti";
    private const string cityRus = "Тольятти";

    [SerializeField] private TMP_Text cityView;
    [SerializeField] private TMP_Text tempView;
    [SerializeField] private TMP_Text humView;
    [SerializeField] private TMP_Text windView;
    [SerializeField] private TMP_Text sunriseView;
    [SerializeField] private TMP_Text sunsetView;
    private string date ;

    async void Start()
    {

        Debug.Log($"start {this}");
        date = DateTime.Today.ToString("yyyy-MM-dd");
        cityView.text = cityRus;
        tempView.text = "0";
        humView.text = "0";
        windView.text = "0";
        sunriseView.text = "0";
        sunsetView.text = "0";

        //StartCoroutine(GetWeatherData());
        await FetchFullWeatherData(city);

    }



    public async Task FetchFullWeatherData(string city)
    {
        string date = DateTime.Today.ToString("yyyy-MM-dd");

        string weatherUrl = $"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={city}";
        string astronomyUrl = $"https://api.weatherapi.com/v1/astronomy.json?key={apiKey}&q={city}&dt={date}";

        try
        {
            Task<string> weatherTask = GetJsonAsync(weatherUrl);
            Task<string> astroTask = GetJsonAsync(astronomyUrl);

            await Task.WhenAll(weatherTask, astroTask);

            var weatherData = JsonUtility.FromJson<WeatherApiResponse>(weatherTask.Result);
            var astronomyData = JsonUtility.FromJson<AstronomyResponse>(astroTask.Result);

            Debug.Log($"🌡 Температура: {weatherData.current.temp_c}°C");
            Debug.Log($"🌇 Закат: {astronomyData.astronomy.astro.sunset}");

            tempView.text = weatherData.current.temp_c.ToString();
            humView.text = weatherData.current.humidity.ToString();
            float wind_mps = weatherData.current.wind_kph * 1000 / 3600;
            windView.text = wind_mps.ToString("F1");
            sunriseView.text = astronomyData.astronomy.astro.sunrise.ToString();
            sunsetView.text = astronomyData.astronomy.astro.sunset.ToString();
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Ошибка: {e.Message}");
        }
    }

    private async Task<string> GetJsonAsync(string url)
    {
        using UnityWebRequest request = UnityWebRequest.Get(url);
        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result == UnityWebRequest.Result.Success)
            return request.downloadHandler.text;
        else
            throw new Exception(request.error);
    }


    IEnumerator GetWeatherData()
    {
        string urlCurrent = $"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={city}";
        string urlAstronomy = $"https://api.weatherapi.com/v1/astronomy.json?key={apiKey}&q={city}&dt={date}";



        using (UnityWebRequest request = UnityWebRequest.Get(urlCurrent))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                WeatherApiResponse weatherData = JsonUtility.FromJson<WeatherApiResponse>(json);
                Debug.Log($"Температура в {weatherData.location.name}: {weatherData.current.temp_c}°C");
                Debug.Log($" {weatherData.current.temp_c}: ");
                Debug.Log($"{weatherData.current.humidity}: ");
                Debug.Log($" {weatherData.current.wind_kph}: ");
                tempView.text = weatherData.current.temp_c.ToString();
                humView.text = weatherData.current.humidity.ToString();
                float wind_mps = weatherData.current.wind_kph * 1000 / 3600;
                windView.text = wind_mps.ToString("F1");
            }
            else
            {
                Debug.LogError($"Ошибка при получении данных: {request.error}");
            }
        }

        using (UnityWebRequest request = UnityWebRequest.Get(urlAstronomy))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                var weatherData = JsonUtility.FromJson<AstronomyResponse>(json);
                Debug.Log($"Восход в : {weatherData.astronomy.astro.sunrise}");
                Debug.Log($"Закат в {weatherData.astronomy.astro.sunset}: ");
                sunriseView.text = weatherData.astronomy.astro.sunrise.ToString();
                sunsetView.text = weatherData.astronomy.astro.sunset.ToString();
                //cityView.text = city;
                //tempView.text = weatherData.current.temp_c.ToString();
                //humView.text = weatherData.current.humidity.ToString();
                //float wind_mps = weatherData.current.wind_kph * 1000 / 3600;
                //windView.text = wind_mps.ToString("F1");
            }
            else
            {
                Debug.LogError($"Ошибка при получении данных: {request.error}");
            }
        }




    }
}
