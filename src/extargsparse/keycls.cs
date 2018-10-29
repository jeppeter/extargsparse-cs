using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace extargsparse
{
public class KeyCls
{
    private static readonly Regex m_helpexpr = new Regex("##([^\\#\\!\\+\\<\\>\\|]+)##$", RegexOptions.IgnoreCase);
    private static readonly Regex m_cmdexpr = new Regex("^([^\\#\\<\\>\\+\\$\\!]+)", RegexOptions.IgnoreCase);
    private static readonly Regex m_prefixexpr = new Regex("\\+([a-zA-Z]+[a-zA-Z_\\-0-9]*)", RegexOptions.IgnoreCase);
    private static readonly Regex m_funcexpr = new Regex("<([^\\<\\>\\#\\$\\| \\t\\!]+)>", RegexOptions.IgnoreCase);
    private static readonly Regex m_flagexpr = new Regex("^([a-zA-Z_\\|\\?\\-]+[a-zA-Z_0-9\\|\\?\\-]*)", RegexOptions.IgnoreCase);
    private static readonly Regex m_mustflagexpr = new Regex("^\\$([a-zA-Z_\\|\\?]+[a-zA-Z_0-9\\|\\?\\-]*)", RegexOptions.IgnoreCase);
    private static readonly Regex m_attrexpr = new Regex("\\!([^\\<\\>\\$!\\#\\|]+)\\!");


    /*to get the flag words */
    private static readonly string[] m_flagwords =  new string[] {"flagname", "helpinfo", "shortflag", "nargs", "varname"};
    private static readonly string[] m_flagspecial = new string[] {"value", "prefix"};
    private static readonly string[] m_cmdwords = new string[] {"cmdname", "function", "helpinfo"};
    private static readonly string[] m_otherwords = new string[] {"origkey", "iscmd", "isflag", "type", "attr", "longprefix", "shortprefix"};
    private static readonly string[] m_formwords = new string[] {"longopt", "shortopt", "optdest", "needarg"};

    protected class KeyAttr
    {
        private char m_splitchar = ';';
        private Dictionary<string, string> m_obj = new Dictionary<string, string>();
        private static readonly string SPLIT_START = "split=";

        public KeyAttr(string instr)
        {
            char sc ;
            string[] vstr;
            string[] kv;
            int i;
            this.m_splitchar = ';';
            if (instr.ToLower().StartsWith(SPLIT_START) &&
                    instr.Length >= (SPLIT_START.Length + 1)) {
                sc = instr[SPLIT_START.Length];
                if (sc == '.' ||
                        sc == '\\' ||
                        sc == '/' ||
                        sc == ':' ||
                        sc == '@' ||
                        sc == '+') {
                    this.m_splitchar = sc;
                } else {
                    throw new KeyException(String.Format("splitchar {0} not valid", sc));
                }
            }
            vstr = instr.Split(this.m_splitchar);
            for (i = 0; i < vstr.Length; i++) {
                if (vstr[i].ToLower().StartsWith(SPLIT_START)) {
                    continue;
                }
                kv = vstr[i].Split('=');
                if (kv.Length < 2) {
                    this.m_obj.Add(kv[0], "");
                } else {
                    this.m_obj.Add(kv[0], kv[1]);
                }
            }
        }

        public string Attr(string key)
        {
            if (this.m_obj.ContainsKey(key)) {
                return this.m_obj[key];
            }
            return String.Empty;
        }

        public override string ToString()
        {
            string rets;
            int i;

            rets = "{";
            i = 0;
            foreach (KeyValuePair<string, string>  k in this.m_obj) {
                if (i > 0) {
                    rets += ";";
                }
                rets += String.Format("{0}={1}", k.Key, k.Value);
                i ++;
            }
            rets += "}";
            return rets;
        }
    }

    protected class TypeClass
    {
        private readonly string typename ;
        public TypeClass(JToken tok)
        {
            var valtype = "";
            JValue val;
            valtype = tok.GetType().FullName;
            if (valtype == "Newtonsoft.Json.Linq.JValue") {
                val = (JValue) tok;
                switch (val.Type) {
                case JTokenType.Integer:
                    this.typename = "int";
                    break;
                case JTokenType.Float:
                    this.typename = "float";
                    break;
                case JTokenType.String:
                case JTokenType.Null:
                    this.typename = "string";
                    break;
                case JTokenType.Boolean:
                    this.typename = "bool";
                    break;
                default:
                    throw new KeyException(String.Format("unknown jvalue type [{0}]", val.Type));
                }
            } else if (valtype == "Newtonsoft.Json.Linq.JArray") {
                this.typename = "list";
            } else if (valtype == "Newtonsoft.Json.Linq.JObject") {
                this.typename = "dict";
            } else {
                throw new KeyException(String.Format("unknown JToken type [{0}]", valtype));
            }
        }

        public string get_type()
        {
            return this.typename;
        }

        public override string ToString()
        {
            return this.typename;
        }
    }

    private JToken m_value = null;
    private string m_prefix = null;
    private string m_flagname = null;
    private string m_helpinfo = null;
    private string m_shortflag = null;
    private object m_nargs = null;
    private string m_varname = null;
    private string m_cmdname = null;
    private string m_function = null;
    private string m_origkey = null;
    private bool m_iscmd = false;
    private bool m_isflag = false;
    private string m_type = null;
    private KeyAttr m_attr = null;
    private bool m_nochange = false;
    private string m_longprefix = "--";
    private string m_shortprefix = "-";

    private void __reset()
    {
        string v;
        KeyAttr va;
        bool bv;
        object ov;
        this.m_value = null;
        this.m_prefix = null;
        this.m_flagname = null;
        this.m_helpinfo = null;
        this.m_shortflag = null;
        this.m_nargs = null;
        this.m_varname = null;
        this.m_cmdname = null;
        this.m_function = null;
        this.m_origkey = null;
        this.m_iscmd = false;
        this.m_isflag = false;
        this.m_type = null;
        this.m_attr = null;
        this.m_nochange = false;
        this.m_longprefix = "--";
        this.m_shortprefix = "-";

        v = this.m_cmdname;
        this.m_cmdname = v;

        bv = this.m_iscmd;
        this.m_iscmd = bv;

        v = this.m_function;
        this.m_function = v;

        v = this.m_varname;
        this.m_varname = v;

        v = this.m_helpinfo;
        this.m_helpinfo = v;

        va = this.m_attr;
        this.m_attr = va;
        ov = this.m_nargs;
        this.m_nargs = ov;

        return;
    }

    private void __throw_exception(string fmt)
    {
        throw new KeyException(fmt);
    }

    public string Longopt
    {
        get {
            string c = "";
            Boolean bval;
            if (!this.m_isflag || this.m_flagname == null ||
                    this.m_type == "args") {
                this.__throw_exception(String.Format("can not set ({0}) Longopt", this.m_origkey));
            }
            c = this.m_longprefix;
            if (this.m_type == "bool") {
                bval = (Boolean) this.m_value;
                if (bval) {
                    c += "no-";
                }
            }

            if (this.m_prefix.Length > 0 &&
                    this.m_type != "help") {
                c += String.Format("{0}_", this.m_prefix);
            }

            c += this.m_flagname;
            if (! this.m_nochange) {
                c = c.ToLower();
                c = c.Replace("_", "-");
            }
            return c;
        } set {
            this.__throw_exception(String.Format("Longopt can not set"));
        }
    }

    public string Shortopt
    {
        get {
            string c = null;
            if (!this.m_isflag ||
                    this.m_flagname == null ||
                    this.m_type == "args") {
                this.__throw_exception(String.Format("can not set ({0}) Shortopt", this.m_origkey));
            }

            if (this.m_shortflag != null) {
                c = String.Format("{0}{1}", this.m_shortprefix, this.m_shortflag);
            }
            return c;
        } set {
            this.__throw_exception(String.Format("Shortopt can not set"));
        }
    }

    public string Optdest
    {
        get {
            string c = "";
            if (!this.m_isflag ||
                    this.m_flagname == null ||
                    this.m_type == "args") {
                this.__throw_exception(String.Format("can not set ({0}) Optdest", this.m_origkey));
            }
            if (this.m_prefix.Length > 0) {
                c += String.Format("{0}_", this.m_prefix);
            }
            c += this.m_flagname;
            if (! this.m_nochange) {
                c = c.ToLower();
            }
            c = c.Replace("-", "_");
            return c;
        }

        set {
            this.__throw_exception(String.Format("Optdest can not set"));
        }
    }

    public int NeedArg
    {
        get {
            if (!this.m_isflag) {
                return 0;
            }

            if (this.m_type == "int" ||
                    this.m_type == "list" ||
                    this.m_type == "long" ||
                    this.m_type == "float" ||
                    this.m_type == "string" ||
                    this.m_type == "jsonfile") {
                return 1;
            }
            return 0;
        } set {
            this.__throw_exception(String.Format("NeedArg can not set"));
        }
    }

    private Object __get_value(string name)
    {
        if (Array.IndexOf(KeyCls.m_flagwords, name) >= 0) {
            switch (name) {
            case "longopt":
                return this.Longopt;
            case "shortopt":
                return this.Shortopt;
            case "optdest":
                return this.Optdest;
            case "needarg":
                return this.NeedArg;
            }
        } else if ( Array.IndexOf(KeyCls.m_flagwords, name) >= 0 ||
                    Array.IndexOf(KeyCls.m_cmdwords, name) >= 0 ||
                    Array.IndexOf(KeyCls.m_flagspecial, name) >= 0 ||
                    Array.IndexOf(KeyCls.m_otherwords, name) >= 0 ) {
            string kname = String.Format("m_{0}", name);
            Type t = typeof(KeyCls);
            FieldInfo info = t.GetField(kname, BindingFlags.NonPublic | BindingFlags.Instance);
            return info.GetValue(this);
        }
        return null;
    }

    private Boolean __eq_value(KeyCls other, string name)
    {
        Object v, ov;
        v = this.__get_value(name);
        ov = other.__get_value(name);
        if (v == null && ov == null) {
            return true;
        } else if (v == null && ov != null) {
            return false;
        } else if (v != null && ov == null) {
            return false;
        } else  {
            return v.Equals(ov);
        }
    }

    private Boolean __eq_array(KeyCls other, string[] narr)
    {
        Boolean bval = true;
        foreach (string s in narr) {
            bval = this.__eq_value(other, s);
            if (!bval) {
                return bval;
            }
        }
        return bval;
    }

    public Boolean Equals(KeyCls other)
    {
        Boolean bval = true;
        bval = this.__eq_array(other, KeyCls.m_flagwords);
        if (!bval) {
            return bval;
        }

        bval = this.__eq_array(other, KeyCls.m_flagspecial);
        if (!bval) {
            return bval;
        }
        return true;
    }

    private void __parse(string prefix, string key, JToken value, bool isflag, bool ishelp, bool isjsonfile)
    {
        bool flagmode = false;
        //bool cmdmode = false;
        string flags = "";
        bool ok = false;
        int cnt = 0;
        MatchCollection ms;
        string []sarr;
        Regex spexpr = new Regex("\\|");
        this.m_origkey = key;
        if (this.m_origkey.Contains("$")) {
            if (this.m_origkey[0] != '$') {
                this.__throw_exception(String.Format("[{0}] not valid key", this.m_origkey));
            }
            ok = true;
            cnt = 0;
            foreach (char ch in this.m_origkey) {
                if (ch == '$') {
                    cnt ++;
                }
            }
            if (cnt > 1) {
                ok = false;
            }
            if (!ok) {
                this.__throw_exception(String.Format("({0}) has ($) more than one", this.m_origkey));
            }
        }

        if (isflag || ishelp || isjsonfile) {
            ms = KeyCls.m_flagexpr.Matches(this.m_origkey);
            if (ms.Count > 1) {
                flags = ms[1].Value;
            }
            if (flags == "") {
                ms = KeyCls.m_mustflagexpr.Matches(this.m_origkey);
                if (ms.Count > 1) {
                    flags = ms[1].Value;
                }
            }

            if (flags == "" && this.m_origkey[0] == '$') {
                this.m_flagname = "$";
                flagmode = true;
            }
            if (flags != "") {
                if (flags.Contains("|")) {
                    sarr = spexpr.Split(flags);
                    if (sarr.Length > 2 || sarr[1].Length != 1 || sarr[0].Length <= 1) {
                        this.__throw_exception(String.Format("({0}) ({1})flag only accept (longop|l) format", this.m_origkey, flags));
                    }
                    this.m_flagname = sarr[0];
                    this.m_shortflag = sarr[1];
                } else {
                    this.m_flagname = flags;
                }
                flagmode = true;
            }
        } else {
            ms = KeyCls.m_mustflagexpr.Matches(this.m_origkey);
            if (ms.Count > 1) {
                flags = ms[1].Value;
                if (flags.Contains("|")) {
                    sarr = spexpr.Split(flags);
                    if (sarr.Length > 2 || sarr[1].Length > 1 || sarr[0].Length <= 1) {
                        this.__throw_exception(String.Format("({0}) ({1})flag only accept (longop|l) format", this.m_origkey, flags));
                    }
                    this.m_flagname = sarr[0];
                    this.m_shortflag = sarr[1];
                } else {
                    if (flags.Length <= 1) {
                        this.__throw_exception(String.Format("({0}) flag must have long opt", this.m_origkey));
                    }
                    this.m_flagname = flags;
                }
                flagmode = true;
            } else if (this.m_origkey[0] == '$') {
                this.m_flagname = "$";
                flagmode = true;
            }
            ms = KeyCls.m_cmdexpr.Matches(this.m_origkey);
            if (ms.Count > 1) {
                
            }
        }
        return;
    }


    public KeyCls(string prefix, string key, JToken value, bool isflag = false, bool ishelp = false,
                  bool isjsonfile = false, string longprefix = "--",
                  string shortprefix = "-", bool nochange = false)
    {
        string valtype;
        TypeClass typcls;
        this.__reset();
        typcls = new TypeClass(key);
        valtype = typcls.get_type();

        this.m_origkey = key;
        this.m_longprefix = longprefix;
        this.m_shortprefix = shortprefix;
        this.m_nochange = nochange;
        if (valtype == "dict") {
            this.__throw_exception(String.Format("key not accept dict[{0}]", key));
        } else {
            this.__parse(prefix, key, value, isflag, ishelp, isjsonfile);
        }
    }
}


}
