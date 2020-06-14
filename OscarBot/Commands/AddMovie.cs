using System;
using System.Threading;
using System.Threading.Tasks;
using OscarBot.Services;
using Microsoft.Extensions.DependencyInjection;
using OscarBot.Models;
using Microsoft.EntityFrameworkCore;

namespace OscarBot.Commands
{
    public class AddMovie : BaseDiscordCmd { public string IdOrUrl { get; set; } }

    public class AddMovieHandler : MediatR.AsyncRequestHandler<AddMovie>
    {
        private readonly TmdbService tmdbService;
        private readonly IServiceProvider serviceProvider;

        public AddMovieHandler(
            TmdbService tmdbService,
            IServiceProvider serviceProvider
            )
        {
            this.tmdbService = tmdbService;
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(AddMovie request, CancellationToken cancellationToken)
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
            if (await db.Movie.AnyAsync(x => x.ImdbId == result.ImdbId))
            {
                var movie = await db.Movie.SingleAsync(x => x.ImdbId == result.ImdbId );
                movie.Watched = false;
                movie.Watchlist = true;
                movie.ServerId = Context.Guild.Id.ToString();
            }
            else
            {
                db.Movie.Add(Movie.FromTmdbMovie(result, Context.User));
            }

            await db.SaveChangesAsync();

            await Context.Channel.SendMessageAsync($"***{ result.Title }*** added to list");
        }
    }
}