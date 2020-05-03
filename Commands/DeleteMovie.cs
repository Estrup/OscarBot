using System;
using System.Threading;
using System.Threading.Tasks;
using OscarBot.Services;
using Microsoft.Extensions.DependencyInjection;
using OscarBot.Models;
using Microsoft.EntityFrameworkCore;

namespace OscarBot.Commands
{
    public class DeleteMovie : BaseDiscordCmd { public string IdOrUrl { get; set; } }

    public class DeleteMovieHandler : MediatR.AsyncRequestHandler<DeleteMovie>
    {
        private readonly OmdbService omdbService;
        private readonly IServiceProvider serviceProvider;

        public DeleteMovieHandler(
            OmdbService omdbService,
            IServiceProvider serviceProvider
            )
        {
            this.omdbService = omdbService;
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(DeleteMovie request, CancellationToken cancellationToken)
        {
            var idOrUrl = request.IdOrUrl;
            var Context = request.Context;

            var result = await omdbService.Get(idOrUrl);
            if (result == null)
            {
                await Context.Channel.SendMessageAsync($"No movie with that id was found...");
            }

            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();
            if (!await db.Movie.AnyAsync(x => x.Id == result.imdbID))
            {
                await Context.Channel.SendMessageAsync($"***{ result.Title }*** was not on the list!");
                return;
            }
            var movie = await db.Movie.SingleAsync(x => x.Id == result.imdbID);
            db.Movie.Remove(movie);

            await db.SaveChangesAsync();

            await Context.Channel.SendMessageAsync($"***{ result.Title }*** was removed from the watchlist");
        }
    }
}