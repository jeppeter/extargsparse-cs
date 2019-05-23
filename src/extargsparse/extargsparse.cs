using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace extargsparse
{
public class  ExtArgsParse  : _LogObject
{
    private delegate bool load_command_line_func(string prefix, KeyCls keycls, List<_ParserCompact> curparser = null);
    private delegate int parse_action_func(NameSpaceEx ns, int validx, KeyCls keycls, string[] args);
    private delegate NameSpaceEx json_set_func(NameSpaceEx ns);
    private delegate void json_value_func(NameSpaceEx ns, KeyCls keycls, object value);

    private static string[] reserved_args = {"subcommand", "subnargs", "nargs", "extargs", "args"};

    private static Priority[]  m_defprior = {Priority.COMMAND_SET, Priority.SUB_COMMAND_JSON_SET, Priority.COMMAND_JSON_SET, Priority.ENVIRONMENT_SET, Priority.ENV_SUB_COMMAND_JSON_SET, Priority.ENV_COMMAND_JSON_SET, Priority.DEFAULT_SET};
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
    private Dictionary<string, load_command_line_func> m_loadcommandmap;
    private Dictionary<string, parse_action_func> m_optparsehandlemap;
    private Dictionary<Priority, json_set_func> m_parsesetmap;
    private Dictionary<string, json_value_func> m_jsonvaluemap;

    private bool _check_flag_insert(KeyCls keycls, List<_ParserCompact> curparser)
    {
        _ParserCompact lastparser = this.m_maincmd;
        if (curparser.Count > 0) {
            lastparser = curparser[curparser.Count - 1];
        }
        foreach (var k in lastparser.cmdopts) {
            if (k.flagname != "$" && keycls.flagname != "$") {
                if (k.type != "help" && keycls.type !=  "help") {
                    if (k.optdest == keycls.optdest) {
                        return false;
                    }
                } else if (k.type == "type" && keycls.type == "help") {
                    return false;
                }
            } else if (k.flagname == "$" && keycls.flagname == "$") {
                return false;
            }
        }
        lastparser.cmdopts.Add(keycls);
        return true;
    }

    private void _check_flag_insert_mustsucc(KeyCls keycls, List<_ParserCompact> curparser)
    {
        bool bval;
        string cmdname = "";
        bval = this._check_flag_insert(keycls, curparser);
        if (!bval) {
            if (curparser != null) {
                int i = 0;
                foreach (var c in curparser) {
                    if (i > 0) {
                        cmdname += ".";
                    }
                    cmdname += c.cmdname;
                    i ++;
                }
            }
            self.error_msg(String.Format("({0}) already in command({1})", keycls.flagname, cmdname));
        }
        return;
    }

    private bool _load_command_line_base(string prefix, KeyCls keycls, List<_ParserCompact> curparser = null)
    {
        if (keycls.isflag && keycls.flagname != "$" && Array.Find(ExtArgsParse.reserved_args, keycls.flagname) >= 0) {
            this.error_msg(String.Format("({0}) in reserved_args ({1})", keycls.flagname, ExtArgsParse.reserved_args));
        }
        this._check_flag_insert_mustsucc(keycls, curparser);
        return true;
    }

    private bool _load_command_line_args(string prefix, KeyCls keycls, List<_ParserCompact> curparser = null)
    {
        return this._check_flag_insert(keycls, curparser);
    }

    private bool _load_command_line_help(KeyCls keycls, List<_ParserCompact> curparser = null)
    {
        return this._check_flag_insert(keycls, curparser);
    }


    private bool _load_command_line_jsonfile(KeyCls keycls, List<_ParserCompact> curparser = null)
    {
        return this._check_flag_insert(keycls, curparser);
    }

    private _ParserCompact _get_subparser_inner(KeyCls keycls, List<_ParserCompact> curparser = null)
    {
        string cmdname = "";
        string parentname;
        parentname = this._format_cmd_from_cmd_array(curparser);
        cmdname += parentname;
        if (cmdname.Length > 0) {
            cmdname += ".";
        }
        cmdname += keycls.cmdname;
    }

    private bool _load_command_line_subparser(string prefix, KeyCls keycls, List<_ParserCompact> curparser = null)
    {
        _ParserCompact parser;
        List<_ParserCompact> nextparser = new List<_ParserCompact>();
        string nextprefix = "";
        if (keycls.value == null || keycls.value.GetType().FullName != "Newtonsoft.Json.Linq.JObject") {
            this.error_msg(String.Format("({0}) value must be dict", keycls.origkey));
        }
        if (keycls.iscmd && Array.Find(ExtArgsParse.reserved_args, keycls.cmdname) >= 0) {
            this.error_msg(String.Format("command({0}) in reserved_args ({1})", keycls.cmdname, ExtArgsParse.reserved_args));
        }

        parser = this._get_subparser_inner(keycls, curparser);
        if (curparser != null && curparser.Count > 0) {
            nextparser = curparser;
        } else {
            nextparser.Add(this.m_maincmd);
        }

        if (this.m_cmdprefixadded) {
            nextprefix = prefix;
            if (nextprefix.Length > 0) {
                nextprefix += "_";
            }
            nextprefix += keycls.cmdname;
        } else {
            nextprefix = "";
        }
        this._load_command_line_inner(nextprefix, keycls.value, nextparser);
        nextparser.RemoveAt(nextparser.Count - 1);
        return true;
    }

    private bool _load_command_line_prefix(string prefix, KeyCls keycls, List<_ParserCompact> curparser = null)
    {
        if (Array.Find(this.reserved_args, element => element == keycls.prefix) != "") {
            string msg;
            msg = String.Format("prefix ({0}) in reserved_args ({1})", keycls.prefix, this.reserved_args);
            this.error_msg(msg);
        }
        this._load_command_line_inner(keycls.prefix, keycls.value, curparser);
        return true;
    }

    private void _need_args_error(int validx, KeyCls keycls, string[] args)
    {
        string keyval = "";
        if (validx > 0) {
            keyval = args[validx - 1];
        }
        if (keycls.longopt == keyval) {
            keyval = keycls.longopt;
        } else if (keycls.shortflag != "" && keyval.IndexOf(keycls.shortflag) >= 1) {
            keyval = keycls.shortopt;
        }
        self.error_msg(String.Format("[{0}] need args", keyval));
        return;
    }

    private int _string_action(NameSpaceEx ns, int validx, KeyCls keycls, string[] args)
    {
        if (args.Length <= validx) {
            this._need_args_error(validx, keycls, args);
        }
        ns.set_value(keycls.optdest, args[validx]);
        return 1;
    }

    private int _bool_action(NameSpaceEx ns, int validx, KeyCls keycls, string[] args)
    {
        if (keycls.value) {
            ns.set_value(keycls.optdest, false);
        } else {
            ns.set_value(keycls.optdest, true);
        }
        return 0;
    }

    private int _int_action(NameSpaceEx ns, int validx, KeyCls keycls, string[] args)
    {
        int base = 10;
        string intstr;
        int intval = 0;
        char c;
        if (validx >= args.Length) {
            this._need_args_error(validx, keycls, args);
        }
        try {
            intstr = args[validx];
            if (args[validx].StartsWith("x") ||
                    args[validx].StartsWith("X")) {
                base = 16;
                intstr = args[validx].SubString(1);
            } else if (args[validx].StartsWith("0x") ||
                       args[validx].StartsWith("0X")) {
                base = 16;
                intstr = args[validx].SubString(2);
            }

            if (base == 16) {
                foreach (c in intstr) {
                    intval <<= 4;
                    if (c >= '0' && c <= '9') {
                        intval += c - '0';
                    } else if (c >= 'A' && c <= 'F') {
                        intval += 10 + c - 'A';
                    } else if (c >= 'a' && c <= 'f') {
                        intval += 10 + c - 'a';
                    } else {
                        this.throw_exception(String.Format("[{0}] not valid", intstr));
                    }
                }
            } else {
                foreach (c in intstr) {
                    intval *= 10;
                    if (c >= '0' && c <= '9') {
                        intval += c - '0';
                    } else {
                        this.throw_exception(String.Format("[{0}] not valid", intstr));
                    }
                }
            }
            ns.set_value(keycls.optdest, intval);
        } catch (KeyException e) {
            string msg = String.Format("{0} not valid int {1}", args[validx], e);
            this.error_msg(msg);
        }
        return 1;
    }

    private int _append_action(NameSpaceEx ns, int validx, KeyCls keycls, string[] args)
    {
        return 1;
    }


    private int _inc_action(NameSpaceEx ns, int validx, KeyCls keycls, string[] args)
    {
        return 0;
    }

    private int _help_action(NameSpaceEx ns, int validx, KeyCls keycls, string[] args)
    {
        return 0;
    }

    private int _command_action(NameSpaceEx ns, int validx, KeyCls keycls, string[] args)
    {
        return 0;
    }

    private int _float_action(NameSpaceEx ns, int validx, KeyCls keycls, string[] args)
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

    private string _format_cmd_from_cmd_array(List<_ParserCompact> curparser)
    {
        string s = "";
        int i;
        for (i = 0 ; i < curparser.Count; i++) {
            if (i > 0) {
                s += ".";
            }
            s += curparser[i].cmdname;
        }
        return s;
    }

    private bool _load_command_line_json_added(List<_ParserCompact> curparser)
    {
        string prefix = "";
        string key = String.Format("{0}##json input file to get the value set##", this.m_jsonlong);
        JToken jtok = JToken.Parse("null");
        KeyCls keycls;
        prefix = this._format_cmd_from_cmd_array(curparser);
        prefix = prefix.Replace('.', '_');
        keycls = new KeyCls(prefix, key, jtok, true, false, true, this.m_longprefix, this.m_shortprefix);
        return this._load_command_line_jsonfile(keycls, curparser);
    }

    private bool _load_command_line_help_added(List<_ParserCompact> curparser)
    {
        string key = "";
        JToken jtok = JToken.Parse("null");
        KeyCls keycls;
        key += String.Format("{0}", this.m_helplong);
        if (this.m_helpshort != "") {
            key += String.Format("|{0}", this.m_helpshort);
        }
        key += "##to display this help information##";
        keycls = new KeyCls("", key, jtok, true, true, false, this.m_longprefix, this.m_shortprefix);
        return this._load_command_line_help(keycls, curparser);
    }


    public ExtArgsParse(ExtArgsOptions options = null, object priority = null) : base()
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
        this.m_maincmd = new _ParserCompact(null, options);
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

        this.m_loadcommandmap = new Dictionary<string, load_command_line_func>();
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

        this.m_optparsehandlemap = new Dictionary<string, parse_action_func>();
        this.m_optparsehandlemap.Add("string", new parse_action_func(_string_action));
        this.m_optparsehandlemap.Add("bool", new parse_action_func(_bool_action));
        this.m_optparsehandlemap.Add("int", new parse_action_func(_int_action));
        this.m_optparsehandlemap.Add("list", new parse_action_func(_append_action));
        this.m_optparsehandlemap.Add("count", new parse_action_func(_inc_action));
        this.m_optparsehandlemap.Add("help", new parse_action_func(_help_action));
        this.m_optparsehandlemap.Add("jsonfile", new parse_action_func(_string_action));
        this.m_optparsehandlemap.Add("command", new parse_action_func(_command_action));
        this.m_optparsehandlemap.Add("float", new parse_action_func(_float_action));


        this.m_parsesetmap = new Dictionary<Priority, json_set_func>();
        this.m_parsesetmap.Add(Priority.SUB_COMMAND_JSON_SET , new json_set_func(_parse_sub_command_json_set));
        this.m_parsesetmap.Add(Priority.COMMAND_JSON_SET , new json_set_func(_parse_command_json_set));
        this.m_parsesetmap.Add(Priority.ENVIRONMENT_SET , new json_set_func(_parse_environment_set));
        this.m_parsesetmap.Add(Priority.ENV_SUB_COMMAND_JSON_SET , new json_set_func(_parse_env_subcommand_json_set));
        this.m_parsesetmap.Add(Priority.ENV_COMMAND_JSON_SET , new json_set_func(_parse_env_command_json_set));

        this.m_jsonvaluemap = new Dictionary<string, json_value_func>();
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

    public void error_msg(msg)
    {
        bool output = false;
        string s = "";
        if (this.m_outputmode.Count > 0) {
            if (this.m_outputmode[this.m_outputmode.Count - 1] == "bash") {
                s += "cat >&2 <<EXTARGSEOF\n";
                s += String.Format("parse command error\n    {0}\n", message);
                s += "EXTARGSEOF\n";
                s += "exit 3\n";
                Console.Out.Write(s);
                output = true;
                Environment.Exit(3);
            }
        }
        if (!output) {
            s += "parse command error\n";
            s += String.Format("    {0}", this.format_call_msg(msg, 2));
        }
        if (this.m_errorhandler == "exit") {
            Console.Error.Write(s);
            Environment.Exit(3);
        } else {
            this.throw_exception(s);
        }
        return;
    }

    private void _load_command_line_inner(string prefix, JObject jobj, List<_ParserCompact> curparser = null)
    {
        List<_ParserCompact> parentpath ;
        int i;
        Dictionary<string, JToken> nobj;
        if (curparser == null) {
            curparser = new List<_ParserCompact>();
        }
        if (!this.m_nojsonoption) {
            this._load_command_line_json_added(curparser);
        }
        if (!this.m_nohelpoption) {
            this._load_command_line_help_added(curparser);
        }

        if (curparser.Count != 0) {
            parentpath = curparser;
        } else {
            parentpath = new List<_ParserCompact>();
            parentpath.Add(this.m_maincmd);
        }

        nobj = jobj.ToObject<Dictionary<string, JToken>>();
        foreach (var kv in nobj) {
            KeyCls keycls;
            bool bval;
            this.Info(String.Format("{0} , {1} , {2} , True", prefix, kv.Key, kv.Value));
            keycls = new KeyCls(prefix, kv.Key, kv.Value, false, false, false, this.m_longprefix, this.m_shortprefix, this.m_options.get_bool("flagnochange"));
            bval = this.m_loadcommandmap[keycls.type].DynamicInvoke(prefix, keycls, parentpath);
            if (!bval) {
                this.error_msg(String.Format("can not add ({0},{1})", kv.Key, kv.Value));
            }
        }
        return;
    }

    private void _load_command_line(JObject jobj)
    {
        if (this.m_ended != 0) {
            this.throw_exception(String.Format("you have call parse_command_line before call load_command_line_string"));
        }
        this._load_command_line_inner("", jobj, null);
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