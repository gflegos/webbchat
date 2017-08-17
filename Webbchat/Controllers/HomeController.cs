using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webbchat.Models;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.Entity;
using System.Web.Helpers;
using JWT;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Webbchat.Controllers {
	public class HomeController : Controller {

		// GET: Home
		WebbchatDatabas db = new WebbchatDatabas(@"Data Source=.\SQLEXPRESS;Initial Catalog=WebbchatDB;Integrated Security=True");
		private static Random slump = new Random();
		public ActionResult Index() {
			return View();
		}

		protected string HämtaIPAdress() {
			System.Web.HttpContext context = System.Web.HttpContext.Current;
			string ipAdress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if(String.IsNullOrEmpty(ipAdress)) {
				ipAdress = context.Request.ServerVariables["REMOTE_ADDR"];
			}
			return ipAdress;
		}

		public static string SkapaSalt(int längd) {
			const string tecken = "0123456789ABCDEF";
			return new string(Enumerable.Repeat(tecken, längd)
				.Select(s => s[slump.Next(s.Length)]).ToArray());
		}

		public static string SkapaJwt(Dictionary<string, object> indata) {
			string nyckel = Crypto.Hash("Klezmer1991");
			string jwt = JsonWebToken.Encode(indata, nyckel, JwtHashAlgorithm.HS256);
			LäsJwt(jwt);
			return jwt;
		}

		public static Dictionary<string, object> LäsJwt(string jwt) {
			string nyckel = Crypto.Hash("Klezmer1991");
			Dictionary<string, object> utdata = JsonWebToken.DecodeToObject(jwt, nyckel) as Dictionary<string, object>;
			return utdata;
		}

		[HttpPost]
		public JsonResult Registrera(string användarnamn, string lösenord) {
			bool status = true;
			bool aktivitet = true;
			string salt = SkapaSalt(8);
			lösenord = salt + lösenord;
			lösenord = salt.Substring(0, 4) + Crypto.Hash(lösenord) + salt.Substring(4, 4);
			string ipAdress = HämtaIPAdress();
			Användare nyAnv = new Användare {
				namn = användarnamn,
				hashatLösenord = lösenord,
				ip = ipAdress,
				aktiv = aktivitet
			};
			try {
				db.Användare.InsertOnSubmit(nyAnv);
				db.SubmitChanges();
			}
			catch(Exception e) {
				status = false;
			}
			return Json(new { lyckades = status });
		}

		[HttpPost]
		public JsonResult LoggaIn(string användarnamn, string lösenord) {
			bool status = false;
			string jwt = "";
			var användarInfo = (from a in db.Användare
								where a.namn == användarnamn
								select new { id = a.id, hashatLösenord = a.hashatLösenord })
								.FirstOrDefault();
			int id = användarInfo.id;
			string lösenTemp = användarInfo.hashatLösenord;
			string dbSalt = lösenTemp.Substring(0, 4) + lösenTemp.Substring(lösenTemp.Length - 4, 4);
			lösenord = dbSalt.Substring(0, 4) + Crypto.Hash(dbSalt + lösenord) + dbSalt.Substring(4, 4);
			if(lösenord == lösenTemp) {
				status = true;
				jwt = SkapaJwt(new Dictionary<string, object>() {
					{ "id", id },
					{ "användarnamn", användarnamn },
					{ "exp", Math.Round((DateTime.UtcNow.AddDays(7)
						- new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds) }
				});
			}
			return Json(new {
				lyckades = status,
				token = jwt
			});
		}
		public JsonResult UppdateraKonto(string jwt) {
			bool status = true;
			int id = 0;
			string användarnamn = "";
			Dictionary<string, object> tokenData = new Dictionary<string, object>();
			try {
				tokenData = LäsJwt(jwt);
			}
			catch(Exception e) {
				status = false;
			}
			if(status) {
				id = Convert.ToInt32(tokenData["id"]);
				användarnamn = (string)tokenData["användarnamn"];
			}
			return Json(new {
				lyckades = status,
				id = id,
				användarnamn = användarnamn
			});
		}

		[HttpPost]
		public JsonResult SkickaMeddelande(string innehåll, string token) {
			bool status = true;
			if(string.IsNullOrEmpty(token)) {
				status = false;
			}
			else {
				int id = Convert.ToInt32(LäsJwt(token)["id"]);
				Meddelande nyttMeddelande = new Meddelande {
					användarId = id,
					innehåll = innehåll,
					tid = DateTime.UtcNow
				};
				try {
					db.Meddelanden.InsertOnSubmit(nyttMeddelande);
					db.SubmitChanges();
				}
				catch(Exception e) {
					status = false;
				}
			}
			return Json(new { lyckades = status });
		}

		[HttpPost]
		public JsonResult HämtaMeddelanden(string token, int meddelandeId = 0) {
			int läsareId;
			if(string.IsNullOrEmpty(token) == false) {
				Dictionary<string, object> tokenInfo = LäsJwt(token);
				läsareId = Convert.ToInt32(tokenInfo["id"]);
			}
			else {
				läsareId = 0;
			}
			var meddelanden = (from meddelande in db.Meddelanden
							   join användare in db.Användare on meddelande.användarId equals användare.id
							   where meddelande.id > meddelandeId && (användare.aktiv || användare.id == läsareId)
							   orderby meddelande.id descending
							   select new {
								   id = meddelande.id, text = meddelande.innehåll,
								   tid = (meddelande.tid - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds,
								   användarnamn = användare.namn
							   }).Take(100).ToArray();
			return Json(meddelanden);
		}
	}
}