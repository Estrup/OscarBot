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
        private readonly OmdbService omdbService;
        private readonly IServiceProvider serviceProvider;

        public AddWatchedHandler(
            OmdbService omdbService,
            IServiceProvider serviceProvider
            )
        {
            this.omdbService = omdbService;
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(AddWatched request, CancellationToken cancellationToken)
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

            if (await db.Movie.AsQueryable().AnyAsync(x => x.Id == result.imdbID))
            {
                var movie = await db.Movie.AsQueryable().SingleAsync(x => x.Id == result.imdbID);
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
                    AddedByUsername = Context.User.Username,
                    AddedAt = DateTime.Now
                });
            }
            await db.SaveChangesAsync();

            await Context.Channel.SendMessageAsync($"***{ result.Title }*** has now been watched");
        }
    }
}