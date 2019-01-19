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
			_ParserCompact  cmd;
			string curarg;
			KeyCls keycls;
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
					cmd = this.m_cmdpaths[idx];
					foreach( var opt in cmd.cmdopts)  {
						if (! opt.isflag) {
							continue;
						}
						if (opt.flagname == "$") {
							continue;
						}

						if (opt.shortflag != "") {
							if (opt.shortflag[0] == curch) {
								this.m_keyidx = oldidx;
								this.m_validx = (oldidx + 1);
								this.m_curidx = oldidx;
								this.m_curcharidx = (oldcharidx + 1);
								this.Info(String.Format("{0} validx [{1}]", opt, this.m_validx));
								return opt;
							}
						}
					}
					idx --;
				}
				this.throw_exception(String.Format("can not parse [{0}]", this.m_args[oldidx]));
			} else {
				if (this.m_bundlemode) {
					curarg = this.m_args[oldidx];
					if (curarg.StartsWith(this.m_longprefix)) {
						if (curarg == this.m_longprefix) {
							this.m_keyidx = -1;
							this.m_validx = oldidx + 1;
							this.m_curcharidx = -1;
							this.m_validx = (oldidx + 1);
							this.m_shortcharargs = -1;
							this.m_longargs = -1;
							this.m_ended = 1;
							if (this.m_args.Length  > this.m_curidx) {
								for(idx =this.m_curidx ; idx < this.m_args.Length ;idx ++) {
									this.m_leftargs.Add(this.m_args[idx]);
								}
							}
							return null;
						}
						idx = this.m_cmdpaths.Count - 1;
						while ( idx >= 0) {
							cmd = this.m_cmdpaths[idx];
							foreach(var opt in cmd.cmdopts) {
								if (!opt.isflag) {
									continue;
								}
								if (opt.flagname == "$") {
									continue;
								}
								this.Info(String.Format("[{0}]longopt {1} curarg {2}", idx, opt.longopt, curarg));
								if (opt.longopt == curarg) {
									this.m_keyidx = oldidx;
									oldidx += 1;
									this.m_validx  = oldidx;
									this.m_shortcharargs = -1;
									this.m_longargs = -1;
									this.Info(String.Format("oldidx {0} (len {1})", oldidx, this.m_args.Length));
									this.m_curidx = oldidx;
									this.m_curcharidx = -1;
									return opt;
								}
							}
							idx --;
						}
						this.throw_exception(String.Format("can not parse ({0})", this.m_args[oldidx]));
					} else if (curarg.StartsWith(this.m_shortprefix)) {
						if (curarg == this.m_shortprefix) {
							if (this.m_parseall) {
								this.m_leftargs.Add(curarg);
								oldidx += 1;
								this.m_curidx = oldidx;
								this.m_curcharidx = -1;
								this.m_longargs = -1;
								this.m_shortcharargs = -1;
								this.m_keyidx = -1;
								this.m_validx = -1;
								return this.__find_key_cls();
							} else {
								this.m_ended = 1;
								for (idx = oldidx ;idx < this.m_args.Length ;idx ++) {
									this.m_leftargs.Add(this.m_args[idx]);
								}
								this.m_validx = oldidx;
								this.m_keyidx = -1;
								this.m_curidx = oldidx;
								this.m_curcharidx = -1;
								this.m_shortcharargs = -1;
								this.m_longargs = -1;
								return null;
							}
						}
						oldcharidx = this.m_shortprefix.Length;
						this.m_curidx = oldidx;
						this.m_curcharidx = oldcharidx;
						return this.__find_key_cls();
					} 
				} else {
					idx = this.m_cmdpaths.Count - 1;
					curarg = this.m_args[oldidx];
					while(idx >= 0) {
						cmd = this.m_cmdpaths[idx];
						foreach( var opt in cmd.cmdopts) {
							if (! opt.isflag) {
								continue;
							}
							if (opt.flagname == "$") {
								continue;
							}
							this.Info(String.Format("[{0}]({1}) curarg {2}", idx, opt.longopt, curarg));
							if (opt.longopt == curarg) {
								this.m_keyidx = oldidx;
								this.m_validx = (oldidx + 1);
								this.m_shortcharargs = -1;
								this.m_longargs = -1;
								this.Info(String.Format("oldidx {0} (len {1})", oldidx,this.m_args.Length));
								this.m_curidx = (oldidx + 1);
								this.m_curcharidx = -1;
								return opt;
							}
						}
						idx --;
					}
					idx = this.m_cmdpaths.Count -1;
					while(idx >= 0) {
						cmd = this.m_cmdpaths[idx];
						foreach(var opt in cmd.cmdopts) {
							if (! opt.isflag) {
								continue;
							}
							if (opt.flagname == "$") {
								continue;
							}
							this.Info(String.Format("[{0}]({1}) curarg [{2}]", idx, opt.shortopt, curarg));
							if (opt.shortopt != "" && opt.shortopt == curarg) {
								this.m_keyidx = oldidx;
								this.m_validx = (oldidx + 1);
								this.m_shortcharargs = -1;
								this.m_longargs = -1;
								this.Info(String.Format("oldidx {0} (len {1})", oldidx, this.m_args.Length));
								this.m_curidx = oldidx;
								this.m_curcharidx = opt.shortopt.Length;
								this.Info(String.Format("[{0}]shortopt ({1})", oldidx, opt.shortopt));
								return opt;
							}
						}
						idx --;
					}

				}
			}

			keycls = this.__find_sub_command(this.m_args[oldidx]);
			if (keycls != null) {
				this.Info(String.Format("find {0}", this.m_args[oldidx]));
				this.m_keyidx = oldidx;
				this.m_validx = (oldidx + 1);
				this.m_curidx = (oldidx + 1);
				this.m_curcharidx = -1;
				this.m_shortcharargs = -1;
				this.m_longargs = -1;
				return keycls;
			} 
			if (this.m_parseall) {
				this.m_leftargs.Add(this.m_args[oldidx]);
				oldidx += 1;
				this.m_keyidx = -1;
				this.m_validx = oldidx;
				this.m_curidx = oldidx;
				this.m_curcharidx = -1;
				this.m_shortcharargs = -1;
				this.m_longargs = -1;
				return this.__find_key_cls();
			} 
			this.m_ended = 1;
			for (idx = oldidx ; idx < this.m_args.Length ; idx ++) {
				this.m_leftargs.Add(this.m_args[idx]);
			}
			this.m_keyidx = -1;
			this.m_curidx = oldidx;
			this.m_curcharidx = -1;
			this.m_shortcharargs = -1;
			this.m_longargs = -1;
			return null;
		}

		public KeyCls step_one(){
			KeyCls keycls;
			if (this.m_ended > 0) {
				return null;
			}
			keycls = this.__find_key_cls();
			if (keycls == null)  {
				return null;
			}
			return keycls;
		}

		public int validx {
			get {
				if (this.m_ended > 0) {
					return this.m_curidx;
				}
				return this.m_validx;
			}
			set {
				this.throw_exception(String.Format("can not set validx"));
			}
		}

		public object get_optval(KeyCls  keycls) {
			if (this.m_ended > 0) {
				return (object) this.m_leftargs;
			}
			if (! keycls.iscmd) {
				return (object) keycls.optdest;
			}
			return (object)this.format_cmdname_path(this.m_cmdpaths);
		}

		public List<_ParserCompact> get_cmd_paths() {
			return this.m_cmdpaths;
		}
	}
}