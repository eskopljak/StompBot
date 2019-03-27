using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;

namespace StompBot
{
    public static class BotAudioComponent
    {
		const string _token = "";

		public static DiscordClient discord;
		public static VoiceNextClient voice;
		static CommandsNextModule commands;
		
		public static async Task Ugasi()
		{
			await discord.DisconnectAsync();
		}

		public static async Task MainAsync(string[] args)
		{
			discord = new DiscordClient(new DiscordConfiguration
			{
				Token = _token,
				TokenType = TokenType.Bot,
				UseInternalLogHandler = true,
				LogLevel = LogLevel.Debug
			});
			
			commands = discord.UseCommandsNext(new CommandsNextConfiguration
			{
				StringPrefix = ".",
				EnableDms = true
			});

			voice = discord.UseVoiceNext();

			commands.RegisterCommands<Komande>();

			RespondNaPoruke();

			await discord.ConnectAsync();

			await Task.Delay(-1);
		}

		static void RespondNaPoruke()
		{
			discord.MessageCreated += async e =>
			{
				if (e.Message.Content.ToLower().StartsWith("izvjestaj "))
				{
					string skracenica = e.Message.Content.Substring(10).ToLower().Trim();

					string link = Baza.LinkIzvjestajaZaPredmet(skracenica);

					if (string.IsNullOrEmpty(link) == false)
					{
						await e.Message.RespondAsync(link);
					}
				}
			};
		}
	}
}
