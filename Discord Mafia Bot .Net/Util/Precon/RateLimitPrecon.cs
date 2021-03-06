﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;

namespace Discord_Mafia_Bot.Util
{
    //DISCLAIMER: Original from: https://github.com/Joe4evr/Discord.Addons/tree/master/src/Discord.Addons.Preconditions

    /// <summary> Sets how often a user is allowed to use this command
    /// or any command in this module. </summary>
    /// <remarks>This is backed by an in-memory collection
    /// and will not persist with restarts.</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class RatelimitAttribute : PreconditionAttribute
    {
        private readonly uint _invokeLimit;
        private readonly bool _noLimitInDMs;
        private readonly bool _noLimitForAdmins;
        private readonly TimeSpan _invokeLimitPeriod;
        private readonly Dictionary<ulong, CommandTimeout> _invokeTracker = new Dictionary<ulong, CommandTimeout>();

        /// <summary> Sets how often a user is allowed to use this command. </summary>
        /// <param name="times">The number of times a user may use the command within a certain period.</param>
        /// <param name="period">The amount of time since first invoke a user has until the limit is lifted.</param>
        /// <param name="measure">The scale in which the <paramref name="period"/> parameter should be measured.</param>
        /// <param name="noLimitInDMs">Set whether or not there is no limit to the command in DMs. Defaults to false.</param>
        /// <param name="noLimitForAdmins">Set whether or not there is no limit to the command for server admins. Defaults to false.</param>
        public RatelimitAttribute(uint times, double period, Measure measure, bool noLimitInDMs = false, bool noLimitForAdmins = false)
        {
            _invokeLimit = times;
            _noLimitInDMs = noLimitInDMs;
            _noLimitForAdmins = noLimitForAdmins;
            
            switch (measure)
            {
                case Measure.Days:
                    _invokeLimitPeriod = TimeSpan.FromDays(period);
                    break;
                case Measure.Hours:
                    _invokeLimitPeriod = TimeSpan.FromHours(period);
                    break;
                case Measure.Minutes:
                    _invokeLimitPeriod = TimeSpan.FromMinutes(period);
                    break;
            }
        }

        /// <summary> Sets how often a user is allowed to use this command. </summary>
        /// <param name="times">The number of times a user may use the command within a certain period.</param>
        /// <param name="period">The amount of time since first invoke a user has until the limit is lifted.</param>
        /// <param name="noLimitInDMs">Set whether or not there is no limit to the command in DMs. Defaults to false.</param>
        /// <param name="noLimitForAdmins">Set whether or not there is no limit to the command for server admins. Defaults to false.</param>
        public RatelimitAttribute(uint times, TimeSpan period, bool noLimitInDMs = false, bool noLimitForAdmins = false)
        {
            _invokeLimit = times;
            _noLimitInDMs = noLimitInDMs;
            _noLimitForAdmins = noLimitForAdmins;
            _invokeLimitPeriod = period;
        }

        /// <inheritdoc />
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (_noLimitInDMs && context.Channel is IPrivateChannel)
                return Task.FromResult(PreconditionResult.FromSuccess());

            IGuildUser gu = context.User as IGuildUser;
            if (_noLimitForAdmins && context.User is IGuildUser && gu.GuildPermissions.Administrator)
                return Task.FromResult(PreconditionResult.FromSuccess());

            var now = DateTime.UtcNow;

            CommandTimeout t;
            var timeout = (_invokeTracker.TryGetValue(context.User.Id, out t)
                && ((now - t.FirstInvoke) < _invokeLimitPeriod))
                    ? t : new CommandTimeout(now);

            timeout.TimesInvoked++;

            if (timeout.TimesInvoked <= _invokeLimit)
            {
                _invokeTracker[context.User.Id] = timeout;
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            else
            {
                return Task.FromResult(PreconditionResult.FromError("You are currently in Timeout."));
            }
        }

        private class CommandTimeout
        {
            public uint TimesInvoked { get; set; }
            public DateTime FirstInvoke { get; }

            public CommandTimeout(DateTime timeStarted)
            {
                FirstInvoke = timeStarted;
            }
        }
    }

    /// <summary> Sets the scale of the period parameter. </summary>
    public enum Measure
    {
        /// <summary> Period is measured in days. </summary>
        Days,

        /// <summary> Period is measured in hours. </summary>
        Hours,

        /// <summary> Period is measured in minutes. </summary>
        Minutes
    }
}