using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace StompBot
{
    static class Baza
    {
		const string _datotekaPredmeta = "predmeti.txt";
		const string _datotekaInfo = "info.txt";

		static int _trenutnaAkademskaGodina;

		static List<Predmet> predmeti;

		static Baza()
		{
			predmeti = new List<Predmet>();

			// ucitavanje iz datoteke info
			try
			{
				FileStream dat = new FileStream(_datotekaInfo, FileMode.Open);

				StreamReader dat_rd = new StreamReader(dat);

				dat_rd.ReadLine();

				_trenutnaAkademskaGodina = Int32.Parse(dat_rd.ReadLine().Trim());

				dat_rd.Close();
				dat.Close();

				// ucitavanje iz datoteke registrovanih predmeta

				dat = new FileStream(_datotekaPredmeta, FileMode.Open);

				dat_rd = new StreamReader(dat);

				while (dat_rd.EndOfStream == false)
				{
					string ime = dat_rd.ReadLine().Trim();
					string skracenica = dat_rd.ReadLine().Trim();
					int id = Int32.Parse(dat_rd.ReadLine().Trim());
					dat_rd.ReadLine();

					predmeti.Add(new Predmet(ime, skracenica, id));
				}

				dat_rd.Close();
				dat.Close();
			}
			catch
			{
				//
			}
		}

		public static List<Predmet> Predmeti { get => predmeti; }

		public static int TrenutnaAkademskaGodina { get => _trenutnaAkademskaGodina; }

		public static string LinkIzvjestajaZaPredmet(string skracenica)
		{
			foreach(Predmet p in predmeti)
			{
				if(p.Skracenica == skracenica)
				{
					return p.LinkIzvjestaja;
				}
			}
			return "";
		}
	}
}
