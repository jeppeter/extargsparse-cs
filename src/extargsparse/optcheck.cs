using System;
using System.Collections.Generic;

namespace extargsparse
{
	public class _OptCheck
	{
		private List<string> m_longopt;
		private List<string> m_shortopt;
		private List<string> m_varname;
		private void __reset()
		{
			this.m_longopt = new List<string>();
			this.m_shortopt = new List<string>();
			this.m_varname  = new List<string>();
			return;
		}

		public _OptCheck()
		{
			this.__reset();
		}

		public void copy(_OptCheck  other)
		{
			int i;
			this.__reset();
			for (i=0 ;i < other.m_longopt.Count ;i ++) {
				this.m_longopt.Add(other.m_longopt[i]);
			}
			for (i=0 ; i < other.m_shortopt.Count ;i++) {
				this.m_shortopt.Add(other.m_shortopt[i]);
			}
			for (i=0 ; i < other.m_varname.Count ;i ++) {
				this.m_varname.Add(other.m_varname[i]);
			}
			return;
		}

		public bool add_and_check(string typename,string v)
		{
			if (typename == "longopt") {
				if (!this.m_longopt.Contains(v)) {
					this.m_longopt.Add(v);				
					return true;
				}
			} else if (typename == "shortopt") {
				if (!this.m_shortopt.Contains(v)) {
					this.m_shortopt.Add(v);
					return true;
				}				
			} else if (typename == "varname") {
				if (!this.m_varname.Contains(v)) {
					this.m_varname.Add(v);
					return true;
				}
				
			} 
			return false;
		}
	}
}