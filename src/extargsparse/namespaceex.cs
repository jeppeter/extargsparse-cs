using System;
using System.Collections.Generic;


namespace extargsparse
{
	public class NameSpaceEx
	{
		private Dictionary<string,object> m_dict;
		private Dictionary<string,bool> m_access;
		private _LogObject m_logger;
		public NameSpaceEx()
		{
			this.m_logger = new _LogObject();
			this.m_dict = new Dictionary<string,object>();
			this.m_access = new Dictionary<string,bool>();
		}

		public void set_value(string k, object v)
		{
			string vtype = v.GetType().FullName;
			this.m_access[k] = true;
			if (vtype == "System.Int32" || 
				vtype == "System.Int64")  {
				this.m_dict[k] =  (System.Int64)  v;
			}

			this.m_dict[k] = v;
			return;
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

		public bool is_accessed(string k)
		{
			if (this.m_access.ContainsKey(k)) {
				return true;
			}
			return false;
		}

		public string[] get_keys()
		{
			List<string> c  = new List<string>();
			foreach( var k in this.m_dict.Keys) {
				c.Add(k);
			}
			return c.ToArray();
		}

		public override string ToString()
		{
			string s="{";
			foreach(string k in this.get_keys())  {
				s += String.Format("{0}={1};",  k, this.m_dict[k]);
			}
			s += "}";
			return s;
		}
	}
}