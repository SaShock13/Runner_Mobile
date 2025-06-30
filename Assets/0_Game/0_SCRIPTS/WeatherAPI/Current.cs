[System.Serializable]
public class Current
{
    public long last_updated_epoch;
    public string last_updated;
    public float temp_c;
    public float temp_f;
    public int is_day;
    public Condition condition;
    public float wind_mph;
    public float wind_kph;
    public int wind_degree;
    public string wind_dir;
    public float pressure_mb;
    public float pressure_in;
    public float precip_mm;
    public float precip_in;
    public int humidity;
    public int cloud;
    public float feelslike_c;
    public float feelslike_f;
    public float vis_km;
    public float vis_miles;
    public float uv;
    public float gust_mph;
    public float gust_kph;
}