using Discord;
using Discord.Commands;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Anti_Dox.Database;
using Discord.WebSocket;

using Discord.Net;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Anti_Dox.Services;


namespace Anti_Dox.CustomPreattributes
{
    public class RequireTeamOwner : PreconditionAttribute
    {
        public RequireTeamOwner()
        {

        }

        public override Task<PreconditionResult>CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            //get user
            var User = context.User as IUser;
            //check for correct id
            return Task.FromResult(User.Id == 652248874873782272
                //happy if correct id
                ? PreconditionResult.FromSuccess()
                //insult person if faulty id
                : PreconditionResult.FromError("Only the bot owner can run this command"));
        }
    }
}
