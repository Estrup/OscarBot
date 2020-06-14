using System;
using System.Threading;
using System.Threading.Tasks;
using OscarBot.Services;
using Microsoft.Extensions.DependencyInjection;
using OscarBot.Models;
using Microsoft.EntityFrameworkCore;
using Discord;

namespace OscarBot.Commands
{
    public class SetupServer : BaseDiscordCmd
    {
        public string Action { get; set; }
        public string Value { get; set; }
    }

    public class SetupServerHandler : MediatR.AsyncRequestHandler<SetupServer>
    {
        private readonly OmdbService omdbService;
        private readonly IServiceProvider serviceProvider;

        public SetupServerHandler(
            OmdbService omdbService,
            IServiceProvider serviceProvider
            )
        {
            this.omdbService = omdbService;
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(SetupServer request, CancellationToken cancellationToken)
        {
            var Context = request.Context;
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            if (request.Action == "init")
            {
                if (await db.Server.AnyAsync(x => x.Id == Context.Guild.Id.ToString()))
                {
                    await Context.Channel.SendMessageAsync("Server alreade initiated");
                    return;
                }

                db.Server.Add(new Server
                {
                    AnnouncementChannelId = Context.Channel.Id.ToString(),
                    AnnounementTriggerTime = "09:00",
                    Id = Context.Guild.Id.ToString(),
                    LastEventNo = 0,
                    Name = Context.Guild.Name,
                    TimeZone = "CET"
                });
                await db.SaveChangesAsync();
                await Context.Channel.SendMessageAsync("Server initiated");
                await Context.Channel.SendMessageAsync("announcement channel: "+ Context.Channel.Name);
                await Context.Channel.SendMessageAsync("announcement time: " + "09:00");
                await Context.Channel.SendMessageAsync("Timezone: CET");

            }
            // var server = await db.Server.SingleAsync(x => x.Id == Context.Guild.Id.ToString());



            //await Context.Channel.SendMessageAsync(embed: eb.Build());
        }
    }
}