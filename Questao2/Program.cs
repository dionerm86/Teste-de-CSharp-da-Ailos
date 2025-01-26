using Newtonsoft.Json;
using Questao2;

public class Program
{
    private const int FIRSTPAGE = 1;

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
        List<Partidas> allMatches = new();

        HttpResponseMessage team1Response = await HttpRequest(team, year, 1, "team1");
        var responseString = JsonConvert.DeserializeObject<ApiResponse>(await team1Response.Content.ReadAsStringAsync());

        if (responseString != null && responseString.Data != null)
        {
            foreach (var currentPage in Enumerable.Range(1, responseString.TotalPages))
            {
                if (currentPage > FIRSTPAGE)
                {
                    var pageResponse = await HttpRequest(team, year, currentPage, "team1");
                    var pageData = JsonConvert.DeserializeObject<ApiResponse>(await pageResponse.Content.ReadAsStringAsync());

                    if (pageData?.Data != null)
                        allMatches.AddRange(pageData.Data);
                }
                else
                    allMatches.AddRange(responseString.Data);
            }
        }

        HttpResponseMessage team2Response = await HttpRequest(team, year, 1, "team2");
        responseString = JsonConvert.DeserializeObject<ApiResponse>(await team2Response.Content.ReadAsStringAsync());

        if (responseString != null && responseString.Data != null)
        {
            foreach (var currentePage in Enumerable.Range(1, responseString.TotalPages))
            {
                if (currentePage > FIRSTPAGE)
                {
                    var pageResponse = await HttpRequest(team, year, currentePage, "team2");
                    var pageData = JsonConvert.DeserializeObject<ApiResponse>(await pageResponse.Content.ReadAsStringAsync());
                    if (pageData?.Data != null)
                        allMatches.AddRange(pageData.Data);
                }
                else
                    allMatches.AddRange(responseString.Data);

            }
        }

        foreach (var match in allMatches)
        {
            if (match.Team1 == team && !string.IsNullOrEmpty(match.Team1Goals))
            {
                totalGoals += int.Parse(match.Team1Goals);
            }

            if (match.Team2 == team && !string.IsNullOrEmpty(match.Team2Goals))
            {
                totalGoals += int.Parse(match.Team2Goals);
            }
        }

        return totalGoals;
    }

    private static async Task<HttpResponseMessage> HttpRequest(string team, int year, int page, string teamType)
    {
        HttpClient client = new();
        string baseUrl = "https://jsonmock.hackerrank.com/api/football_matches";

        string url = $"{baseUrl}?year={year}&{teamType}={team}&page={page}";

        return await client.GetAsync(url);
    }

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
