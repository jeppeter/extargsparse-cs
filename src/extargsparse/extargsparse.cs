using System;
using System.Collections.Generic;

namespace extargsparse
{
	public class  ExtArgsParse  : _LogObject
	{
		private delegate bool load_command_line_func(string prefix,KeyCls keycls,List<_ParserCompact> curparser=null);
		//private delegate int parse_action_func(NameSpaceEx args, int validx, KeyCls keycls,string[] params);

		private static Priority[]  m_defprior = {Priority.COMMAND_SET,Priority.SUB_COMMAND_JSON_SET,Priority.COMMAND_JSON_SET,Priority.ENVIRONMENT_SET,Priority.ENV_SUB_COMMAND_JSON_SET,Priority.ENV_COMMAND_JSON_SET,Priority.DEFAULT_SET};
		private Priority[] m_priority;
		private ExtArgsOptions  m_options;
		private _ParserCompact  m_maincmd;
		private string m_helphandler;
		private List<string> m_outputmode;
		private int m_ended;
		private string m_longprefix;
		private string m_shortprefix;
		private bool m_nohelpoption;
		private bool m_nojsonoption;
		private string m_helplong;
		private string m_helpshort;
		private string m_jsonlong;
		private bool m_cmdprefixadded;
		private string m_errorhandler;
		private Dictionary<string,load_command_line_func> m_loadcommandmap;
		//private Dictionary<string,parse_action_func> m_optparsehandlemap;

		private bool _load_command_line_base(string prefix,KeyCls keycls, List<_ParserCompact> curparser=null)
		{
			return true;
		}

		private bool _load_command_line_args(string prefix, KeyCls keycls, List<_ParserCompact> curparser=null)
		{
			return true;
		}

		private bool _load_command_line_help(string prefix, KeyCls keycls, List<_ParserCompact> curparser=null)
		{
			return true;
		}

		private bool _load_command_line_jsonfile(string prefix, KeyCls keycls, List<_ParserCompact> curparser=null)
		{
			return true;
		}

		private bool _load_command_line_subparser(string prefix, KeyCls keycls, List<_ParserCompact> curparser=null)
		{
			return true;
		}

		private bool _load_command_line_prefix(string prefix, KeyCls keycls, List<_ParserCompact> curparser=null)
		{
			return true;
		}

		private int _string_action(NameSpaceEx args, int validx, KeyCls keycls, string[] params)
		{
			return 1;
		}

		private int _bool_action(NameSpaceEx args, int validx, KeyCls keycls, string[] params)
		{
			return 0;
		}

		private int _int_action(NameSpaceEx args, int validx, KeyCls keycls, string[] params)
		{
			return 1;
		}

		private int _append_action(NameSpaceEx args, int validx, KeyCls keycls, string[] params)
		{
			return 1;
		}


		private int _inc_action(NameSpaceEx args, int validx, KeyCls keycls, string[] params)
		{
			return 0;
		}

		private int _help_action(NameSpaceEx args, int validx, KeyCls keycls, string[] params)
		{
			return 0;
		}

		private int _command_action(NameSpaceEx args, int validx, KeyCls keycls, string[] params)
		{
			return 0;
		}

		private int _float_action(NameSpaceEx args, int validx, KeyCls keycls, string[] params)
		{
			return 1;
		}


		public ExtArgsParse(ExtArgsOptions options=null, object priority=null) : base()
		{
			string vtype;
			if (priority != null) {
				vtype = priority.GetType().FullName;
				if (vtype != "extargsparse.Priority") {
					this.throw_exception(String.Format("second parameter not Priority type"));
				}
				this.m_priority = (Priority[])priority;
			}  else {
				this.m_priority =  ExtArgsParse.m_defprior;
			}

			if (options == null)  {
				options = new ExtArgsOptions("{}");
			}
			this.m_options =  options;
			this.m_maincmd = new _ParserCompact(null,options);
			this.m_helphandler = this.m_options.get_string("helphandler");
			this.m_outputmode = new List<string>(); 
			this.m_ended = 0;
			this.m_longprefix = options.get_string("longprefix");
			this.m_shortprefix = options.get_string("shortprefix");
			this.m_nohelpoption = options.get_bool("nohelpoption");
			this.m_nojsonoption = options.get_bool("nojsonoption");
			this.m_helplong = options.get_string("helplong");
			this.m_helpshort = options.get_string("helpshort");
			this.m_jsonlong = options.get_string("jsonlong");
			this.m_cmdprefixadded = options.get_bool("cmdprefixadded");
			this.m_errorhandler = options.get_string("errorhandler");

			this.m_loadcommandmap = new Dictionary<string,load_command_line_func>();
			this.m_loadcommandmap.Add("string", new load_command_line_func(_load_command_line_base));
			this.m_loadcommandmap.Add("int", new load_command_line_func(_load_command_line_base));
			this.m_loadcommandmap.Add("float", new load_command_line_func(_load_command_line_base));
			this.m_loadcommandmap.Add("list", new load_command_line_func(_load_command_line_base));
			this.m_loadcommandmap.Add("bool", new load_command_line_func(_load_command_line_base));
			this.m_loadcommandmap.Add("args", new load_command_line_func(_load_command_line_base));
			this.m_loadcommandmap.Add("command", new load_command_line_func(_load_command_line_subparser));
			this.m_loadcommandmap.Add("prefix", new load_command_line_func(_load_command_line_prefix));
			this.m_loadcommandmap.Add("count", new load_command_line_func(_load_command_line_base));
			this.m_loadcommandmap.Add("help", new load_command_line_func(_load_command_line_base));
			this.m_loadcommandmap.Add("jsonfile", new load_command_line_func(_load_command_line_base));

			this.m_optparsehandlemap = new Dictionary<string,parse_action_func>();
			this.m_optparsehandlemap.Add("string", new parse_action_func(_string_action));

		}
	}

}