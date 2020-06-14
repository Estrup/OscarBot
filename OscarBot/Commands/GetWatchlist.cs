using System;
using System.Threading;
using System.Threading.Tasks;
using OscarBot.Services;
using Microsoft.Extensions.DependencyInjection;
using OscarBot.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using Discord;
using OscarBot.Tables;

namespace OscarBot.Commands
{
    public class GetWatchlist : BaseDiscordCmd
    {
    }

    public class GetWatchlistHandler : MediatR.AsyncRequestHandler<GetWatchlist>
    {
        private readonly IServiceProvider serviceProvider;

        public GetWatchlistHandler(
            IServiceProvider serviceProvider
            )
        {
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(GetWatchlist request, CancellationToken cancellationToken)
        {
            var Context = request.Context;
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var list = await db.Movie.AsQueryable().Where(x => !x.Watched)
                .OrderBy(x => x.Title)
                .ToListAsync();

            var builder = new TableBuilder<Movie>() {
                MaxPageLength = 1900
            };
           
            builder.AddColumn("Title", "Title", 30);
            //builder.AddColumn("Director", "Director", 20);
            builder.AddColumn("Runtime", "Runtime");
            builder.AddColumn("AddedBy", "AddedByUsername");
            var pages = builder.Build(list);
            foreach (var page in pages)
            {
                await Context.Channel.SendMessageAsync("```" + page + "```");
            }
           
        }
    }
}