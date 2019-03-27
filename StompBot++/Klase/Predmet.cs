using System;
using System.Collections.Generic;
using System.Text;

namespace StompBot
{
    class Predmet
    {
		string ime, skracenica;
		int id;

		public Predmet(string ime, string skracenica, int id)
		{
			this.ime = ime;
			this.skracenica = skracenica;
			this.id = id;
		}

		public string Ime { get => ime; }
		public string Skracenica { get => skracenica; }
		public int ID { get => id; }

		public string LinkIzvjestaja { get => $"https://zamger.etf.unsa.ba/?sta=izvjestaj/predmet&predmet={id}&ag={Baza.TrenutnaAkademskaGodina}"; }
    }
}
