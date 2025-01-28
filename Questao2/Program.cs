using Newtonsoft.Json;
using Questao2;

public class Program : HttpService
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

        List<Match> allMatches = new();
        allMatches.AddRange(await GetMatchesForTypeTeam(team, year, "team1"));
        allMatches.AddRange(await GetMatchesForTypeTeam(team, year, "team2"));

        foreach (var match in allMatches)
            totalGoals += GetGoalsForTypeTeam(match, team);

        return totalGoals;
    }

    private static async Task<List<Match>> GetMatchesForTypeTeam(string team, int year, string teamType)
    {
        List<Match> matches = new();

        HttpResponseMessage response = await HttpRequest(team, year, 1, teamType);
        var responseString = JsonConvert.DeserializeObject<ResponseApi>(await response.Content.ReadAsStringAsync());

        if (responseString != null && responseString.Data != null)
        {
            foreach (var currentPage in Enumerable.Range(1, responseString.TotalPages))
            {
                if (currentPage > FIRSTPAGE)
                {
                    var pageResponse = await HttpRequest(team, year, currentPage, teamType);
                    var pageData = JsonConvert.DeserializeObject<ResponseApi>(await pageResponse.Content.ReadAsStringAsync());

                    if (pageData?.Data != null)
                        matches.AddRange(pageData.Data);
                }
                else
                {
                    matches.AddRange(responseString.Data);
                }
            }
        }

        return matches;
    }

    private static int GetGoalsForTypeTeam(Match match, string team)
    {
        int goals = 0;

        if (match.Team1 == team && !string.IsNullOrEmpty(match.Team1Goals))
            goals += int.Parse(match.Team1Goals);

        if (match.Team2 == team && !string.IsNullOrEmpty(match.Team2Goals))
            goals += int.Parse(match.Team2Goals);

        return goals;
    }
}
