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
    public static class Audio
    {
        public static VoiceNextConnection mojVnc;

		public const string _playlist_folder = "playliste\\";

		public static string[] playlista;
		static int trenutna;
		public static bool PlayaTrenutno;
		public static bool Pauzirano;

		public static CancellationTokenSource cts;
		public static CommandContext ctx = null;

		static Audio()
		{
			cts = new CancellationTokenSource();

			PlayaTrenutno = false;
			Pauzirano = false;
		}

		public static async Task IspisiPlaylistu(CommandContext ctx, string ime, CommandContext kontekstZaIspisPoruke)
		{
			string ime_direktorija = _playlist_folder + ime.Trim();

			if (Directory.Exists(ime_direktorija))
			{
				if(PlayaTrenutno == true)
				{
					cts.Cancel();

					Thread.Sleep(500);

					cts = new CancellationTokenSource();
				}

				string[] fajlovi = Directory.GetFiles(ime_direktorija);

				playlista = Directory.GetFiles(ime_direktorija);

				for(int i = 0; i < fajlovi.Length; i++)
				{
					fajlovi[i] = Path.GetFileNameWithoutExtension(fajlovi[i]);
				}

				int redni_broj = 1;

				string ispis = "";
				
				foreach(string s in fajlovi)
				{
					ispis += $"{redni_broj}. {s}\n";

					redni_broj++;
				}
                
				if(ispis.Length != 0)
				{
					await kontekstZaIspisPoruke.RespondAsync(ispis);

					await kontekstZaIspisPoruke.RespondAsync("👌");
				}
			}
		}

		public static async Task PokreniPlaylistu(CommandContext ctx, int redni_broj, CancellationToken token)
		{
			var vnext = ctx.Client.GetVoiceNextClient();

			var vnc = vnext.GetConnection(ctx.Guild);

            vnc = mojVnc;

            if (vnc != null && playlista != null && redni_broj >= 1 && redni_broj <= playlista.Length)
            {
                trenutna = redni_broj - 1;

                PlayaTrenutno = true;
                Pauzirano = false;

                await vnc.SendSpeakingAsync(true);

                while (true)
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $@"-i ""{playlista[trenutna]}"" -ac 2 -f s16le -ar 48000 pipe:1",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };

                    Process ffmpeg = Process.Start(psi);
                    Stream ffout = ffmpeg.StandardOutput.BaseStream;

                    byte[] buff = new byte[3840];
                    int br = 0;

                    while ((br = ffout.Read(buff, 0, buff.Length)) > 0)
                    {
                        if (token.IsCancellationRequested == true)
                        {
                            ffout.Close();
                            ffmpeg.Close();

                            cts.Dispose();

                            trenutna = 0;

                            await vnc.SendSpeakingAsync(false);

                            PlayaTrenutno = false;
                            Pauzirano = false;

                            return;
                        }

                        await vnc.SendAsync(buff, 20);

                        while (Pauzirano == true)
                        {
                            if (token.IsCancellationRequested == true)
                            {
                                ffout.Close();
                                ffmpeg.Close();

                                cts.Dispose();

                                trenutna = 0;

                                await vnc.SendSpeakingAsync(false);

                                PlayaTrenutno = false;
                                Pauzirano = false;

                                return;
                            }

                            Thread.Sleep(500);
                        }
                    }

					if(playlista.Length > 1)
					{
						Random rand = new Random();
						var stara = trenutna;
						do
						{
							trenutna = (stara + rand.Next(playlista.Length)) % playlista.Length;
						} while (stara == trenutna);
					}
					else
					{
						trenutna = 0;
					}
				}
            }
		}

		public static async Task Next(CommandContext ctx)
		{
			var zadnja = trenutna;

			cts.Cancel();

			while (PlayaTrenutno) ;

			if (playlista.Length > 1)
			{
				Random rand = new Random();
				var stara = zadnja;
				do
				{
					zadnja = (stara + rand.Next(playlista.Length)) % playlista.Length;
				} while (stara == zadnja);
			}
			else
			{
				zadnja = 0;
			}

			trenutna = zadnja;

			cts = new CancellationTokenSource();

			await PokreniPlaylistu(ctx, trenutna + 1, cts.Token);
		}

		public static async Task Pause(CommandContext ctx)
		{
			await Task.Run(new Action(() =>
			{
				if (Pauzirano == false) Pauzirano = true;
				else Pauzirano = false;

                var vnext = ctx.Client.GetVoiceNextClient();

                var vnc = vnext.GetConnection(ctx.Guild);

                vnc.SendSpeakingAsync(false);
			}));
		}

		public static async Task Stop()
		{
			await Task.Run(new Action(() =>
			{
				if (PlayaTrenutno == true)
				{
					cts.Cancel();

					Thread.Sleep(500);

					cts = new CancellationTokenSource();
				}
			}));
		}
    }
}
