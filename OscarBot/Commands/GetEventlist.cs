using System;
using System.Threading;
using System.Threading.Tasks;
using OscarBot.Services;
using Microsoft.Extensions.DependencyInjection;
using OscarBot.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Discord;
using System.Text;

namespace OscarBot.Commands
{
    public class GetEventlist : BaseDiscordCmd
    {
        public string Type { get; set; }
    }

    public class GetEventlistHandler : MediatR.AsyncRequestHandler<GetEventlist>
    {
        private readonly OmdbService omdbService;
        private readonly IServiceProvider serviceProvider;

        public GetEventlistHandler(
            OmdbService omdbService,
            IServiceProvider serviceProvider
            )
        {
            this.omdbService = omdbService;
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(GetEventlist request, CancellationToken cancellationToken)
        {
            var Context = request.Context;
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var list = await db.Event
                .Include(x => x.EventMovies)
                .ThenInclude(x => x.Movie)
                .AsQueryable().Where(x => x.Completed == (request.Type == "completed")).OrderByDescending(x => x.EventStart)
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
    }
}