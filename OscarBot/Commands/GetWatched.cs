using System;
using System.Threading;
using System.Threading.Tasks;
using OscarBot.Services;
using Microsoft.Extensions.DependencyInjection;
using OscarBot.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Discord;
using OscarBot.Tables;

namespace OscarBot.Commands
{
    public class GetWatchedList : BaseDiscordCmd
    {
    }

    public class GetWatchedListHandler : MediatR.AsyncRequestHandler<GetWatchedList>
    {
        private readonly IServiceProvider serviceProvider;

        public GetWatchedListHandler(
            IServiceProvider serviceProvider
            )
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task Handle(GetWatchedList request, CancellationToken cancellationToken)
        {
            var Context = request.Context;
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var list = await db.Movie.AsQueryable().Where(x => x.Watched)
                 .OrderBy(x => x.Title)
                 .ToListAsync();
            var returnstring = new StringBuilder();

            //foreach (var item in list)
            //{
            //    returnstring.AppendLine($"**{ item.Title }** *https://www.imdb.com/title/{ item.Id}*");
            //}
            //var eb = new EmbedBuilder() { Title = "We have watched these movies", Description = returnstring.ToString(), Color = Color.DarkBlue };
            //await Context.Channel.SendMessageAsync(embed: eb.Build());

            var builder = new TableBuilder<Movie>()
            {
                MaxPageLength = 2000 - 6
            };

            builder.AddColumn("Title", "Title", 25);
            //builder.AddColumn("Director", "Director", 20);
            builder.AddColumn("WatchedDate", "WatchedDate");
            var pages = builder.Build(list);
            foreach (var page in pages)
            {
                await Context.Channel.SendMessageAsync("```" + page + "```");
            }
        }
    }
}