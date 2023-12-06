using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;


public class Appointment
{
  [JsonProperty("Day")] public string DayOfWeek { get; set; }

  [JsonProperty("Hour")] public TimeSpan StartTime { get; set; }

  [JsonProperty("Duration")] public int Duration { get; set; }
}

public class Program
{
  public static async Task Main()
  {
    string jsonFileUrl = "https://luegopago.blob.core.windows.net/luegopago-uploads/Pruebas%20LuegoPago/data.json";
    string jsonContent = await DownloadJsonFromUrl(jsonFileUrl);

    List<Appointment> appointments = JsonConvert.DeserializeObject<List<Appointment>>(jsonContent);

    PrintAllAvailableSpaces(appointments);
  }

  private static void PrintAllAvailableSpaces(List<Appointment> appointments)
  {
    bool switchControl = true;
    while (switchControl)
    {
      Console.WriteLine("Por favor ingrese el día que desea agendar una cita (Ej. Lunes)");
      string? dayToCheck = NormalizeDayOfTheWeek(Console.ReadLine());

      int? availableSpaces = CalculateAvailableSpaces(appointments, dayToCheck);

      if (availableSpaces != null)
      {
        Console.WriteLine($"Citas disponibles para el día {dayToCheck} son: {availableSpaces}");

        Console.WriteLine("Desea consultar de nuevo? (Si/No)");
        string? response = Console.ReadLine().ToLower();

        if (response == "no")
        {
          switchControl = false;
        }
      }
      else
      {
        Console.WriteLine("Por favor ingrese un día válido");
      }
    }
  }

  private static async Task<string> DownloadJsonFromUrl(string url)
  {
    using HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    if (response.IsSuccessStatusCode)
    {
      return await response.Content.ReadAsStringAsync();
    }
    else
    {
      throw new Exception("Failed to download the JSON file from the provided URL.");
    }
  }

  private static string NormalizeDayOfTheWeek(string dayOfWeek)
  {
    return Regex.Replace(dayOfWeek.ToLower().Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");
  }

  private static int? CalculateAvailableSpaces(List<Appointment> appointments, string dayToCheck)
  {
    TimeSpan openingTime = TimeSpan.FromHours(9);
    TimeSpan closingTime = TimeSpan.FromHours(17);
    TimeSpan minAppointmentDuration = TimeSpan.FromMinutes(30);

    var appointmentsForDay = appointments
      .Where(appointment =>
        NormalizeDayOfTheWeek(appointment.DayOfWeek) == dayToCheck.ToLower())
      .OrderBy(appointment => appointment.StartTime)
      .ToList();

    int availableSpaces = 0;

    TimeSpan currentTime = openingTime;

    if (appointmentsForDay.Count > 0)
    {
      TimeSpan timeUntilFirstAppointment = appointmentsForDay[0].StartTime - currentTime;
      availableSpaces += (int)(timeUntilFirstAppointment.TotalMinutes / minAppointmentDuration.TotalMinutes);
    }
    else
    {
      availableSpaces += (int)((closingTime - currentTime).TotalMinutes / minAppointmentDuration.TotalMinutes);
    }

    for (int i = 0; i < appointmentsForDay.Count - 1; i++)
    {
      TimeSpan timeBetweenAppointments = appointmentsForDay[i + 1].StartTime -
                                         (appointmentsForDay[i].StartTime +
                                          TimeSpan.FromMinutes(appointmentsForDay[i].Duration));
      int spacesBetween = (int)(timeBetweenAppointments.TotalMinutes / minAppointmentDuration.TotalMinutes);
      availableSpaces +=
        Math.Max(0, spacesBetween - 1);
    }

    if (appointmentsForDay.Count > 0)
    {
      TimeSpan timeAfterLastAppointment = closingTime -
                                          (appointmentsForDay.Last().StartTime +
                                           TimeSpan.FromMinutes(appointmentsForDay.Last().Duration));
      availableSpaces += (int)(timeAfterLastAppointment.TotalMinutes / minAppointmentDuration.TotalMinutes);
    }

    return availableSpaces;
  }
}