using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Threading.Tasks;

namespace Webbchat.Models {
	[Table(Name = "Meddelanden")]
	public class Meddelande {
		private int _id;
		[Column(Storage = "_id", DbType = "INT NOT NULL IDENTITY",
			IsPrimaryKey = true, IsDbGenerated = true)]
		public int id {
			get {
				return this._id;
			}
			set {
				this._id = value;
			}
		}
		private int _användarId;
		[Column(Storage = "_användarId", DbType = "INT NOT NULL")]
		public int användarId {
			get {
				return this._användarId;
			}
			set {
				this._användarId = value;
			}
		}
		private string _innehåll;
		[Column(Storage = "_innehåll", DbType = "TEXT")]
		public string innehåll {
			get {
				return this._innehåll;
			}
			set {
				this._innehåll = value;
			}
		}
		private DateTime _tid;
		[Column(Storage = "_tid", DbType = "DATETIME NOT NULL")]
		public DateTime tid {
			get {
				return this._tid;
			}
			set {
				this._tid = value;
			}
		}
	}
}