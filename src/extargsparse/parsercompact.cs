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
				Debug.Assert(kcls.iscmd);
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

		private string _get_opt_helpname(KeyCls opt){
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
			if (retsize == null) {
				retsize = new _HelpSize();
			}
			retsize.cmdnamesize = this.m_cmdname.Length;
			retsize.cmdhelpsize = this.m_helpinfo.Length;

			return retsize;
		}

	}
}