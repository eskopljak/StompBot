using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;

namespace StompBot
{
    class Program
    {
		static void Main(string[] args)
		{
			BotAudioComponent.MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
		}
	}
}
