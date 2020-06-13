using System;
using System.Threading;
using System.Threading.Tasks;
using OscarBot.Services;
using Microsoft.Extensions.DependencyInjection;
using OscarBot.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace OscarBot.Commands
{
    public class AddWatched : BaseDiscordCmd
    {
        public string IdOrUrl { get; set; }
    }

    public class AddWatchedHandler : MediatR.AsyncRequestHandler<AddWatched>
    {
        private readonly TmdbService tmdbService;
        private readonly IServiceProvider serviceProvider;

        public AddWatchedHandler(
            TmdbService tmdbService,
            IServiceProvider serviceProvider
            )
        {
            this.tmdbService = tmdbService;
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(AddWatched request, CancellationToken cancellationToken)
        {
            var idOrUrl = request.IdOrUrl;
            var Context = request.Context;

            var result = await tmdbService.Get(idOrUrl);
            if (result == null)
            {
                await Context.Channel.SendMessageAsync($"No movie with that id was found...");
            }

            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            if (await db.Movie.AsQueryable().AnyAsync(x => x.ImdbId == result.ImdbId))
            {
                var movie = await db.Movie.AsQueryable().SingleAsync(x => x.ImdbId == result.ImdbId);
                movie.Watched = true;
                movie.WatchedDate = DateTime.Now;
            }
            else
            {
                db.Movie.Add(Movie.FromTmdbMovie(result, Context.User));
            }
            await db.SaveChangesAsync();

            await Context.Channel.SendMessageAsync($"***{ result.Title }*** has now been watched");
        }
    }
}