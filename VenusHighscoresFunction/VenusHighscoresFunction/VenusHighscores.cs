using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace HighscoreFunction
{
    public class VenusHighscores
    {
        private readonly ILogger<VenusHighscores> _logger;

        public VenusHighscores(ILogger<VenusHighscores> logger)
        {
            _logger = logger;
        }

        [Function("VenusHighscores")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req)
        {
            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");

            // GET: Highscores abrufen
            if (req.Method == "GET")
            {
                _logger.LogInformation("Processing GET request to retrieve highscores.");

                List<object> highscores = new List<object>();

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        string query = "SELECT name, score, timestamp FROM scores ORDER BY score DESC, timestamp DESC";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            using (SqlDataReader reader = await command.ExecuteReaderAsync())
                            {
                                int count = 0;
                                while (await reader.ReadAsync() && count < 7) // limit to 7 entries for display
                                {
                                    highscores.Add(new
                                    {
                                        name = reader["name"].ToString(),
                                        score = (int)reader["score"],
                                        timestamp = reader["timestamp"].ToString()
                                    });
                                    count++;
                                }
                            }
                        }
                    }

                    var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                    response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    await response.WriteStringAsync(JsonSerializer.Serialize(highscores));
                    return response;
                }
                catch (System.Exception ex)
                {
                    _logger.LogError($"Fehler beim Abrufen der Highscores: {ex.Message}");
                    var response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    await response.WriteStringAsync(JsonSerializer.Serialize(new { error = ex.Message }));
                    return response;
                }
            }
            // POST: Highscore speichern
            else if (req.Method == "POST")
            {
                _logger.LogInformation("Processing POST request to save a highscore.");

                // Request-Body lesen
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var jsonDoc = JsonDocument.Parse(requestBody);
                var root = jsonDoc.RootElement;
                string name = root.GetProperty("name").GetString();
                int score = root.GetProperty("score").GetInt32();

                if (string.IsNullOrEmpty(name) || score < 0)
                {
                    var response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    await response.WriteStringAsync(JsonSerializer.Serialize(new { error = "Ungültige Daten: Name darf nicht leer sein und Score muss >= 0 sein." }));
                    return response;
                }

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        string query = "INSERT INTO scores (name, score, timestamp) VALUES (@name, @score, GETDATE())";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@score", score);
                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                    response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    await response.WriteStringAsync(JsonSerializer.Serialize(new { message = "Highscore erfolgreich gespeichert" }));
                    return response;
                }
                catch (System.Exception ex)
                {
                    _logger.LogError($"Fehler beim Speichern des Highscores: {ex.Message}");
                    var response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    await response.WriteStringAsync(JsonSerializer.Serialize(new { error = ex.Message }));
                    return response;
                }
            }

            var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            badRequestResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await badRequestResponse.WriteStringAsync(JsonSerializer.Serialize(new { error = "Ungültige HTTP-Methode. Verwende GET zum Abrufen oder POST zum Speichern." }));
            return badRequestResponse;
        }
    }
}