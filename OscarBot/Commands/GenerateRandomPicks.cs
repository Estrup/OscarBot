using System;
using System.Threading;
using System.Threading.Tasks;
using OscarBot.Services;
using Microsoft.Extensions.DependencyInjection;
using OscarBot.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace OscarBot.Commands
{
    public class GenerateRandomPicks : BaseDiscordCmd
    {
        public int EventId { get; set; }
        public int PickCount { get; set; }
    }

    public class GenerateRandomPicksHandler : MediatR.AsyncRequestHandler<GenerateRandomPicks>
    {
        private readonly IServiceProvider serviceProvider;
        private string[] consults = new string[] { "horosopes", "the oracles", "bill gates" };

        public GenerateRandomPicksHandler(
            IServiceProvider serviceProvider
            )
        {
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(GenerateRandomPicks request, CancellationToken cancellationToken)
        {
            var Context = request.Context;
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var mevent = await db.Event
                .Include(x => x.EventMovies)
                .ThenInclude(x => x.Movie)
                .SingleOrDefaultAsync(x => x.Id == request.EventId);
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

            for (int i = 0; i < request.PickCount; i++)
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
    }
}