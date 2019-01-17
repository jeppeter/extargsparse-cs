using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace extargsparse
{
	public class ExtArgsOptions : _LogObject
	{
		private static Dictionary<string,object>  m_defdict = new Dictionary<string,object>{
			{"prog", Environment.GetCommandLineArgs().Length > 0 ? Environment.GetCommandLineArgs()[0] : ""},
			{"usage", ""},
			{"description", ""},
			{"epilog", ""},
			{"version","0.0.1"},
			{"errorhandler" , "exit"},
			{"helphandler", null},
			{"longprefix", "--"},
			{"shortprefix", "-"},
			{"nohelpoption", false},
			{"nojsonoption", false},
			{"helplong", "help"},
			{"helpshort", "h"},
			{"jsonlong", "json"},
			{"cmdprefixadded", true},
			{"parseall", true},
			{"screenwidth", 80},
			{"flagnochange", false}
		};

		private Dictionary<string,object> m_dict;

		private object _get_token(string key,JToken tokv)
		{
			string valtype;
			JValue jval;
			object v;
			valtype = tokv.GetType().FullName;
			if (valtype == "Newtonsoft.Json.Linq.JValue") {
				jval = (JValue) tokv;
				switch (jval.Type) {
					case JTokenType.Integer:
						v = (object)((System.Int64) jval.Value);
						break;
					case JTokenType.Float:
						v = (object)((System.Double) jval.Value);
						break;
					case JTokenType.String:
						v = (object)((System.String) jval.Value);
						break;
					case JTokenType.Null:
						v = (object)null;
						break;
					case JTokenType.Boolean:
						v = (object)((System.Boolean) jval.Value);
						break;
					default:
						throw new Exception(String.Format("value type [{0}] not supported", jval.Type));
				}
			} else if (valtype == "Newtonsoft.Json.Linq.JArray") {
				v = (object) this._get_array(key, tokv.Value<JArray>());
			} else if (valtype == "Newtonsoft.Json.Linq.JObject") {
				v = (object) this._get_object(key, tokv.Value<JObject>());
			} else {
				throw new Exception(String.Format("not supported type [{0}]", valtype));
			}
			return v;
		}

		private object[] _get_array(string key,JArray jarr)
		{
			object[] retobj;
			int i;
			JToken tok;

			retobj = new object[jarr.Count];
			for (i=0; i < jarr.Count ;i ++) {
				tok = jarr[i];
				retobj[i] = this._get_token(String.Format("{0}[{1}]", key, i), tok);
			}
			return retobj;
		}

		private Dictionary<string, object> _get_object(string key,JObject jobj)
		{
			Dictionary<string,object> retdict = new Dictionary<string,object>();
			Dictionary<string,JToken> nobj;
			List<String> keys;
			string k;
			object v;
			int i;
			nobj = jobj.ToObject<Dictionary<string,JToken>>();
			keys = new List<String>(nobj.Keys);
			for (i=0; i < keys.Count; i++) {
				k = keys[i];
				v = this._get_token(k, nobj[k]);
				retdict[k] = v;
			}
			return retdict;
		}

		private Dictionary<string,object> _parse_json(string s)
		{
			JToken tok = JToken.Parse(s);
			Dictionary<string,JToken> dtok;
			Dictionary<string, object> retdict = new Dictionary<string,object>();
			List<string> keys;
			string k;
			object v;
			JToken tokv;
			JValue jval;
			string valtype;
			int i;
			dtok = tok.ToObject<Dictionary<string, JToken>>();
			keys = new List<string>(dtok.Keys);
			for (i=0; i < keys.Count; i++) {
				k = keys[i];
				tokv = dtok[k];

				valtype = tokv.GetType().FullName;
				if (valtype == "Newtonsoft.Json.Linq.JValue") {
					jval = (JValue) tokv;
					switch (jval.Type) {
						case JTokenType.Integer:
							v = (object)((System.Int64) jval.Value);
							break;
						case JTokenType.Float:
							v = (object)((System.Double) jval.Value);
							break;
						case JTokenType.String:
							v = (object)((System.String) jval.Value);
							break;
						case JTokenType.Null:
							v = (object)null;
							break;
						case JTokenType.Boolean:
							v = (object)((System.Boolean) jval.Value);
							break;
						default:
							throw new Exception(String.Format("value type [{0}] not supported", jval.Type));
					}
				} else if (valtype == "Newtonsoft.Json.Linq.JArray") {
					v = _get_array(k, tokv.Value<JArray>());
				} else if (valtype == "Newtonsoft.Json.Linq.JObject") {
					v = _get_object(k, tokv.Value<JObject>());
				} else {
					throw new Exception(String.Format("not supported type [{0}]", valtype));
				}
				retdict[k] = v;
			}
			return retdict;
		}

		private void _set_dict(Dictionary<string,object> setting)
		{
			foreach (KeyValuePair<string,object> kv in setting) {
				this.m_dict[kv.Key] = kv.Value;
			}
			return;			
		}

		public ExtArgsOptions(string setting) : base()
		{
			Dictionary<string,object> s;
			this.m_dict = new Dictionary<string,object>();
			s = this._parse_json(setting);
			this._set_dict(ExtArgsOptions.m_defdict);
			this._set_dict(s);
		}

		public ExtArgsOptions(Dictionary<string,object> setting) : base()
		{
			this.m_dict = new Dictionary<string,object>();
			this._set_dict(ExtArgsOptions.m_defdict);
			this._set_dict(setting);
		}

		public Double get_float(string k)
		{
			object obj=null;
			Double dval=0.0;
			if (this.m_dict.ContainsKey(k)) {
				obj = this.m_dict[k];
				if (obj != null) {
					if (obj.GetType().FullName == "System.Double") {
						dval = (Double) obj;
					}
				}
			}
			return dval;
		}

		public string get_string(string k)
		{
			string s = "";
			object obj = null;
			if (this.m_dict.ContainsKey(k)) {
				obj = this.m_dict[k];
				if (obj != null) {
					if (obj.GetType().FullName == "System.String") {
						s = (string) obj;
					}
				}
			}
			return s;
		}

		public System.Int64 get_int(string k)
		{
			System.Int64 ival = 0;
			object obj = null;
			if (this.m_dict.ContainsKey(k)) {
				obj = this.m_dict[k];
				if (obj != null) {
					if (obj.GetType().FullName == "System.Int64") {
						ival = (System.Int64) obj;
					}
				}
			}
			return ival;
		}

		public bool get_bool(string k)
		{
			bool bval = false;
			object obj = null;
			if (this.m_dict.ContainsKey(k)) {
				obj = this.m_dict[k];
				if (obj != null) {
					if (obj.GetType().FullName == "System.Boolean") {
						bval = (System.Boolean) obj;
					}
				}
			}
			return bval;
		}

	}
}