using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;


public class Appointment
{
    [JsonProperty("Day")]
    public string DayOfWeek { get; set; }

    [JsonProperty("Hour")]
    public TimeSpan StartTime { get; set; }

    [JsonProperty("Duration")]
    public int Duration { get; set; }
}

public class Program
{
    public static async Task Main()
    {
        string jsonFileUrl = "https://luegopago.blob.core.windows.net/luegopago-uploads/Pruebas%20LuegoPago/data.json";

        string jsonContent = await DownloadJsonFromUrl(jsonFileUrl);

        List<Appointment> appointments = JsonConvert.DeserializeObject<List<Appointment>>(jsonContent);

        Console.WriteLine("Por favor ingrese el día que desea agendar una cita (Ej. Lunes)");
        var dayToCheck = Console.ReadLine();

        string formatedDayToCheck = Regex.Replace(dayToCheck.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");

        int availableSpaces = CalculateAvailableSpaces(appointments, formatedDayToCheck);

        Console.WriteLine($"Citas disponibles para el dia {dayToCheck} son: {availableSpaces}");
    }

    public static async Task<string> DownloadJsonFromUrl(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception("Failed to download the JSON file from the provided URL.");
            }
        }
    }

    public static int CalculateAvailableSpaces(List<Appointment> appointments, string dayToCheck)
    {
        TimeSpan openingTime = TimeSpan.FromHours(9);
        TimeSpan closingTime = TimeSpan.FromHours(17);
        TimeSpan minAppointmentDuration = TimeSpan.FromMinutes(30);

        var appointmentsForDay = appointments.Where(appointment => appointment != null && Regex.Replace(appointment.DayOfWeek.ToLower().Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "") == dayToCheck.ToLower()).ToList();

        if (appointmentsForDay == null || appointmentsForDay.Count == 0)
        {
            return 0;
        }
        else
        {
            int totalAppointmentDuration = appointmentsForDay.Sum(appointment => appointment.Duration);

            TimeSpan availableTime = closingTime - openingTime;
            int remainingTime = (int)(availableTime.TotalMinutes - totalAppointmentDuration);

            int availableSpaces = remainingTime / (int)minAppointmentDuration.TotalMinutes;

            return availableSpaces;
        }
    }
}