using Newtonsoft.Json;

namespace Questao2
{
    public class ResponseApi
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }
        public List<Partidas> Data { get; set; }
    }
}
