using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Threading.Tasks;

namespace Webbchat.Models {
	[Table(Name = "Användare")]
	public class Användare {
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
		private string _namn;
		[Column(Storage = "_namn", DbType = "NVARCHAR(32) NOT NULL UNIQUE")]
		public string namn {
			get {
				return this._namn;
			}
			set {
				this._namn = value;
			}
		}
		private string _hashatLösenord;
		[Column(Storage = "_hashatLösenord", DbType = "NVARCHAR(72) NOT NULL")]
		public string hashatLösenord {
			get {
				return this._hashatLösenord;
			}
			set {
				this._hashatLösenord = value;
			}
		}
		private bool _aktiv;
		[Column(Storage = "_aktiv", DbType = "BIT NOT NULL")]
		public bool aktiv {
			get {
				return this._aktiv;
			}
			set {
				this._aktiv = value;
			}
		}
		private string _ip;
		[Column(Storage = "_ip", DbType = "NVARCHAR(64) NULL UNIQUE")]
		public string ip {
			get {
				return this._ip;
			}
			set {
				this._ip = value;
			}
		}
	}
}