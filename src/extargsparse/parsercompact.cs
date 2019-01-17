using System;
using System.Diagnostics;

namespace extargsparse
{
	public class _ParserCompact : _LogObject
	{
		private KeyCls m_keycls;
		private string m_cmdname;

		public _ParserCompact(KeyCls kcls=null, ExtArgsOptions opt=null) : base()
		{
			if (kcls != null) {
				Debug.Assert(kcls.iscmd);
				this.m_keycls = kcls;
				this.m_cmdname = kcls.cmdname;
			} else {
				this.m_keycls = null;
			}
		}
	}
}