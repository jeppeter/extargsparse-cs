using System;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace extargsparse
{
	public class _ParserCompact : _LogObject
	{
		private KeyCls m_keycls;
		private List<KeyCls> m_cmdopts;
		private string m_cmdname;
		private string m_helpinfo;
		private string m_callfunction;
		private List<_ParserCompact> m_subcommands;
		private int m_screenwidth;
		private string m_epilog;
		private string m_description;
		private string m_prog;
		private string m_usage;
		private string m_version;

		public _ParserCompact(KeyCls kcls=null, ExtArgsOptions opt=null) : base()
		{
			JToken tok;
			if (kcls != null) {
				System.Diagnostics.Debug.Assert(kcls.iscmd);
				this.m_keycls = kcls;
				this.m_cmdname = kcls.cmdname;
				this.m_cmdopts = new List<KeyCls>();
				this.m_subcommands = new List<_ParserCompact>();
				this.m_helpinfo = String.Format("{0} handler", this.m_cmdname);
				if (kcls.helpinfo != "") {
					this.m_helpinfo = kcls.helpinfo;
				}
				this.m_callfunction = "";
				if (kcls.function != "") {
					this.m_callfunction = kcls.function;
				}
			} else {
				tok = JToken.Parse("{}");
				this.m_keycls = new KeyCls("","main",tok ,false);
				this.m_cmdname = "";
				this.m_cmdopts = new List<KeyCls>();
				this.m_subcommands = new List<_ParserCompact>();
				this.m_helpinfo = "";
				this.m_callfunction = "";
			}
			this.m_screenwidth = 0;
			if (opt != null && opt.get_int("screenwith") > 0) {
				this.m_screenwidth = (int)opt.get_int("screenwidth");
			}

			if (this.m_screenwidth <= 40){
				this.m_screenwidth = 40;
			}
			this.m_epilog = "";
			this.m_description = "";
			this.m_prog = "";
			this.m_usage = "";
			this.m_version = "";
			if (opt != null && opt.get_string("prog") != "") {
				this.m_prog = opt.get_string("prog");
			}
		}

		private string _get_opt_name(KeyCls opt){
			string optname = "";
			optname += opt.longopt;
			if (opt.shortopt != "") {
				optname += String.Format("|%s", opt.shortopt);
			}
			return optname;
		}

		private string _get_opt_expr(KeyCls opt) {
			string optexpr = "";
			if (opt.type != "bool" && opt.type != "args" && opt.type != "dict" && opt.type != "help") {
				optexpr += opt.varname;
				optexpr = optexpr.Replace("-","_");
			}
			return optexpr;
		}

		private string _get_opt_info(KeyCls opt) {
			string helpinfo = "";
			if (opt.attr!= null && opt.attr.Attr("opthelp") != "") {
				helpinfo = (string) this.call_func(opt.attr.Attr("opthelp"), opt);
			} else {
				if (opt.type == "bool") {
					bool bval;
					bval = (bool) opt.value;
					if (bval) {
						helpinfo += String.Format("{0} set false default(True)", opt.optdest);
					} else {
						helpinfo += String.Format("{0} set true default(False)", opt.optdest);
					}
				} else if (opt.type == "string" && (string)(opt.value) == "+"){
					if (opt.isflag) {
						helpinfo += String.Format("{0} inc" , opt.optdest);
					} else {
						this.throw_exception(String.Format("cmd({0}) can not set value({1})",opt.cmdname,opt.value));
					}
				} else if (opt.type == "help") {
					helpinfo += "to display this help information";
				} else {
					if (opt.isflag) {
						helpinfo += String.Format("{0} set default({1})",opt.optdest,opt.value);
					} else {
						helpinfo += String.Format("{0} command exec",opt.cmdname);
					}
				}
				if (opt.helpinfo != "") {
					helpinfo = opt.helpinfo;
				}
			}
			return helpinfo;
		}

		private string _get_cmd_cmdname(KeyCls cmd) {
			string cmdname = "";
			if (cmd.cmdname != "") {
				cmdname += cmd.cmdname;
			}
			return cmdname;
		}

		private string _get_cmd_helpinfo(KeyCls cmd) {
			string cmdhelp = "";
			if (cmd.helpinfo != "") {
				cmdhelp = cmd.helpinfo;
			}
			return cmdhelp;
		}

		public _HelpSize get_help_size(_HelpSize helpsize=null,int recursive=0) {
			_HelpSize retsize = helpsize;
			string optname,optexpr,opthelp;
			if (retsize == null) {
				retsize = new _HelpSize();
			}
			retsize.cmdnamesize = this.m_cmdname.Length;
			retsize.cmdhelpsize = this.m_helpinfo.Length;

			foreach( var opt in this.m_cmdopts) {
				if (opt.type == "args") {
					continue;
				}
				optname = this._get_opt_name(opt);
				optexpr = this._get_opt_expr(opt);
				opthelp = this._get_opt_info(opt);
				retsize.optnamesize = optname.Length + 1;
				retsize.optexprsize = optexpr.Length + 1;
				retsize.opthelpsize = opthelp.Length + 1;
			}

			if (recursive != 0) {
				foreach(var cmd in this.m_subcommands) {
					if (recursive > 0) {
						retsize = cmd.get_help_size(retsize,recursive - 1);
					} else {
						retsize = cmd.get_help_size(retsize,recursive);
					}
				}
			}

			foreach(var cmd in this.m_subcommands) {
				retsize.cmdnamesize = cmd.cmdname.Length + 2;
				retsize.cmdhelpsize = cmd.helpinfo.Length;
			}

			return retsize;
		}

		private string _get_indent_string(string s,int indentsize, int maxsize) {
			string rets="";
			string curs="";
			int i,j;
			char[] trimchar = {' ', '\t'};
			for (j=0;j < indentsize;j++) {
				curs += " ";
			}
			for (i=0; i < s.Length; i ++) {
				char c = s[i];
				if ((c == '\t' || c == ' ') && curs.Length >= maxsize) {
					rets += curs;
					rets += "\n";
					curs = "";
					for (j = 0 ;j < indentsize;j ++) {
						curs += " ";
					}
					continue;
				}
				curs += c;
			}
			if (curs.Trim(trimchar) != "") {
				rets += curs.Trim(trimchar) + "\n";
			}
			curs = "";
			return rets;
		}

		public string get_help_info(_HelpSize helpsize=null,List<_ParserCompact> parentcmds=null) {
			string s="";
			_ParserCompact rootcmd= null;
			_ParserCompact curcmd=null;
			int i;
			string curs;
			string fmts;
			string simpfmts;
			if (helpsize == null) {
				helpsize = this.get_help_size();
			}
			if (parentcmds == null && this.m_usage != "") {
				s += String.Format("{0}", this.m_usage);
			} else {
				rootcmd = this;
				curcmd = this;
				if (parentcmds != null && parentcmds.Count > 0) {
					curcmd = parentcmds[0];
				}
				this.Debug(String.Format("curcmd {0}", curcmd.format()));
				if (rootcmd.prog != "") {
					s += rootcmd.prog;
				} else {
					if (Environment.GetCommandLineArgs().Length > 0) {
						s += Environment.GetCommandLineArgs()[0];
					}					
				}
				if (rootcmd.version != "") {
					s += String.Format(" {0}", rootcmd.version);
				}

				if (parentcmds.Count > 0) {
					foreach(var cmd in parentcmds) {
						s += String.Format(" {0}", cmd.cmdname);
					}
				}
				s += String.Format(" {0}", this.cmdname);
				if (curcmd.helpinfo != "") {
					s += String.Format(" {0}", curcmd.helpinfo);
				} else {
					if (this.m_cmdopts.Count > 0) {
						s += String.Format(" [OPTIONS]");
					}
					if (this.m_subcommands.Count > 0) {
						s += String.Format(" [SUBCOMMANDS]");
					}
					foreach(var args in this.m_cmdopts) {
						if (args.flagname == "$") {
							string ftype ;
							object nargs = args.nargs;
							string sval;
							int ival;
							ftype = nargs.GetType().FullName;
							if (ftype == "System.String") {
								sval = (string) nargs;
								if (sval == "+") {
									s += " args...";
								} else if (sval == "*") {
									s += " [args...]";
								} else if (sval == "?") {
									s += " arg";
								}
							} else {
								ival = (int) nargs;
								if (ival > 1) {
									s += " args...";
								} else if (ival == 1) {
									s += " arg";
								}
							}
						}
					}
				}
				s += "\n";
			}

			if (this.m_description != "") {
				s += String.Format("{0}\n", this.m_description);
			}
			if (this.m_cmdopts.Count > 0) {
				s += "[OTPIONS]\n";
				fmts = "{";
				fmts += "0,";
				fmts += String.Format("-{0}",helpsize.optnamesize);
				fmts += "} {";
				fmts += "1,";
				fmts += String.Format("-{0}",helpsize.optexprsize);
				fmts += "} {";
				fmts += "2,";
				fmts += String.Format("-{0}", helpsize.opthelpsize);
				fmts += "}";
				simpfmts = "{";
				simpfmts += "0,";
				simpfmts += String.Format("-{0}", helpsize.optnamesize);
				simpfmts += "} {";
				simpfmts += "1,";
				simpfmts += String.Format("-{0}", helpsize.optexprsize);
				simpfmts += "}";
				foreach( var opt in this.m_cmdopts) {
					string optname,optexpr,opthelp;
					if (opt.type == "args") {
						continue;
					}
					optname = this._get_opt_name(opt);
					optexpr = this._get_opt_expr(opt);
					opthelp = this._get_opt_info(opt);
					curs = "";
					for (i=0; i < 4 ;i++) {
						curs += " ";
					}
					curs += String.Format(fmts, optname,optexpr,opthelp);
					if (curs.Length < this.m_screenwidth) {
						s += curs + "\n";
					} else {
						curs = "";
						for (i=0;i< 4 ;i++) {
							curs += " ";
						}
						curs += String.Format(simpfmts, optname, optexpr);
						s += curs + "\n";
						if (this.m_screenwidth > 60) {
							s += this._get_indent_string(curs,20,this.m_screenwidth);
						} else {
							s += this._get_indent_string(curs,15,this.m_screenwidth);
						}
					}
				}
			}

			if (this.m_subcommands.Count > 0) {
				s += "[SUBCOMMANDS]\n";
				fmts = "{";
				fmts += "0,";
				fmts += String.Format("-{0}", helpsize.cmdnamesize);
				fmts += "} {";
				fmts += "1,";
				fmts += String.Format("-{1}", helpsize.cmdhelpsize);
				fmts += "}";
				simpfmts = "{";
				simpfmts += "0,";
				simpfmts += String.Format("-{0}", helpsize.cmdnamesize);
				simpfmts += "}";
				foreach(var cmd in this.m_subcommands) {
					string cmdname;
					string cmdhelp;
					cmdname = cmd.cmdname;
					cmdhelp = cmd.helpinfo;
					curs = "";
					for (i=0; i < 4;i++) {
						curs += " ";
					}
					curs += String.Format(fmts,cmdname,cmdhelp);
					if (curs.Length < this.m_screenwidth) {
						s += curs + "\n";
					} else {
						curs = "";
						for (i= 0 ;i < 4 ;i++) {
							curs += " ";
						}
						curs += String.Format(simpfmts, cmdname);
						s += curs + "\n";
						if (this.m_screenwidth >= 60) {
							s += this._get_indent_string(cmdhelp,20,this.m_screenwidth);
						}else {
							s += this._get_indent_string(cmdhelp, 15, this.m_screenwidth);
						}
					}
				}
			}

			if (this.m_epilog != "") {
				s += String.Format("\n{0}\n", this.m_epilog);
			}
			this.Info(String.Format("{0}",s));
			return s;
		}

		public string format()
		{
			string s ="";
			int i;
			s += String.Format("@{0}|", this.m_cmdname);
			if (this.m_subcommands.Count > 0) {
				s += String.Format("subcommands<{0}<", this.m_subcommands.Count);
				i = 0;
				foreach( var c in this.m_subcommands) {
					if (i > 0) {
						s += "|";
					}
					s += String.Format("{0}",c.cmdname);
					i ++;
				}
				s += ">";
			}

			if (this.m_cmdopts.Count > 0) {
				s += String.Format("cmdopts[{0}]<", this.m_cmdopts.Count);
				i = 0;
				foreach( var o in this.m_cmdopts) {
					s += String.Format("{0}", o);
				}
				s += ">";
			}
			return s;
		}



		public string cmdname
		{
			get{
				return this.m_cmdname;
			}
			set{
				this.throw_exception(String.Format("can not set cmdname"));
			}
		}

		public string helpinfo
		{
			get{
				return this.m_helpinfo;
			}
			set {
				this.throw_exception(String.Format("can not set helpinfo"));
			}
		}

		public string prog
		{
			get {
				return this.m_prog;
			}
			set {
				this.throw_exception(String.Format("can not set prog"));
			}
		}

		public string version
		{
			get {
				return this.m_version;
			}
			set {
				this.throw_exception(String.Format("can not set version"));
			}
		}
	}
}