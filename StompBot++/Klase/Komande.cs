using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;

namespace StompBot
{
    public class Komande
    {
		[Command("obrisi")]
		public async Task Obrisi(CommandContext ctx)
		{
			ulong zadnjiId = ctx.Channel.LastMessageId;
			var poruke = await ctx.Channel.GetMessagesAsync(before: zadnjiId);

			var zadnjaPoruka = await ctx.Channel.GetMessageAsync(ctx.Channel.LastMessageId);
			await zadnjaPoruka.DeleteAsync();

			foreach(var poruka in poruke)
			{
				if(poruka.Author.IsCurrent || poruka.ToString().Contains("Contents: ."))
				{
					await poruka.DeleteAsync();
				}
			}
		}

		[Command("ugasi")]
		public async Task Ugasi(CommandContext ctx)
		{
			await Audio.Stop();
			await BotAudioComponent.Ugasi();
		}
		
		[Command("ide")]
		public async Task Ide(CommandContext ctx, [RemainingText] string ime)
		{
			await Audio.IspisiPlaylistu(Audio.ctx, ime, ctx);
		}

		[Command("moze")]
		public async Task Moze(CommandContext ctx, int redni_broj)
		{
			if (Audio.PlayaTrenutno == true)
			{
				Audio.cts.Cancel();

				Thread.Sleep(500);

				Audio.cts = new CancellationTokenSource();
			}

			await Audio.PokreniPlaylistu(Audio.ctx, redni_broj, Audio.cts.Token);
		}

		[Command("next")]
		public async Task Next(CommandContext ctx)
		{
			await Audio.Next(ctx);
		}

		[Command("pause")]
		public async Task Pause(CommandContext ctx)
		{
			await Audio.Pause(ctx);
		}

		[Command("stop")]
		public async Task Stop(CommandContext ctx)
		{
			await Audio.Stop();
		}

		[Command("join")]
		public async Task Join(CommandContext ctx)
		{
				var vnext = ctx.Client.GetVoiceNextClient();

				var vnc = vnext.GetConnection(ctx.Guild);

				if (vnc == null)
				{
					var chn = ctx.Member?.VoiceState?.Channel;

					if (chn != null)
					{
						vnc = await vnext.ConnectAsync(chn);

						Audio.mojVnc = vnc;
					}
				}

				Audio.ctx = ctx;	
		}
		
        [Command("leave")]
        public async Task Leave(CommandContext ctx)
        {
				var vnext = ctx.Client.GetVoiceNextClient();
				if (vnext == null)
				{
					return;
				}

				var vnc = vnext.GetConnection(ctx.Guild);
				if (vnc == null)
				{
					return;
				}

				vnc.Disconnect();
        }
    }
}
