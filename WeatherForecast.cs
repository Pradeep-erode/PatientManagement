using Serilog.Events;

namespace PatientManagement
{
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }

    public class LogEvent
    {
        public DateTime Timestamp { get; set; }

        public string Level { get; set; }

        public string RenderedMessage { get; set; }
        //public string Category { get; set; }
       // public string SubmissionId { get; set; }

    }
}
