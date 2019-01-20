using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace extargsparse
{
	public class  ExtArgsParse  : _LogObject
	{
		private delegate bool load_command_line_func(string prefix,KeyCls keycls,List<_ParserCompact> curparser=null);
		private delegate int parse_action_func(NameSpaceEx ns, int validx, KeyCls keycls,string[] args);
		private delegate NameSpaceEx json_set_func(NameSpaceEx ns);
		private delegate void json_value_func(NameSpaceEx ns,KeyCls keycls,object value);

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
		private Dictionary<string,parse_action_func> m_optparsehandlemap;
		private Dictionary<Priority, json_set_func> m_parsesetmap;
		private Dictionary<string,json_value_func> m_jsonvaluemap;

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

		private int _string_action(NameSpaceEx ns,int validx,KeyCls keycls,string[] args)
		{
			return 1;
		}

		private int _bool_action(NameSpaceEx ns,int validx,KeyCls keycls,string[] args)
		{
			return 0;
		}

		private int _int_action(NameSpaceEx ns,int validx,KeyCls keycls,string[] args)
		{
			return 1;
		}

		private int _append_action(NameSpaceEx ns,int validx,KeyCls keycls,string[] args)
		{
			return 1;
		}


		private int _inc_action(NameSpaceEx ns,int validx,KeyCls keycls,string[] args)
		{
			return 0;
		}

		private int _help_action(NameSpaceEx ns,int validx,KeyCls keycls,string[] args)
		{
			return 0;
		}

		private int _command_action(NameSpaceEx ns,int validx,KeyCls keycls,string[] args)
		{
			return 0;
		}

		private int _float_action(NameSpaceEx ns,int validx,KeyCls keycls,string[] args)
		{
			return 1;
		}

		private NameSpaceEx _parse_sub_command_json_set(NameSpaceEx ns)
		{
			return ns;
		}


		private NameSpaceEx _parse_command_json_set(NameSpaceEx ns)
		{
			return ns;
		}

		private NameSpaceEx _parse_environment_set(NameSpaceEx ns)
		{
			return ns;
		}

		private NameSpaceEx _parse_env_subcommand_json_set(NameSpaceEx ns)
		{
			return ns;
		}

		private NameSpaceEx _parse_env_command_json_set(NameSpaceEx ns)
		{
			return ns;
		}

		private void _json_value_string(NameSpaceEx ns, KeyCls keycls, object value)
		{
			return;
		}

		private void _json_value_int(NameSpaceEx ns, KeyCls keycls, object value)
		{
			return;
		}

		private void _json_value_bool(NameSpaceEx ns, KeyCls keycls, object value)
		{
			return;
		}

		private void _json_value_list(NameSpaceEx ns, KeyCls keycls, object value)
		{
			return;
		}

		private void _json_value_float(NameSpaceEx ns, KeyCls keycls, object value)
		{
			return;
		}

		private void _json_value_error(NameSpaceEx ns, KeyCls keycls, object value)
		{
			return;
		}

		private void _load_command_line_json_added(List<_ParserCompact> curparser)
		{
			return;
		}

		private void _load_command_line_help_added(List<_ParserCompact> curparser)
		{
			return;
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
			this.m_optparsehandlemap.Add("bool", new parse_action_func(_bool_action));
			this.m_optparsehandlemap.Add("int", new parse_action_func(_int_action));
			this.m_optparsehandlemap.Add("list", new parse_action_func(_append_action));
			this.m_optparsehandlemap.Add("count", new parse_action_func(_inc_action));
			this.m_optparsehandlemap.Add("help", new parse_action_func(_help_action));
			this.m_optparsehandlemap.Add("jsonfile", new parse_action_func(_string_action));
			this.m_optparsehandlemap.Add("command", new parse_action_func(_command_action));
			this.m_optparsehandlemap.Add("float", new parse_action_func(_float_action));


			this.m_parsesetmap = new Dictionary<Priority,json_set_func>();
			this.m_parsesetmap.Add(Priority.SUB_COMMAND_JSON_SET , new json_set_func(_parse_sub_command_json_set));
			this.m_parsesetmap.Add(Priority.COMMAND_JSON_SET , new json_set_func(_parse_command_json_set));
			this.m_parsesetmap.Add(Priority.ENVIRONMENT_SET , new json_set_func(_parse_environment_set));
			this.m_parsesetmap.Add(Priority.ENV_SUB_COMMAND_JSON_SET , new json_set_func(_parse_env_subcommand_json_set));
			this.m_parsesetmap.Add(Priority.ENV_COMMAND_JSON_SET , new json_set_func(_parse_env_command_json_set));

			this.m_jsonvaluemap = new Dictionary<string,json_value_func>();
			this.m_jsonvaluemap.Add("string", new json_value_func(_json_value_string));
			this.m_jsonvaluemap.Add("bool", new json_value_func(_json_value_bool));
			this.m_jsonvaluemap.Add("int", new json_value_func(_json_value_int));
			this.m_jsonvaluemap.Add("list", new json_value_func(_json_value_list));
			this.m_jsonvaluemap.Add("count", new json_value_func(_json_value_int));
			this.m_jsonvaluemap.Add("jsonfile", new json_value_func(_json_value_string));
			this.m_jsonvaluemap.Add("float", new json_value_func(_json_value_float));
			this.m_jsonvaluemap.Add("command", new json_value_func(_json_value_error));
			this.m_jsonvaluemap.Add("help", new json_value_func(_json_value_error));
		}

		private void _load_command_line_inner(string prefix,JObject jobj, List<_ParserCompact> curparser=null)
		{
			if (curparser == null) {
				curparser = new List<_ParserCompact>();
			}
			if (!this.m_nojsonoption) {
				this._load_command_line_json_added(curparser);
			}
			if (!this.m_nohelpoption) {
				this._load_command_line_help_added(curparser);
			}
			return;
		}

		private void _load_command_line(JObject jobj)
		{
			if (this.m_ended != 0) {
				this.throw_exception(String.Format("you have call parse_command_line before call load_command_line_string"));
			}
			this._load_command_line_inner("",jobj,null);
			return;
		}

		public void load_command_line_string(string s)
		{
			JToken tok = JToken.Parse(s);
			string stype = tok.GetType().FullName;
			if (stype != "Newtonsoft.Json.Linq.JObject") {
				this.throw_exception(String.Format("[{0}] not type {}", s));
			}
			this._load_command_line(tok.Value<JObject>());
			return;
		}

	}

}