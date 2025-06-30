using UnityEngine;

[System.Serializable]
public class WeatherApiResponse
{
    public Location location;
    public Current current;
    public Astronomy astronomy;
}
