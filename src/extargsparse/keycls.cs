using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
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
        private Dictionary<string, string> m_obj;
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
                if (kv < 2) {
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

        public string ToString()
        {
            string rets;
            int i;
            Dictionary<string, string>.KeyCollection  k ;
            rets = "{";
            i = 0;
            for (k in this.m_obj.Keys) {
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
                case JTokenType.Double:
                    this.typename = "float";
                    break;
                case JTokenType.String:
                case JTokenType.Null:
                    this.typename = "string"
                                    break;
                case JTokenType.Boolean:
                    this.typename = "bool";
                    break;
                default:
                    throw new KeyException(String.Format("unknown jvalue type [{0}]", val.Type));
                    break;
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

        public string ToString()
        {
            return this.typename;
        }
    }

    private object m_value = null;
    private string m_prefix = null;
    private string m_flagname = null;
    private string m_helpinfo = null;
    private string m_shortflag = null;
    private object m_nargs = null;
    private string m_varname = null;
    private string m_cmdname = null;
    private string m_function = null;
    private string m_origkey = null;
    private bool m_iscmd = null;
    private bool m_isflag = null;
    private string m_type = null;
    private KeyAttr m_attr = null;
    private bool m_nochange = false;
    private string m_longprefix = "--";
    private string m_shortprefix = "-";

    private void __reset()
    {
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
        return;
    }

    private void __throw_exception(string fmt)
    {
        new throw KeyException(fmt);
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

    private Object __get_value(KeyCls pthis, string name)
    {
        if (KeyCls.m_flagwords.Contains(name)) {
            switch (name) {
            case "longopt":
                return pthis.Longopt;
            case "shortopt":
                return pthis.Shortopt;
            case "optdest":
                return pthis.Optdest;
            case "needarg":
                return pthis.NeedArg;
            }
        } else if (KeyCls.m_flagwords.Contains(name)) {
            switch(name) {
                
            }
        }
    }

    private bool equal(KeyCls other)
    {
        Boolean bval = true;
        foreach

    }
}


}
