using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OscarBot.Services.Tmdb;

namespace OscarBot.Services
{
    public class TmdbService
    {
        private readonly string _key;
        private readonly HttpClient _http;

        public TmdbService(HttpClient http)
        {
            _key = Environment.GetEnvironmentVariable("TmdbToken");
            _http = http;
        }

        public async Task<TmdbMovie> GetTitle(long id)
        {
            var url = $"https://api.themoviedb.org/3/movie/{id}?api_key={_key}&language=en-US&append_to_response=release_dates%2Credits";
            var resp = await _http.GetAsync(url);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TmdbMovie>(json);
        }

        public async Task<TmdbMovie> Get(string idOrUrl)
        {
            var imdbid = "";
            if (idOrUrl.StartsWith("http") || idOrUrl.StartsWith("www"))
            {
                var reg = new Regex(@"\/title\/([a-zA-Z0-9_.-]*)", RegexOptions.IgnoreCase);
                var match = reg.Match(idOrUrl);
                if (match.Success)
                {
                    imdbid = match.Groups[1].Value;
                }
                else
                {
                    return null;
                }
            }
            else
                imdbid = idOrUrl;

            var id = await GetTmdbId(imdbid);
            if (id == null) return null;
            var result = await GetTitle(id.Value);
            return result;
        }

        public async Task<long?> GetTmdbId(string imdbId)
        {
            var url = $"https://api.themoviedb.org/3/find/{imdbId}?api_key={this._key}&language=en-US&external_source=imdb_id";
            var resp = await _http.GetAsync(url);
            var json = await resp.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TmdbSearchResult>(json);
            if (result.MovieResults.Count != 0) return null;

            return result.MovieResults.First().Id;
        }
    }

    //public class OmdbSearch
    //{
    //    public List<OmdbResult> Search { get; set; }
    //}
    //public class OmdbResult
    //{
    //    public string Title { get; set; }
    //    public string Year { get; set; }
    //    public string Rated { get; set; }
    //    public string Released { get; set; }
    //    public string Runtime { get; set; }
    //    public string Genre { get; set; }
    //    public string Director { get; set; }
    //    public string Writer { get; set; }
    //    public string Actors { get; set; }
    //    public string Plot { get; set; }
    //    public string Language { get; set; }
    //    public string Country { get; set; }
    //    public string Awards { get; set; }
    //    public string Poster { get; set; }
    //    public string Metascore { get; set; }
    //    public string imdbRating { get; set; }
    //    public string imdbVotes { get; set; }
    //    public string imdbID { get; set; }
    //    public string Type { get; set; }
    //    public bool Response { get; set; }
    //}
}
