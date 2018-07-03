using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace extargsparse
{
public class KeyCls
{
    private static readonly Regex m_helpexpr = new Regex("##([^\\#\\!\\+\\<\\>\\|]+)##$", RegexOptions.IgnoreCase);
    private static readonly Regex m_cmdexpr = new Regex("^([^\\#\\<\\>\\+\\$\\!]+)", RegexOptions.IgnoreCase);
    private static readonly Regex m_prefixexpr = new Regex("\\+([a-zA-Z]+[a-zA-Z_\\-0-9]*)", RegexOptions.IgnoreCase);
    private static readonly Regex m_funcexpr = new Regex("<([^\\<\\>\\#\\$\\| \\t\\!]+)>", RegexOptions.IgnoreCase);
    private static readonly Regex m_flagexpr = new Regex("^([a-zA-Z_\\|\\?\\-]+[a-zA-Z_0-9\\|\\?\\-]*)", RegexOptions.IgnoreCase);
    private static readonly Regex m_mustflagexpr = new Regex("^\\$([a-zA-Z_\\|\\?]+[a-zA-Z_0-9\\|\\?\\-]*)",RegexOptions.IgnoreCase);
    private static readonly Regex m_attrexpr = new Regex("\\!([^\\<\\>\\$!\\#\\|]+)\\!");

    protected class KeyAttr
    {
    	private string m_splitchar = ";";
    	private Dictionary<string,string> m_obj;

    	public KeyAttr(string instr)
    	{
    		char sc ;
    		if (instr.ToLower().StartsWith("splitchar=") &&
    			instr.Length >= 7) {
    			sc = instr[6];
    			if (sc == char('.')) {
    				this.m_splitchar = ".";
    			} else if (sc == char('\\')) {
    				this.m_splitchar = "\\";
    			}
    		}
    	}

    	public string Attr(string key)
    	{

    	}

    	public string ToString()
    	{

    	}
    }


}
}
