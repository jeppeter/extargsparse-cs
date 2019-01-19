using System;

namespace extargsparse
{
	public class _ParseState : _LogObject
	{
		public _ParseState(string[] args,_ParserCompact maincmd,ExtArgsOptions optattr=null) : base()
		{
			if (optattr == null) {
				optattr = new ExtArgsOptions("{}");
			}
		}
	}
}