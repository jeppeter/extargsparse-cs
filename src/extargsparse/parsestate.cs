using System;
using System.Collections.Generic;

namespace extargsparse
{
	public class _ParserState : _LogObject
	{
		private List<_ParserCompact> m_cmdpaths;
		public _ParserState(string[] args,_ParserCompact maincmd,ExtArgsOptions optattr=null) : base()
		{
			if (optattr == null) {
				optattr = new ExtArgsOptions("{}");
			}
			this.m_cmdpaths = new List<_ParserCompact>();
			this.m_cmdpaths.Add(maincmd);
		}
	}
}