using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using OscarBot.Services;
namespace OscarBot.Commands

{
    public class ImdbSearch : BaseDiscordCmd
    {
        public string term { get; set; }
    }

    public class ImdbSearchHandler : MediatR.AsyncRequestHandler<ImdbSearch>
    {
        private readonly OmdbService omdbService;
        public ImdbSearchHandler(
            OmdbService omdbService)
        {
            this.omdbService = omdbService;
        }

        protected override async Task Handle(ImdbSearch request, CancellationToken cancellationToken)
        {
            var result = await omdbService.Search(request.term);
            var returnstring = new StringBuilder();
            returnstring.AppendLine($"I found these titles");
            foreach (var item in result.Take(5))
            {
                returnstring.AppendLine($"{ item.Title }({ item.Year }) *https://www.imdb.com/title/{ item.imdbID}*");
            }
            var eb = new EmbedBuilder() { Title = "Search results", Description = returnstring.ToString(), Color = Color.DarkOrange };

            //await Context.Channel.SendMessageAsync(returnstring.ToString());
            await request.Context.Channel.SendMessageAsync(embed: eb.Build());
        }
    }
}