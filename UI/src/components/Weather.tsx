import { useEffect, useState } from "react";

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

const Weather = () => {
  const [data, setData] = useState<WeatherForecast[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch("https://localhost:7125/WeatherForecast", {
      headers: {
        accept: "text/plain",
      },
    })
      .then((res) => {
        if (!res.ok) {
          throw new Error("Failed to fetch weather data");
        }
        return res.json();
      })
      .then((result) => {
        setData(result);
        setLoading(false);
      })
      .catch((err) => {
        setError(err.message);
        setLoading(false);
      });
  }, []);

  if (loading) return <p>Loading...</p>;
  if (error) return <p style={{ color: "red" }}>{error}</p>;

  return (
    <div>
      <h2>Weather Forecast</h2>
      <ul>
        {data.map((item, index) => (
          <li key={index}>
            <strong>{item.date}</strong> — {item.temperatureC}°C (
            {item.temperatureF}°F) — {item.summary}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default Weather;
