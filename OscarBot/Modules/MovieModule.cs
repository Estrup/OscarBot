using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OscarBot.Services;
using Microsoft.EntityFrameworkCore;
using OscarBot.Models;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MediatR;
using OscarBot.Commands;

namespace OscarBot.Modules
{
    public class MovieModule : ModuleBase<SocketCommandContext>
    {
        public IServiceProvider ServiceProvider { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        public IMediator Mediator { get; set; }


        [Command("add")]
        public async Task Add(string idOrUrl) =>
            await Mediator.Send(new AddMovie { Context = Context, IdOrUrl = idOrUrl });

        [Command("delete")]
        [Alias("del")]
        public async Task Delete(string idOrUrl) =>

            await Mediator.Send(new DeleteMovie { Context = Context, IdOrUrl = idOrUrl });

        [Command("addpick")]
        public async Task AddPick(int eventid, string idOrUrl) =>
            await Mediator.Send(new AddPick
            {
                Context = Context,
                IdOrUrl = idOrUrl,
                EventId = eventid
            });

        [Command("delpick")]
        public async Task DeletePick(int eventid, string idOrUrl) =>
            await Mediator.Send(new DeletePick
            {
                Context = Context,
                IdOrUrl = idOrUrl,
                EventId = eventid
            });

        [Command("watched")]
        public async Task AddWatched(string idOrUrl) =>
           await Mediator.Send(new AddWatched { Context = Context, IdOrUrl = idOrUrl });

        [Command("watchlist")]
        public async Task GetWatchlist() =>
            await Mediator.Send(new GetWatchlist { Context = Context });

        [Command("watched")]
        public async Task GetWatched() =>
            await Mediator.Send(new GetWatchedList { Context = Context });


        [Command("randompicks")]
        public async Task GenerateRandomPicks(int eventid, int num = 1) =>
            await Mediator.Send(new GenerateRandomPicks
            {
                Context = Context,
                EventId = eventid,
                PickCount = num
            });

        [Command("event")]
        public async Task ViewEvent(int eventid) =>
            await Mediator.Send(new GetEvent
            {
                Context = Context,
                EventId = eventid
            });

        [Command("completeevent")]
        public async Task CompleteEvent(int eventid) =>
            await Mediator.Send(new CompleteEvent { Context = Context, EventId = eventid });

        [Command("addevent")]
        public async Task AddEvent(string title, DateTime start, DateTime? end = null)
            => await Mediator.Send(new AddEvent
            {
                Context = Context,
                Title = title,
                Start = start,
                End = end
            });

        [Command("delevent")]
        public async Task DeleteEvent(int eventid) =>
            await Mediator.Send(new DeleteEvent { Context = Context, EventId = eventid });

        [Command("eventlist")]
        public async Task GetEventList(string type = "") =>
            await Mediator.Send(new GetEventlist { Context = Context, Type = type });

        [Command("setup")]
        public async Task Setup(string action, string value = null) =>
            await Mediator.Send(new SetupServer{ Context = Context, Action = action, Value = value});

        [Command("help")]
        public async Task GetHelp()
        {

            var returnstring = new StringBuilder();
            returnstring.AppendLine("Misc");
            returnstring.AppendLine("<idOrUrl> refers to imdb key or the url of a title. Either works.");
            returnstring.AppendLine(" ");
            returnstring.AppendLine($"!m add <idOrUrl> - *Adds movie to watchlist*");
            returnstring.AppendLine($"!m del <idOrUrl> - *Deletes movie from watchlist*");
            returnstring.AppendLine($"!m addevent \"<title>\" <start> <end> - *Create a new event, Dateformat: yyyy-mm-ddThh:mm End is optional*");
            returnstring.AppendLine($"!m delevent <eventid> - *deletes event*");
            returnstring.AppendLine($"!m randompicks <eventid> <n> - *Picks <n> movies for event. Defaults to 1*");
            returnstring.AppendLine($"!m addpick <eventid> <idOrUrl> - *Manually add movie to event*");
            returnstring.AppendLine($"!m delpick <eventid> <idOrUrl> - *Manually remove movie from event*");
            returnstring.AppendLine($"!m completeevent <eventid> - *Mark movied as watched and the event as completed*");
            returnstring.AppendLine($"!m watched - *Shows a list of already watched movies*");
            returnstring.AppendLine($"!m watched <idOrUrl> - *Marks a title as watched*");
            returnstring.AppendLine($"!m watchlist  - *Shows a the complete watchlist*");
            returnstring.AppendLine($"!m eventlist  - *Shows upcoming events*");
            returnstring.AppendLine($"!m eventlist completed  - *Shows completed events*");

            returnstring.AppendLine($"!m search \"<term>\"  - *Search imdb for movie or show*");
            var eb = new EmbedBuilder() { Title = "This is what i know...", Description = returnstring.ToString(), Color = Color.DarkBlue };
            await Context.Channel.SendMessageAsync(embed: eb.Build());
        }

    }
}
