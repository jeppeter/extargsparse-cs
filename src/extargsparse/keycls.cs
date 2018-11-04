using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

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

    public string longopt
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

    public string shortopt
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

    public string optdest
    {
        get {
            string c = "";
            if (!this.m_isflag ||
                    this.m_flagname == null ||
                    this.m_type == "args") {
                this.__throw_exception(String.Format("can not set ({0}) optdest", this.m_origkey));
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
            this.__throw_exception(String.Format("optdest can not set"));
        }
    }

    public string flagname
    {
        get {
            return this.m_flagname;
        }
        set {
            this.__throw_exception(String.Format("Flagname can not set"));
        }
    }

    public object value
    {
        get {
            string typ;
            JValue val;
            if (this.m_value != null) {
                typ = this.m_value.GetType().FullName;
                if (typ == "Newtonsoft.Json.Linq.JValue") {
                    val = (JValue) this.m_value;
                    switch(val.Type) {
                    case JTokenType.Integer:
                        return (int) val;
                    case JTokenType.Float:
                        return (float) val;
                    case JTokenType.String:
                        return (string) val;
                    case JTokenType.Null:
                        return null;
                    case JTokenType.Boolean:
                        return (bool) val;
                    }
                    this.__throw_exception(String.Format("can not determin [{0}] value [{1}]", this.m_origkey, this.m_value));
                } else if (typ == "Newtonsoft.Json.Linq.JArray" || typ == "Newtonsoft.Json.Linq.JObject") {
                    return this.m_value;
                } else  {
                    this.__throw_exception(String.Format("[{0}] value type [{1}] [{2}]", this.m_origkey, typ,this.m_value));
                }
            } 
            return null;            
        }
        set {
            this.__throw_exception(String.Format("Value can not set"));
        }
    }

    public string type
    {
        get {
            return this.m_type;
        }
        set {
            this.__throw_exception(String.Format("Type can not set"));
        }
    }

    public string shortflag
    {
        get {
            return this.m_shortflag;
        }
        set {
            this.__throw_exception(String.Format("Shortflag can not set"));
        }
    }

    public string prefix
    {
        get {
            return this.m_prefix;
        }
        set {
            this.__throw_exception(String.Format("Prefix can not set"));
        }
    }

    public int needarg
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
                return this.longopt;
            case "shortopt":
                return this.shortopt;
            case "optdest":
                return this.optdest;
            case "needarg":
                return this.needarg;
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

    private void __set_value(string k, object v)
    {
        if (Array.IndexOf(KeyCls.m_flagwords, k) >= 0 ||
                Array.IndexOf(KeyCls.m_cmdwords, k) >= 0 ||
                Array.IndexOf(KeyCls.m_flagspecial, k) >= 0 ||
                Array.IndexOf(KeyCls.m_otherwords, k) >= 0) {
            string kname = String.Format("m_{0}", k);
            Type t  = typeof(KeyCls);
            FieldInfo info = t.GetField(kname, BindingFlags.NonPublic | BindingFlags.Instance);
            info.SetValue(this, v);
        }
        this.__throw_exception(String.Format("{0} not support key", k));
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

    private bool __object_equal(object a , object b)
    {
        string atype, btype;
        atype = a.GetType().FullName;
        btype = b.GetType().FullName;
        if (atype != btype) {
            return false;
        }
        return a.Equals(b);
    }

    private void __set_flag(string prefix, string key, JToken value)
    {
        JObject jobj;
        List<String> keys;
        Dictionary<string, JToken> nobj;
        string k;
        int i;
        object gv;
        JToken v;
        JValue jval;
        TypeClass typcls ;
        string strval;
        this.m_isflag = true;
        this.m_iscmd = false;
        this.m_origkey = key;
        Debug.Assert(value.GetType().FullName == "Newtonsoft.Json.Linq.JObject");
        jobj = value.Value<JObject>();
        if (jobj["value"] != null) {
            this.m_value = null;
            this.m_type = "string";
        }
        nobj = jobj.ToObject<Dictionary<string, JToken>>();
        keys = new List<String>(nobj.Keys);

        for (i = 0; i < keys.Count ; i++) {
            k = keys[i];
            if (Array.IndexOf(KeyCls.m_flagwords, k) >= 0) {
                v = nobj[k] ;
                gv = this.__get_value(k);
                if (gv.Equals("") && !this.__object_equal(v, gv)) {
                    this.__throw_exception(String.Format("set ({0}) for not equal value ({1}) ({2})", k , v, gv));
                }
                typcls = new TypeClass(v);
                if (!(typcls.get_type() != "string" && typcls.get_type() != "int")) {
                    this.__throw_exception(String.Format("({0})({1})({2}) can not take other than int or string ({3})", this.m_origkey, k, v, typcls.get_type()));
                }
                this.__set_value(k, v);
            } else if (Array.IndexOf(KeyCls.m_flagspecial, k) >= 0) {
                if (k == "prefix") {
                    string newprefix;
                    typcls = new TypeClass(nobj[k]);
                    if (typcls.get_type() != "string") {
                        this.__throw_exception(String.Format("({0}) prefix not string", nobj[k]));
                    }
                    jval = (JValue) nobj[k];
                    switch (jval.Type) {
                    case JTokenType.Null:
                        this.__throw_exception(String.Format("{0} is None", k));
                        break;
                    }
                    newprefix = "";
                    if (prefix.Length > 0) {
                        newprefix += String.Format("{0}_", prefix);
                    }
                    newprefix += (System.String)nobj[k] ;
                    this.m_prefix = newprefix;
                } else if (k == "value") {
                    typcls = new TypeClass(nobj[k]);
                    if (typcls.get_type() == "dict") {
                        this.__throw_exception(String.Format("({0})({1}) can not accept dict", this.m_origkey, k));
                    }
                    this.m_value = nobj[k];
                    this.m_type = typcls.get_type();
                } else {
                    this.__set_value(k, nobj[k]);
                }
            } else if (k == "attr") {
                if (this.m_attr == null ) {
                    strval = (string) nobj[k];
                    this.m_attr = new KeyAttr(strval);
                }
            }
        }
        if (this.m_prefix == "" && prefix.Length > 0) {
            this.m_prefix = prefix;
        }
        return;
    }

    private void __validate()
    {
        TypeClass typcls;
        string typestr;
        int ival;
        if (this.m_isflag) {
            Debug.Assert(!this.m_iscmd);
            if (this.m_function != null) {
                this.__throw_exception(String.Format("({0}) can not accept function", this.m_origkey));
            }
            if (this.m_type == "dict" && this.m_flagname != "") {
                this.__throw_exception(String.Format("({0}) flag can not accept dict", this.m_origkey));
            }
            typcls = new TypeClass(this.m_value);
            if (this.m_type != typcls.get_type() && this.m_type != "count" && 
                this.m_type != "help" && this.m_type != "jsonfile") {
                this.__throw_exception(String.Format("({0}) value ({1}) not match type ({2})", this.m_origkey,
                    this.m_value, this.m_type));
            }

            if (this.m_flagname == "") {
                if (this.m_prefix == "") {
                    this.__throw_exception(String.Format("({0}) should at least for prefix", this.m_origkey));
                }
                this.m_type = "prefix";
                typcls = new TypeClass(this.m_value);
                if (typcls.get_type() != "dict") {
                    this.__throw_exception(String.Format("({0}) should used dict to make prefix", this.m_origkey));
                }

                if (this.m_helpinfo != "") {
                    this.__throw_exception(String.Format("({0}) should not have help info", this.m_origkey));
                }

                if (this.m_shortflag != "") {
                    this.__throw_exception(String.Format("({0}) should not set shortflag", this.m_origkey));
                }
            } else if (this.m_flagname == "$") {
                this.m_type = "args";
                if (this.m_shortflag != "") {
                    this.__throw_exception(String.Format("({0}) can not set shortflag for args",this.m_origkey));
                }
            } else {
                if (this.m_flagname.Length <= 0) {
                    this.__throw_exception(String.Format("({0}) can not accept ({1})short flag in flagname", this.m_origkey, this.m_flagname));
                }
            }
            if (this.m_shortflag != "") {
                if (this.m_shortflag.Length > 1) {
                    this.__throw_exception(String.Format("({0}) can not accept ({0}) for shortflag", this.m_origkey, this.m_shortflag));
                }
            }

            if (this.m_type == "bool") {
                if (this.m_nargs != null ) {
                    ival = (int) this.m_nargs;
                    if (ival != 0) {
                        this.__throw_exception(String.Format("bool type ({0}) can not accept not 0 nargs", this.m_nargs));    
                    }                    
                }
                this.m_nargs = 0;
            } else if (this.m_type == "help")  {
                if (this.m_nargs != null) {
                    ival = (int) this.m_nargs;
                    if (ival != 0) {
                        this.__throw_exception(String.Format("help type ({0}) can not accept not 0 nargs", this.m_nargs));    
                    }                    
                }
                this.m_nargs = 0;
            } else if (this.m_type != "prefix" && this.m_flagname != "$" && this.m_type != "count") {
                if  (this.m_flagname != "$" && this.m_nargs != null) {
                    typestr = this.m_nargs.GetType().FullName;
                    if (typestr == "System.Int32") {
                        ival = (int) this.m_nargs;
                        if (ival != 1) {
                            this.__throw_exception(String.Format("({0})only $ can accept nargs option", this.m_origkey));
                        }
                    }
                }
                this.m_nargs = 1;
            } else if (this.m_flagname == "$" && this.m_nargs == null) {
                this.m_nargs = "*";
            }
        } else {
            if (this.m_cmdname == "") {
                this.__throw_exception(String.Format("({0}) not set cmdname",this.m_origkey));
            }
            if (this.m_shortflag != "") {
                this.__throw_exception(String.Format("({0}) has shortflag ({0})", this.m_origkey, this.m_shortflag));
            }
            if (this.m_nargs != null) {
                this.__throw_exception(String.Format("({0}) has nargs ({0})", this.m_origkey, this.m_nargs));
            }
            if (this.m_type != "dict") {
                this.__throw_exception(String.Format("({0}) command must be dict", this.m_origkey));
            }
            if (this.m_prefix == "")  {
                this.m_prefix  +=  this.m_cmdname;
            }
            this.m_type = "command";
        }

        if (this.m_isflag && this.m_varname == "" && this.m_flagname != "") {
            if (this.m_flagname != "$") {
                this.m_varname = this.optdest;
            } else {
                if (this.m_prefix.Length > 0) {
                    this.m_varname = "subnargs";
                } else {
                    this.m_varname = "args";
                }
            }
        }
        return;
    }

    private void __parse(string prefix, string key, JToken value, bool isflag, bool ishelp, bool isjsonfile)
    {
        bool flagmode = false;
        bool cmdmode = false;
        string flags = "";
        string newprefix = "";
        bool ok = false;
        int cnt = 0;
        MatchCollection ms;
        string []sarr;
        TypeClass typcls;
        Regex spexpr = new Regex("\\|");
        string valtype;
        JValue jval;
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
            if (ms.Count == 1 && ms[0].Groups.Count == 2) {
                flags = ms[0].Groups[1].Value;
            }
            if (flags == "") {
                ms = KeyCls.m_mustflagexpr.Matches(this.m_origkey);
                if (ms.Count == 1 && ms[0].Groups.Count == 2) {
                    flags = ms[0].Groups[1].Value;
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
            if (ms.Count == 1 && ms[0].Groups.Count == 2) {
                flags = ms[0].Groups[1].Value;
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
            if (ms.Count == 1 && ms[0].Groups.Count == 2) {
                Debug.Assert(! flagmode);
                flags = ms[0].Groups[1].Value;
                if (flags.Contains("|")) {
                        sarr = spexpr.Split(flags);
                        if (sarr.Length > 2 || sarr[0].Length <= 1 || sarr[1].Length != 1) {
                            this.__throw_exception(String.Format("({0}) ({1})flag only accept (longop|l) format", this.m_origkey, flags));
                        }
                        this.m_flagname = sarr[0];
                        this.m_shortflag = sarr[1];
                    flagmode = true;
                } else {
                    this.m_cmdname = flags;
                    cmdmode = true;
                }
            }
        }
        ms = KeyCls.m_helpexpr.Matches(this.m_origkey);
        if (ms.Count  == 1 && ms[0].Groups.Count == 2) {
            this.m_helpinfo = ms[0].Groups[1].Value;
        }
        newprefix = "";
        if (prefix.Length > 0) {
            newprefix = prefix;
        }
        ms = KeyCls.m_prefixexpr.Matches(this.m_origkey);
        if (ms.Count == 1 && ms[0].Groups.Count == 2) {
            if (newprefix.Length > 0) {
                newprefix += "_";
            }
            newprefix += ms[0].Groups[1].Value;
            this.m_prefix = newprefix;
        } else {
            if (newprefix.Length > 0) {
                this.m_prefix = newprefix;
            }
        }
        if (flagmode) {
            this.m_isflag = true;
            this.m_iscmd = false;
        }
        if (cmdmode) {
            this.m_isflag = false;
            this.m_iscmd = true;
        }

        if (!this.m_isflag && !this.m_iscmd) {
            /*default is flag*/
            this.m_isflag = true;
            this.m_iscmd = false;
        }

        this.m_value = value;
        if (! ishelp && ! isjsonfile) {
            typcls = new TypeClass(value);
            this.m_type = typcls.get_type();
        } else if (ishelp) {
            this.m_type = "help";
            this.m_nargs = 0;
        } else if (isjsonfile) {
            this.m_type = "jsonfile";
            this.m_nargs = 1;
        }

        if (this.m_type == "help" && value != null) {
            this.__throw_exception(String.Format("help type must be value None"));
        }
        if (cmdmode && this.m_type != "dict") {
            flagmode = true;
            cmdmode = false;
            this.m_isflag = true;
            this.m_iscmd = false;
            this.m_flagname = this.m_cmdname;
            this.m_cmdname = "";
        }

        if (this.m_isflag && this.m_type == "string" && this.m_flagname != "$") {
            Debug.Assert(value != null);
            valtype = value.GetType().FullName;
            if (valtype == "Newtonsoft.Json.Linq.JValue") {
                jval = value as JValue;
                if (jval.Type == JTokenType.String) {
                    if ((System.String)jval.Value == "+") {
                        jval = new JValue(0);
                        this.m_value = jval;
                        this.m_type = "count";
                        this.m_nargs = 0;
                    }
                }
            }
        }

        if (this.m_isflag && this.m_flagname == "$" && this.m_type != "dict") {
            if (this.m_type == "string") {
                string strval = (System.String)(this.m_value as JValue);
                if (! "+*?".Contains(strval)) {
                    this.__throw_exception(String.Format("{0} not valid for $", strval));
                }
                this.m_nargs = strval;
                this.m_value = null;
                this.m_type = "string";
            } else if ( this.m_type == "int") {
                int intval = (System.Int32) (this.m_value as JValue);
                this.m_value = null;
                this.m_type = "string";
                this.m_nargs = intval;
            } else {
                this.__throw_exception(String.Format("({0})({1})({1}) for $ should option dict set opt or +?* specialcase or type int", prefix, this.m_origkey, this.m_value));
            }
        }

        if (this.m_isflag && this.m_type == "dict" && this.m_flagname != "") {
            this.__set_flag(prefix, key, value);
        }

        ms = KeyCls.m_attrexpr.Matches(this.m_origkey);
        if (ms.Count == 1 && ms[0].Groups.Count == 2) {
            this.m_attr = new KeyAttr(ms[0].Groups[1].Value);
        }

        ms = KeyCls.m_funcexpr.Matches(this.m_origkey);        
        if (ms.Count == 1 && ms[0].Groups.Count == 2) {
            if (this.m_isflag) {
                this.m_varname = ms[0].Groups[1].Value;
            } else {
                this.m_function = ms[0].Groups[1].Value;
            }
        }
        this.__validate();

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
