using System;


namespace extargsparse
{
	public class _HelpSize : _LogObject
	{
		private int m_optnamesize ;
		private int m_optexprsize ;
		private int m_opthelpsize ;
		private int m_cmdnamesize ;
		private int m_cmdhelpsize ;

		public _HelpSize() : base()
		{
			this.m_optnamesize = 0;
			this.m_optexprsize = 0;
			this.m_opthelpsize = 0;
			this.m_cmdnamesize = 0;
			this.m_cmdhelpsize = 0;
		}

		public int optnamesize 
		{
			get {
				return this.m_optnamesize;
			}
			set {
				if (this.m_optnamesize < value) {
					this.m_optnamesize = value;
				}
			}
		}


		public int optexprsize
		{
			get {
				return this.m_optexprsize;
			}
			set {
				if (this.m_optexprsize < value) {
					this.m_optexprsize = value;
				}
			}
		}

		public int opthelpsize
		{
			get {
				return this.m_opthelpsize;
			}
			set {
				if (this.m_opthelpsize < value) {
					this.m_opthelpsize = value;
				}
			}
		}

		public int cmdnamesize 
		{
			get {
				return this.m_cmdnamesize;
			}
			set {
				if (this.m_cmdnamesize < value) {
					this.m_cmdnamesize = value;
				}
			}
		}

		public int cmdhelpsize
		{
			get {
				return this.m_cmdhelpsize;
			}
			set {
				if (this.m_cmdhelpsize < value) {
					this.m_cmdhelpsize = value;
				}
			}
		}

		public string format()
		{
			string s = "";
			s += String.Format("optnamesize={0};", this.m_optnamesize);
			s += String.Format("optexprsize={0};", this.m_optexprsize);
			s += String.Format("opthelpsize={0};", this.m_opthelpsize);
			s += String.Format("cmdnamesize={0};", this.m_cmdnamesize);
			s += String.Format("cmdhelpsize={0};", this.m_cmdhelpsize);
			return s;
		}
	}
}