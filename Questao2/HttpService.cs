namespace Questao2
{
    public abstract class HttpService
    {
        protected static async Task<HttpResponseMessage> HttpRequest(string team, int year, int page, string teamType)
        {
            string baseUrl = "https://jsonmock.hackerrank.com/api/football_matches";
            string url = $"{baseUrl}?year={year}&{teamType}={team}&page={page}";

            using (HttpClient client = new())
            {
                return await client.GetAsync(url);
            }
        }
    }
}
