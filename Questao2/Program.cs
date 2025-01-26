using Newtonsoft.Json;
using Questao2;

public class Program
{
    public static async Task Main(string[] args)
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = await GetTotalScoredGoals(teamName, year);

        Console.WriteLine($"Team {teamName} scored {totalGoals} goals in {year}");

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await GetTotalScoredGoals(teamName, year);

        Console.WriteLine($"Team {teamName} scored {totalGoals} goals in {year}");

        // Output expected:
        // Team Paris Saint-Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static async Task<int> GetTotalScoredGoals(string team, int year)
    {
        int totalGoals = 0;

        // Faz a primeira requisição para obter o total de páginas
        HttpResponseMessage response = await MakeApiRequest(team, year, 1, "team1");
        var responseString = JsonConvert.DeserializeObject<ApiResponse>(await response.Content.ReadAsStringAsync());

        if (responseString == null || responseString.Data == null)
            return totalGoals;

        // Itera por todas as páginas de resultados para `team1`
        foreach (var page in Enumerable.Range(1, responseString.TotalPages))
        {
            response = await MakeApiRequest(team, year, page, "team1");
            responseString = JsonConvert.DeserializeObject<ApiResponse>(await response.Content.ReadAsStringAsync());

            foreach (var match in responseString.Data)
            {
                totalGoals += int.Parse(match.Team1Goals);
            }
        }

        // Agora busca como `team2`
        response = await MakeApiRequest(team, year, 1, "team2");
        responseString = JsonConvert.DeserializeObject<ApiResponse>(await response.Content.ReadAsStringAsync());

        if (responseString == null || responseString.Data == null)
            return totalGoals;

        // Itera por todas as páginas de resultados para `team2`
        foreach (var page in Enumerable.Range(1, responseString.TotalPages))
        {
            response = await MakeApiRequest(team, year, page, "team2");
            responseString = JsonConvert.DeserializeObject<ApiResponse>(await response.Content.ReadAsStringAsync());

            foreach (var match in responseString.Data)
            {
                totalGoals += int.Parse(match.Team2Goals);
            }
        }

        return totalGoals;
    }

    private static async Task<HttpResponseMessage> MakeApiRequest(string team, int year, int page, string teamType)
    {
        HttpClient client = new();
        string baseUrl = "https://jsonmock.hackerrank.com/api/football_matches";

        string url = $"{baseUrl}?year={year}&{teamType}={team}&page={page}";

        return await client.GetAsync(url);
    }

    // API Response Models
    public class ApiResponse
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }
        public List<Partidas> Data { get; set; }
    }
}
