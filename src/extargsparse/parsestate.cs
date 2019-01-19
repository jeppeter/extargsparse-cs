using System;
using System.Collections.Generic;

namespace extargsparse
{
	public class _ParserState : _LogObject
	{
		private List<_ParserCompact> m_cmdpaths;
		private int m_curidx;
		private int m_curcharidx;
		private int m_shortcharargs;
		private int m_longargs;
		private int m_keyidx;
		private int m_validx;
		private string[] m_args;
		private int m_ended;
		private string m_longprefix;
		private string m_shortprefix;
		private bool m_parseall;
		private List<string> m_leftargs;
		private bool m_bundlemode;
		public _ParserState(string[] args,_ParserCompact maincmd,ExtArgsOptions optattr=null) : base()
		{
			if (optattr == null) {
				optattr = new ExtArgsOptions("{}");
			}
			this.m_cmdpaths = new List<_ParserCompact>();
			this.m_cmdpaths.Add(maincmd);
			this.m_curidx = 0;
			this.m_curcharidx = -1;
			this.m_shortcharargs = -1;
			this.m_longargs = -1;
			this.m_keyidx = -1;
			this.m_validx = -1;
			this.m_args = args;
			this.m_ended = 0;
			this.m_longprefix = optattr.get_string("longprefix");
			this.m_shortprefix = optattr.get_string("shortprefix");
			if (this.m_shortprefix == "" || this.m_longprefix == "" ||
				this.m_longprefix != this.m_shortprefix) {
				this.m_bundlemode = true;
			} else {
				this.m_bundlemode = false;
			}
			this.m_parseall = optattr.get_bool("parseall");
			this.m_leftargs = new List<string>();
		}

		public string format_cmdname_path(List<_ParserCompact> curparser=null) {
			string cmdname = "";
			if (curparser == null) {
				curparser = this.m_cmdpaths;
			}
			foreach( var c in curparser) {
				if (cmdname.Length > 0) {
					cmdname += ".";
				}
				cmdname += c.cmdname;
			}
			return cmdname;
		}

		private KeyCls __find_sub_command(string name) {
			_ParserCompact cmdparent = null;
			cmdparent = this.m_cmdpaths[(this.m_cmdpaths.Count - 1)];
			if (cmdparent != null) {
				foreach(var cmd in cmdparent.subcommands) {
					if (cmd.cmdname == name) {
						this.m_cmdpaths.Add(cmd);
						return cmd.keycls;
					}
				}
			}
			return null;
		}
	}
}