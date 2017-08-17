using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace Webbchat.Models {
	public class WebbchatDatabas:DataContext {
		public Table<Användare> Användare;
		public Table<Meddelande> Meddelanden;
		public WebbchatDatabas(string connection):base(connection) {

		}
	}
}