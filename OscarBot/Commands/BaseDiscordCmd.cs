using System;
using Discord.Commands;
using MediatR;

namespace OscarBot.Commands
{
    public abstract class BaseDiscordCmd : IRequest
    {
        public SocketCommandContext Context { get; set; }        
    }
}
