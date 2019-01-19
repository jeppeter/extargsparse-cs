using System;
using System.Collections.Generic;
using System.Diagnostics;

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

		public void add_parse_args(int nargs) {
			if (this.m_curidx >= 0) {
				if (nargs > 0 && this.m_shortcharargs >0) {
					this.throw_exception(String.Format("{0} already set args", this.m_args[this.m_curidx]));
				}
				if (this.m_shortcharargs < 0) {
					this.m_shortcharargs = 0;
				}
				this.m_shortcharargs += nargs;
			} else {
				if (this.m_longargs > 0) {
					this.throw_exception(String.Format("{0} not handled", this.m_args[this.m_curidx]));
				}
				if (this.m_longargs < 0) {
					this.m_longargs = 0;
				}
				this.m_longargs += nargs;
				this.Info(String.Format("longargs [{0}] nargs [{1}]", this.m_longargs, nargs));
			}
			return;
		}

		private KeyCls __find_key_cls(){
			int oldcharidx ;
			int oldidx;
			char curch;
			int idx;
			if (this.m_ended > 0) {
				return null;
			}
			if (this.m_longargs > 0) {
				System.Diagnostics.Debug.Assert(this.m_curcharidx < 0);
				this.m_curidx += this.m_longargs;
				System.Diagnostics.Debug.Assert(this.m_args.Length >= this.m_curidx);
				this.m_longargs = -1;
				this.m_validx = -1;
				this.m_keyidx = -1;
			}

			oldcharidx = this.m_curcharidx;
			oldidx = this.m_curidx;
			if (oldidx >= this.m_args.Length) {
				this.m_curidx = oldidx;
				this.m_curcharidx = -1;
				this.m_shortcharargs = -1;
				this.m_longargs = -1;
				this.m_keyidx = -1;
				this.m_validx = -1;
				this.m_ended = 1;
				return null;
			}
			if (oldcharidx >= 0) {
				string c;
				c = this.m_args[oldidx];
				if (c.Length <= oldcharidx) {
					oldidx += 1;
					this.Info(String.Format("oldidx [{0}]", oldidx));
					if (this.m_shortcharargs > 0) {
						oldidx += this.m_shortcharargs;
					}
					this.Info(String.Format("oldidx [%s] __shortcharargs [%d]",oldidx,this.m_shortcharargs));
					this.m_curidx = oldidx;
					this.m_curcharidx = -1;
					this.m_shortcharargs = -1;
					this.m_keyidx = -1;
					this.m_validx = -1;
					this.m_longargs = -1;
					return this.__find_key_cls();
				} 
				curch = c[oldcharidx];
				this.Info(String.Format("argv[{0}][{1}] {2}", oldidx, oldcharidx, curch));
				idx = this.m_cmdpaths.Count -1;
				while( idx >= 0) {
					
				}
			} else {

			}
			return null;
		}

	}
}