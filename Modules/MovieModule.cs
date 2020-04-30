using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OscarBot.Services;
using Microsoft.EntityFrameworkCore;
using OscarBot.Models;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace OscarBot.Modules
{
    public class MovieModule : ModuleBase<SocketCommandContext>
    {
        // Dependency Injection will fill this value in for us
        public OmdbService OmdbService { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }

        [Command("search")]
        public async Task Search(params string[] terms)
        {
            var searchterm = string.Join(" ", terms);
            var result = await OmdbService.Search(searchterm);
            var returnstring = new StringBuilder();
            returnstring.AppendLine($"I found these titles");
            foreach (var item in result.Take(5))
            {
                returnstring.AppendLine($"{ item.Title }({ item.Year }) *https://www.imdb.com/title/{ item.imdbID}*");
            }
            var eb = new EmbedBuilder() { Title = "Search results", Description = returnstring.ToString(), Color = Color.DarkOrange };

            //await Context.Channel.SendMessageAsync(returnstring.ToString());
            await Context.Channel.SendMessageAsync(embed: eb.Build());
        }

        [Command("add")]
        public async Task Add(string idOrUrl)
        {
            var result = await OmdbService.Get(idOrUrl);
            if (result == null)
            {
                await Context.Channel.SendMessageAsync($"No movie with that id was found...");
            }

            using var scope = this.ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();
            if (await db.Movie.AsQueryable().AnyAsync(x => x.Id == result.imdbID))
            {
                var movie = await db.Movie.AsQueryable().SingleAsync(x => x.Id == result.imdbID);
                movie.Watched = false;
            }
            else
            {
                db.Movie.Add(new Movie
                {
                    Id = result.imdbID,
                    Title = result.Title,
                    Plot = result.Plot,
                    Actors = result.Actors,
                    Director = result.Director,
                    Language = result.Director,
                    Runtime = result.Runtime,
                    AddedBy = Context.User.Id.ToString(),
                    AddedByUsername = Context.User.Username
                });
            }

            await db.SaveChangesAsync();

            await Context.Channel.SendMessageAsync($"***{ result.Title }*** added to list");
        }


        [Command("delete")]
        [Alias("del")]
        public async Task Delete(string idOrUrl)
        {

            var result = await OmdbService.Get(idOrUrl);
            if (result == null)
            {
                await Context.Channel.SendMessageAsync($"No movie with that id was found...");
            }

            using var scope = this.ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();
            if (!await db.Movie.AsQueryable().AnyAsync(x => x.Id == result.imdbID))
            {
                await Context.Channel.SendMessageAsync($"***{ result.Title }*** was not on the list!");
                return;
            }
            var movie = await db.Movie.AsQueryable().SingleAsync(x => x.Id == result.imdbID);
            db.Movie.Remove(movie);

            await db.SaveChangesAsync();

            await Context.Channel.SendMessageAsync($"***{ result.Title }*** was removed from the watchlist");
        }

        [Command("addpick")]
        public async Task AddPick(int eventid, string idOrUrl)
        {

            var result = await OmdbService.Get(idOrUrl);
            if (result == null)
            {
                await Context.Channel.SendMessageAsync($"No movie with that id was found...");
            }

            using var scope = this.ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var mevent = await db.Event
                .Include(x => x.EventMovies)
                .ThenInclude(x => x.Movie)
                .AsQueryable().SingleOrDefaultAsync(x => x.Id == eventid);
            if (mevent == null)
            {
                await Context.Channel.SendMessageAsync($"Event not found...");
                return;
            }

            var alreadyselected = mevent.EventMovies.Select(x => x.Movie);

            if (alreadyselected.Any(x => x.Id == result.imdbID)) await Context.Channel.SendMessageAsync($"Movie already added to event..");

            Movie movie;
            if (await db.Movie.AsQueryable().AnyAsync(x => x.Id == result.imdbID))
            {
                movie = await db.Movie.AsQueryable().SingleAsync(x => x.Id == result.imdbID);

            }
            else
            {
                movie = new Movie
                {
                    Id = result.imdbID,
                    Title = result.Title,
                    Plot = result.Plot,
                    Actors = result.Actors,
                    Director = result.Director,
                    Language = result.Director,
                    Runtime = result.Runtime,
                    Picked = true,
                    AddedBy = Context.User.Id.ToString(),
                    AddedByUsername = Context.User.Username
                };
            }
            mevent.EventMovies.Add(new EventMovie { Movie = movie });
            await db.SaveChangesAsync();

            await Context.Channel.SendMessageAsync($"***{ result.Title }*** added to event");
        }

        [Command("delpick")]
        public async Task DeletePick(int eventid, string idOrUrl)
        {

            var result = await OmdbService.Get(idOrUrl);
            if (result == null)
            {
                await Context.Channel.SendMessageAsync($"No movie with that id was found...");
            }

            using var scope = this.ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var mevent = await db.Event
                .Include(x => x.EventMovies)
                .ThenInclude(x => x.Movie)
                .AsQueryable().SingleOrDefaultAsync(x => x.Id == eventid);
            if (mevent == null)
            {
                await Context.Channel.SendMessageAsync($"Event not found...");
                return;
            }

            var em = mevent.EventMovies.SingleOrDefault(x => x.MovieId == result.imdbID);
            if (em != null)
            {
                mevent.EventMovies.Remove(em);
                await db.SaveChangesAsync();
            }

            await Context.Channel.SendMessageAsync($"***{ result.Title }*** removed from event");
        }

        [Command("watched")]
        public async Task AddWatched(string idOrUrl)
        {
            var result = await OmdbService.Get(idOrUrl);
            if (result == null)
            {
                await Context.Channel.SendMessageAsync($"No movie with that id was found...");
            }

            using var scope = this.ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            if (await db.Movie.AsQueryable().AnyAsync(x => x.Id == result.imdbID))
            {
                var movie = await db.Movie.AsQueryable().SingleAsync(x => x.Id == result.imdbID);
                movie.Picked = false;
                movie.Watched = true;
                movie.WatchedDate = DateTime.Now;
            }
            else
            {
                db.Movie.Add(new Movie
                {
                    Id = result.imdbID,
                    Title = result.Title,
                    Plot = result.Plot,
                    Actors = result.Actors,
                    Director = result.Director,
                    Language = result.Director,
                    Runtime = result.Runtime,
                    Watched = true,
                    WatchedDate = DateTime.Now,
                    AddedBy = Context.User.Id.ToString(),
                    AddedByUsername = Context.User.Username
                });
            }
            await db.SaveChangesAsync();

            await Context.Channel.SendMessageAsync($"***{ result.Title }*** has now been watched");
        }

        [Command("watchlist")]
        public async Task GetWatchlist()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var list = await db.Movie.AsQueryable().Where(x => !x.Watched).ToListAsync();
            var returnstring = new StringBuilder();
            //returnstring.AppendLine($"I found these titles");
            foreach (var item in list)
            {
                returnstring.AppendLine($"**{ item.Title }** *https://www.imdb.com/title/{ item.Id}*");
            }
            var eb = new EmbedBuilder() { Title = "To-watch list", Description = returnstring.ToString(), Color = Color.DarkBlue };
            await Context.Channel.SendMessageAsync(embed: eb.Build());
        }

        [Command("watched")]
        public async Task GetWatched()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var list = await db.Movie.AsQueryable().Where(x => x.Watched).ToListAsync();
            var returnstring = new StringBuilder();
            //returnstring.AppendLine($"I found these titles");
            foreach (var item in list)
            {
                returnstring.AppendLine($"**{ item.Title }** *https://www.imdb.com/title/{ item.Id}*");
            }
            var eb = new EmbedBuilder() { Title = "We have watched these movies", Description = returnstring.ToString(), Color = Color.DarkBlue };
            await Context.Channel.SendMessageAsync(embed: eb.Build());
        }

        private string[] consults = new string[] { "horosopes", "the oracles", "bill gates" };

        [Command("randompicks")]
        public async Task GenerateRandomPicks(int eventid, int num = 1)
        {
            using var scope = this.ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var mevent = await db.Event
                .Include(x => x.EventMovies)
                .ThenInclude(x => x.Movie)
                .AsQueryable().SingleOrDefaultAsync(x => x.Id == eventid);
            if (mevent == null)
            {
                await Context.Channel.SendMessageAsync($"Event not found...");
                return;
            }

            var random = new Random();

            IEnumerable<Movie> list = await db.Movie.AsQueryable().Where(x => !x.Watched).ToListAsync();
            var alreadyselected = mevent.EventMovies.Select(x => x.Movie);

            var picked = new List<Movie>();

            await Context.Channel.SendMessageAsync($"... Consulting { this.consults.ElementAt(random.Next(consults.Count())) }...");
            await Task.Delay(1500);

            for (int i = 0; i < num; i++)
            {
                var randomlist = list.Except(picked).Except(alreadyselected);
                var idx = random.Next(randomlist.Count());
                var pick = randomlist.ElementAt(idx);
                picked.Add(pick);
                await Context.Channel.SendMessageAsync($"Pick # { i + 1 } " + System.Environment.NewLine + $"**{ pick.Title }** https://www.imdb.com/title/{ pick.Id}");
                await Task.Delay(1500);
            }
            foreach (var m in picked)
            {
                mevent.EventMovies.Add(new EventMovie { Movie = m });
            }

            await db.SaveChangesAsync();
        }

        [Command("event")]
        public async Task ViewEvent(int eventid)
        {
            using var scope = this.ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var mevent = await db.Event
                .Include(x => x.EventMovies)
                .ThenInclude(x => x.Movie)
                .AsQueryable().SingleOrDefaultAsync(x => x.Id == eventid);
            if (mevent == null)
            {
                await Context.Channel.SendMessageAsync($"Event not found...");
                return;
            }

            var description = $"'!m randompicks { mevent.Id } <n>' to pick <n> random movies for the event";
            description = description + Environment.NewLine + $"'!m addpick { mevent.Id } <idOrUrl>' to add a specific movie";

            var eb = new EmbedBuilder() { Title = "Event: " + mevent.Title, Description = description, Color = Color.DarkBlue };
            eb.AddField("Id", mevent.Id.ToString(), true);
            eb.AddField("Start", mevent.EventStart.ToString("dd-MM-yyyy HH:mm"), true);
            if (mevent.EventEnd != null)
                eb.AddField("End", mevent.EventEnd.Value.ToString("dd-mm-yyyy HH:mm"), true);

            await Context.Channel.SendMessageAsync(embed: eb.Build());

            var list = mevent.EventMovies.Select(x => x.Movie);
            int i = 0;
            if (list.Count() == 0)
            {
                await Context.Channel.SendMessageAsync($"There is nothing here :(");
                return;
            }
            foreach (var pick in list)
            {
                await Context.Channel.SendMessageAsync($"Pick # { i + 1 } " + System.Environment.NewLine + $"**{ pick.Title }** https://www.imdb.com/title/{ pick.Id}");
                i++;
                await Task.Delay(1500);
            }
        }

        [Command("completeevent")]
        public async Task CompleteEvent(int eventid)
        {
            using var scope = this.ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var mevent = await db.Event
                .Include(x => x.EventMovies)
                .ThenInclude(x => x.Movie)
                .AsQueryable().SingleOrDefaultAsync(x => x.Id == eventid);
            if (mevent == null)
            {
                await Context.Channel.SendMessageAsync($"Event not found...");
                return;
            }

            mevent.Completed = true;
            foreach (var m in mevent.EventMovies.Select(x => x.Movie))
            {
                m.Watched = true;
                m.WatchedDate = mevent.EventStart;
            }

            await db.SaveChangesAsync();

            await Context.Channel.SendMessageAsync($"Event and movie marked as watched!");
        }

        // Get info on a user, or the user who invoked the command if one is not specified
        [Command("adduser")]
        public async Task UserInfoAsync(IUser user, string role)
        {
            using var scope = this.ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var uuser = await db.User.AsQueryable().SingleOrDefaultAsync(x => x.Id == user.Id);
            if (uuser == null)
            {
                db.User.Add(new User
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = role.ToUpper()
                });
            }
            else
            {
                uuser.Role = role.ToUpper();
            }
            await db.SaveChangesAsync();
        }

        [Command("addevent")]
        public async Task AddEvent(string title, DateTime start, DateTime? end = null)
        {
            using var scope = this.ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();
            var mevent = new Event
            {
                Title = title,
                EventEnd = end,
                EventStart = start,
                AddedBy = Context.User.Id.ToString(),
                AddedByUsername = Context.User.Username
            };
            db.Event.Add(mevent);

            await db.SaveChangesAsync();
            var description = $"'!m randompicks { mevent.Id } <n>' to pick <n> random movies for the event";
            description = description + Environment.NewLine + $"'!m addpick { mevent.Id } <idOrUrl>' to add a specific movie";

            var eb = new EmbedBuilder() { Title = "Event created: " + mevent.Title, Description = description, Color = Color.DarkBlue };
            eb.AddField("Id", mevent.Id.ToString(), true);
            eb.AddField("Start", start.ToString("dd-MM-yyyy HH:mm"), true);
            if (end != null)
                eb.AddField("End", end.Value.ToString("dd-mm-yyyy HH:mm"), true);
            await Context.Channel.SendMessageAsync(embed: eb.Build());
        }

        [Command("delevent")]
        public async Task DeleteEvent(int eventid)
        {
            using var scope = this.ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var mevent = await db.Event
                .Include(x => x.EventMovies)
                .AsQueryable().SingleOrDefaultAsync(x => x.Id == eventid);
            if (mevent == null)
            {
                await Context.Channel.SendMessageAsync($"Event not found...");
                return;
            }

            db.Event.Remove(mevent);

            await db.SaveChangesAsync();
            await Context.Channel.SendMessageAsync($"Event deleted...");
        }

        [Command("eventlist")]
        public async Task GetEventList(string type = "")
        {
            using var scope = this.ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var list = await db.Event
                .Include(x => x.EventMovies)
                .ThenInclude(x => x.Movie)
                .AsQueryable().Where(x => x.Completed == (type == "completed")).OrderByDescending(x => x.EventStart)
                .Take(5).ToListAsync();


            foreach (var item in list)
            {
                var returnstring = new StringBuilder();
                var blurb = "";

                if (item.EventMovies.Count > 0)
                {
                    foreach (var m in item.EventMovies.Select(x => x.Movie))
                    {
                        returnstring.AppendLine($"- **{ m.Title }**");
                    }
                    //var movie = item.EventMovies.First().Movie;
                    //blurb = movie.Title.Substring(0, movie.Title.Length < 20 ? movie.Title.Length : 20);
                    //returnstring.AppendLine($"**Id { item.Id }** *Start: { item.EventStart:dd-MM-yyyy HH:mm}*  {blurb}...");
                }
                var eb = new EmbedBuilder()
                {
                    Title = item.Title,
                    Description = returnstring.ToString(),
                    Color = Color.DarkBlue
                };
                eb.AddField("Id", item.Id.ToString(), true);
                eb.AddField("Start", item.EventStart.ToString("dd-MM-yyyy HH:mm"), true);
                if (item.EventEnd != null)
                    eb.AddField("End", item.EventEnd.Value.ToString("dd-mm-yyyy HH:mm"), true);
                await Context.Channel.SendMessageAsync(embed: eb.Build());
            }

        }

        [Command("help")]
        public async Task GetHelp()
        {

            var returnstring = new StringBuilder();
            returnstring.AppendLine("Misc");
            returnstring.AppendLine("<idOrUrl> refers to imdb key or the url of a title. Either works.");
            returnstring.AppendLine(" ");
            returnstring.AppendLine($"!m add <idOrUrl> - *Adds movie to watchlist*");
            returnstring.AppendLine($"!m del <idOrUrl> - *Deletes movie from watchlist*");
            returnstring.AppendLine($"!m addevent \"<title>\" <start> <end> - *Create a new event, Dateformat: yyyy-mm-ddThh:mm End is optional*");
            returnstring.AppendLine($"!m delevent <eventid> - *deletes event*");
            returnstring.AppendLine($"!m randompicks <eventid> <n> - *Picks <n> movies for event. Defaults to 1*");
            returnstring.AppendLine($"!m addpick <eventid> <idOrUrl> - *Manually add movie to event*");
            returnstring.AppendLine($"!m delpick <eventid> <idOrUrl> - *Manually remove movie from event*");
            returnstring.AppendLine($"!m completevent <eventid> - *Mark movied as watched and the event as completed*");
            returnstring.AppendLine($"!m watched - *Shows a list of already watched movies*");
            returnstring.AppendLine($"!m watched <idOrUrl> - *Marks a title as watched*");
            returnstring.AppendLine($"!m watchlist  - *Shows a the complete watchlist*");
            returnstring.AppendLine($"!m eventlist  - *Shows upcoming events*");
            returnstring.AppendLine($"!m eventlist completed  - *Shows completed events*");

            returnstring.AppendLine($"!m search \"<term>\"  - *Search imdb for movie or show*");
            var eb = new EmbedBuilder() { Title = "This is what i know...", Description = returnstring.ToString(), Color = Color.DarkBlue };
            await Context.Channel.SendMessageAsync(embed: eb.Build());
        }
        //// Ban a user
        //[Command("ban")]
        //[RequireContext(ContextType.Guild)]
        //// make sure the user invoking the command can ban
        //[RequireUserPermission(GuildPermission.BanMembers)]
        //// make sure the bot itself can ban
        //[RequireBotPermission(GuildPermission.BanMembers)]
        //public async Task BanUserAsync(IGuildUser user, [Remainder] string reason = null)
        //{
        //    await user.Guild.AddBanAsync(user, reason: reason);
        //    await ReplyAsync("ok!");
        //}

        //// [Remainder] takes the rest of the command's arguments as one argument, rather than splitting every space
        //[Command("echo")]
        //public Task EchoAsync([Remainder] string text)
        //    // Insert a ZWSP before the text to prevent triggering other bots!
        //    => ReplyAsync('\u200B' + text);

        //// Setting a custom ErrorMessage property will help clarify the precondition error
        //[Command("guild_only")]
        //[RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        //public Task GuildOnlyCommand()
        //    => ReplyAsync("Nothing to see here!");
    }
}
