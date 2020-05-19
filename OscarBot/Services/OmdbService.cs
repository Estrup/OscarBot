using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OscarBot.Services
{
    public class OmdbService
    {
        private readonly string _key;
        private readonly HttpClient _http;

        public OmdbService(HttpClient http)
        {
            _key = Environment.GetEnvironmentVariable("OmdbKey");
            _http = http;
        }

        public async Task<List<OmdbResult>> Search(string term)
        {
            var url = $"http://www.omdbapi.com/?apikey={_key}&s={term}";
            var resp = await _http.GetAsync(url);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<OmdbSearch>(json).Search;
        }

        public async Task<OmdbResult> GetTitle(string id)
        {
            var url = $"http://www.omdbapi.com/?apikey={_key}&i={id}";
            var resp = await _http.GetAsync(url);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<OmdbResult>(json);
        }

        public async Task<OmdbResult> Get(string idOrUrl)
        {
            var id = "";
            if (idOrUrl.StartsWith("http") || idOrUrl.StartsWith("www"))
            {
                var reg = new Regex(@"\/title\/([a-zA-Z0-9_.-]*)", RegexOptions.IgnoreCase);
                var match = reg.Match(idOrUrl);
                if (match.Success)
                {
                    id = match.Groups[1].Value;
                }
                else
                {
                    return null;
                }
            }
            else
                id = idOrUrl;

            var result = await GetTitle(id);
            return result;
        }
    }

    public class OmdbSearch
    {
        public List<OmdbResult> Search { get; set; }
    }
    public class OmdbResult
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Rated { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Actors { get; set; }
        public string Plot { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Awards { get; set; }
        public string Poster { get; set; }
        public string Metascore { get; set; }
        public string imdbRating { get; set; }
        public string imdbVotes { get; set; }
        public string imdbID { get; set; }
        public string Type { get; set; }
        public bool Response { get; set; }
    }
}
