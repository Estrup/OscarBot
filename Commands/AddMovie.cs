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
        private readonly OmdbService omdbService;
        private readonly IServiceProvider serviceProvider;

        public AddMovieHandler(
            OmdbService omdbService,
            IServiceProvider serviceProvider
            )
        {
            this.omdbService = omdbService;
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(AddMovie request, CancellationToken cancellationToken)
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
            if (await db.Movie.AnyAsync(x => x.Id == result.imdbID))
            {
                var movie = await db.Movie.SingleAsync(x => x.Id == result.imdbID);
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
    }
}