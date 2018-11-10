using NUnit.Framework;
using extargsparse;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace extargsparse_Tests
{
public class keycls_Tests
{
    [SetUp]
    public void setUp()
    {
        return;
    }
    [TearDown]
    public void tearDown()
    {
        return;
    }

    private void __opt_fail_check(KeyCls keycls)
    {
        bool ok =false;
        string val;
        try {
            val = keycls.longopt;
        }
        catch(KeyException ec) {
            KeyException nec;
            nec = ec;
            ec = nec;
            ok = true;
        }
        Assert.AreEqual(ok, true);

        ok = false;
        try {
            val = keycls.optdest;
        }
        catch(KeyException ec) {
            KeyException nec;
            nec = ec;
            ec = nec;
            ok = true;
        }
        Assert.AreEqual(ok, true);

        ok = false;
        try {
            val = keycls.shortopt;
        }
        catch(KeyException ec) {
            KeyException nec;
            nec = ec;
            ec = nec;
            ok = true;
        }
        Assert.AreEqual(ok, true);
        return;
    }


    [Test]
    public void test_A001()
    {
        KeyCls flags;
        JToken  jval;
        jval = JToken.Parse("\"string\"");
        flags = new KeyCls("", "$flag|f+type", jval, false);
        Assert.AreEqual(flags.flagname, "flag");
        Assert.AreEqual(flags.longopt, "--type-flag");
        Assert.AreEqual(flags.shortopt, "-f");
        Assert.AreEqual(flags.optdest, "type_flag");
        Assert.AreEqual(flags.value, "string");
        Assert.AreEqual(flags.type, "string");
        Assert.AreEqual(flags.shortflag, "f");
        Assert.AreEqual(flags.prefix, "type");
        Assert.AreEqual(flags.cmdname, null);
        Assert.AreEqual(flags.helpinfo, null);
        Assert.AreEqual(flags.isflag, true);
        Assert.AreEqual(flags.iscmd, false);
        Assert.AreEqual(flags.varname, "type_flag");
        return;
    }

    [Test]
    public void test_A002()
    {
        KeyCls flags;
        JToken jval;
        jval = JToken.Parse("[]");
        flags = new KeyCls("", "$flag|f+type", jval, true);
        Assert.AreEqual(flags.flagname, "flag");
        Assert.AreEqual(flags.shortflag, "f");
        Assert.AreEqual(flags.prefix, "type");
        Assert.AreEqual(flags.longopt, "--type-flag");
        Assert.AreEqual(flags.shortopt, "-f");
        Assert.AreEqual(flags.optdest, "type_flag");
        Assert.AreEqual(flags.value, jval);
        Assert.AreEqual(flags.type, "list");
        Assert.AreEqual(flags.helpinfo , null);
        Assert.AreEqual(flags.function , null);
        Assert.AreEqual(flags.cmdname , null);
        Assert.AreEqual(flags.isflag, true);
        Assert.AreEqual(flags.iscmd, false);
        Assert.AreEqual(flags.varname, "type_flag");
        return;
    }

    [Test]
    public void test_A003()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("false");
        flags = new KeyCls("","flag|f",jval,false);
        Assert.AreEqual(flags.flagname,"flag");
        Assert.AreEqual(flags.shortflag,"f");
        Assert.AreEqual(flags.longopt,"--flag");
        Assert.AreEqual(flags.shortopt,"-f");
        Assert.AreEqual(flags.value,false);
        Assert.AreEqual(flags.type,"bool");
        Assert.AreEqual(flags.prefix, "");
        Assert.AreEqual(flags.helpinfo, null);
        Assert.AreEqual(flags.function , null);
        Assert.AreEqual(flags.cmdname , null);
        Assert.AreEqual(flags.isflag, true);
        Assert.AreEqual(flags.iscmd, false);
        Assert.AreEqual(flags.varname,"flag");
        return;
    }

    [Test]
    public void test_A004()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("{}");
        flags = new KeyCls("newtype","flag<flag.main>##help for flag##", jval, false);
        Assert.AreEqual(flags.cmdname,"flag");
        Assert.AreEqual(flags.function,"flag.main");
        Assert.AreEqual(flags.type,"command");
        Assert.AreEqual(flags.prefix,"newtype");
        Assert.AreEqual(flags.helpinfo,"help for flag");
        Assert.AreEqual(flags.flagname,null);
        Assert.AreEqual(flags.shortflag,null);
        Assert.AreEqual(flags.value,jval);
        Assert.AreEqual(flags.isflag,false);
        Assert.AreEqual(flags.iscmd,true);
        this.__opt_fail_check(flags);
        Assert.AreEqual(flags.varname,null);
        return;
    }


    [Test]
    public void test_A005()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("\"\"");
        flags = new KeyCls("","flag<flag.main>##help for flag##", jval, true);
        Assert.AreEqual(flags.cmdname,null);
        Assert.AreEqual(flags.function,null);
        Assert.AreEqual(flags.type,"string");
        Assert.AreEqual(flags.prefix,"");
        Assert.AreEqual(flags.flagname,"flag");
        Assert.AreEqual(flags.helpinfo,"help for flag");
        Assert.AreEqual(flags.shortflag,null);
        Assert.AreEqual(flags.value,"");
        Assert.AreEqual(flags.isflag,true);
        Assert.AreEqual(flags.iscmd,false);
        Assert.AreEqual(flags.longopt,"--flag");
        Assert.AreEqual(flags.shortopt,null);
        Assert.AreEqual(flags.optdest,"flag");
        Assert.AreEqual(flags.varname,"flag.main");
        return;
    }

    [Test]
    public void test_A006()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("{\"new\":false}");
        flags = new KeyCls("","flag+type<flag.main>##main",jval,false);
        Assert.AreEqual(flags.cmdname , "flag");
        Assert.AreEqual(flags.prefix , "type");
        Assert.AreEqual(flags.function , "flag.main");
        Assert.AreEqual(flags.helpinfo, null);
        Assert.AreEqual(flags.flagname, null);
        Assert.AreEqual(flags.shortflag, null);
        Assert.AreEqual(flags.isflag, false);
        Assert.AreEqual(flags.iscmd, true);
        Assert.AreEqual(flags.type,"command");
        Assert.AreEqual(flags.value,jval);
        Assert.AreEqual(flags.varname,null);
        this.__opt_fail_check(flags);
        return;
    }

    [Test]
    public void test_A007()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("{}");
        flags = new KeyCls("","+flag", jval,false);
        Assert.AreEqual(flags.prefix,"flag");
        Assert.AreEqual(flags.value,jval);
        Assert.AreEqual(flags.cmdname , null);
        Assert.AreEqual(flags.shortflag , null);
        Assert.AreEqual(flags.flagname ,null);
        Assert.AreEqual(flags.function , null);
        Assert.AreEqual(flags.helpinfo , null);
        Assert.AreEqual(flags.isflag, true);
        Assert.AreEqual(flags.iscmd, false);
        Assert.AreEqual(flags.type,"prefix");
        Assert.AreEqual(flags.varname, null);
        this.__opt_fail_check(flags);
        return;
    }

    [Test]
    public void test_A008()
    {
        JToken jval;
        KeyCls flags;
        int ok = 0;
        try{
            jval = JToken.Parse("null");
            flags = new KeyCls("","+flag## help ##",jval,false);
        }
        catch(KeyException ec){
            KeyException nec;
            nec = ec;
            ec = nec;
            ok = 1;
        }
        Assert.AreEqual(ok ,1);
        return;
    }

    [Test]
    public void test_A009()
    {
        JToken jval;
        KeyCls flags;
        int ok = 0;
        try{
            jval = JToken.Parse("null");
            flags = new KeyCls("","+flag<flag.main>",jval,false);
        }
        catch(KeyException ec){
            KeyException nec;
            nec = ec;
            ec = nec;
            ok = 1;
        }
        Assert.AreEqual(ok ,1);
        return;
    }

    [Test]
    public void test_A010()
    {
        JToken jval;
        KeyCls flags;
        int ok = 0;
        try{
            jval = JToken.Parse("\"\"");
            flags = new KeyCls("","flag|f2",jval,false);
        }
        catch(KeyException ec){
            KeyException nec;
            nec = ec;
            ec = nec;
            ok = 1;
        }
        Assert.AreEqual(ok ,1);
        return;
    }

    [Test]
    public void test_A011()
    {
        JToken jval;
        KeyCls flags;
        int ok = 0;
        try{
            jval = JToken.Parse("null");
            flags = new KeyCls("","f|f2",jval,false);
        }
        catch(KeyException ec){
            KeyException nec;
            nec = ec;
            ec = nec;
            ok = 1;
        }
        Assert.AreEqual(ok ,1);
        return;
    }

    [Test]
    public void test_A012()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("{}");
        flags = new KeyCls("","$flag|f<flag.main>", jval,false);
        Assert.AreEqual(flags.prefix,"");
        Assert.AreEqual(flags.value,null);
        Assert.AreEqual(flags.cmdname, null);
        Assert.AreEqual(flags.shortflag ,"f");
        Assert.AreEqual(flags.flagname,"flag");
        Assert.AreEqual(flags.function, null);
        Assert.AreEqual(flags.helpinfo, null);
        Assert.AreEqual(flags.isflag,true);
        Assert.AreEqual(flags.iscmd,false);
        Assert.AreEqual(flags.type,"string");
        Assert.AreEqual(flags.varname,"flag.main");
        Assert.AreEqual(flags.longopt,"--flag");
        Assert.AreEqual(flags.shortopt,"-f");
        Assert.AreEqual(flags.optdest,"flag");
        return;
    }

    [Test]
    public void test_A013()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("null");
        flags = new KeyCls("","$flag|f+cc<flag.main>",jval,false);
        Assert.AreEqual(flags.prefix,"cc");
        Assert.AreEqual(flags.value,null);
        Assert.AreEqual(flags.cmdname, null);
        Assert.AreEqual(flags.shortflag ,"f");
        Assert.AreEqual(flags.flagname,"flag");
        Assert.AreEqual(flags.function,null);
        Assert.AreEqual(flags.helpinfo,null);
        Assert.AreEqual(flags.isflag,true);
        Assert.AreEqual(flags.iscmd,false);
        Assert.AreEqual(flags.type,"string");
        Assert.AreEqual(flags.varname,"flag.main");
        Assert.AreEqual(flags.longopt,"--cc-flag");
        Assert.AreEqual(flags.shortopt,"-f");
        Assert.AreEqual(flags.optdest,"cc_flag");
        return;
    }

    [Test]
    public void test_A014()
    {
        JToken jval;
        KeyCls flags;
        int ok = 0;
        try{
            jval = JToken.Parse("\"\"");
            flags = new KeyCls("","c$",jval,false);
        }
        catch(KeyException ec){
            KeyException nec;
            nec = ec;
            ec = nec;
            ok = 1;
        }
        Assert.AreEqual(ok ,1);
        return;
    }

    [Test]
    public void test_A015()
    {
        JToken jval;
        KeyCls flags;
        int ok = 0;
        try{
            jval = JToken.Parse("null");
            flags = new KeyCls("","$$",jval,false);
        }
        catch(KeyException ec){
            KeyException nec;
            nec = ec;
            ec = nec;
            ok = 1;
        }
        Assert.AreEqual(ok ,1);
        return;
    }

    [Test]
    public void test_A016()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("{ \"nargs\":\"+\"}");
        flags = new KeyCls("","$",jval,false);
        Assert.AreEqual(flags.flagname , "$");
        Assert.AreEqual(flags.prefix ,"");
        Assert.AreEqual(flags.type,"args");
        Assert.AreEqual(flags.varname,"args");
        Assert.AreEqual(flags.value, null);
        Assert.AreEqual(flags.nargs,"+");
        Assert.AreEqual(flags.cmdname, null);
        Assert.AreEqual(flags.shortflag, null);
        Assert.AreEqual(flags.function, null);
        Assert.AreEqual(flags.helpinfo, null);
        Assert.AreEqual(flags.isflag, true);
        Assert.AreEqual(flags.iscmd, false);
        this.__opt_fail_check(flags);
        return;
    }

    [Test]
    public void test_A017()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("3.3");
        flags = new KeyCls("type","flag+app## flag help ##",jval,false);
        Assert.AreEqual(flags.flagname ,"flag");
        Assert.AreEqual(flags.prefix , "type_app");
        Assert.AreEqual(flags.cmdname, null);
        Assert.AreEqual(flags.shortflag , null);
        Assert.AreEqual(flags.function , null);     
        Assert.AreEqual(flags.type,"float");
        Assert.AreEqual(flags.value,(double)3.3);
        Assert.AreEqual(flags.longopt,"--type-app-flag");
        Assert.AreEqual(flags.shortopt,null);
        Assert.AreEqual(flags.optdest,"type_app_flag");
        Assert.AreEqual(flags.helpinfo, " flag help ");
        Assert.AreEqual(flags.isflag, true);
        Assert.AreEqual(flags.iscmd, false);
        Assert.AreEqual(flags.varname,"type_app_flag");
        return;
    }

    [Test]
    public void test_A018()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("{}");
        flags = new KeyCls("","flag+app<flag.main>## flag help ##",jval,false);
        Assert.AreEqual(flags.flagname , null);
        Assert.AreEqual(flags.prefix , "app");
        Assert.AreEqual(flags.cmdname, "flag");
        Assert.AreEqual(flags.shortflag , null);
        Assert.AreEqual(flags.varname, null);
        Assert.AreEqual(flags.type,"command");
        Assert.AreEqual(flags.value,jval);
        Assert.AreEqual(flags.function , "flag.main");
        Assert.AreEqual(flags.helpinfo, " flag help ");
        Assert.AreEqual(flags.isflag, false);
        Assert.AreEqual(flags.iscmd, true);
        this.__opt_fail_check(flags);
        return;
    }

    [Test]
    public void test_A019()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("{\"prefix\":\"good\",\"value\":false}");
        flags = new KeyCls("","$flag## flag help ##",jval,false);
        Assert.AreEqual(flags.flagname , "flag");
        Assert.AreEqual(flags.prefix , "good");
        Assert.AreEqual(flags.value, false);
        Assert.AreEqual(flags.type,"bool");
        Assert.AreEqual(flags.helpinfo, " flag help ");
        Assert.AreEqual(flags.varname, "good_flag");
        Assert.AreEqual(flags.nargs, 0);
        Assert.AreEqual(flags.shortflag , null);
        Assert.AreEqual(flags.cmdname, null);
        Assert.AreEqual(flags.function , null);
        Assert.AreEqual(flags.longopt, "--good-flag");
        Assert.AreEqual(flags.shortopt, null);
        Assert.AreEqual(flags.optdest, "good_flag");
        return;
    }

    [Test]
    public void test_A020()
    {
        KeyCls flags;
        int ok = 0;
        try{
            flags = new KeyCls("","$",null,false);
        }
        catch(KeyException ec){
            KeyException nec;
            nec = ec;
            ec = nec;
            ok = 1;
        }
        Assert.AreEqual(ok ,1);
        return;
    }

    [Test]
    public void test_A021()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("{\"nargs\":\"?\",\"value\":null}");
        flags = new KeyCls("command","$## self define ##",jval,false);
        Assert.AreEqual(flags.iscmd , false);
        Assert.AreEqual(flags.isflag , true);
        Assert.AreEqual(flags.prefix , "command");
        Assert.AreEqual(flags.varname, "subnargs");
        Assert.AreEqual(flags.flagname , "$");
        Assert.AreEqual(flags.shortflag , null);
        Assert.AreEqual(flags.value, null);
        Assert.AreEqual(flags.type,"args");
        Assert.AreEqual(flags.nargs, "?");
        Assert.AreEqual(flags.helpinfo, " self define ");
        this.__opt_fail_check(flags);
        return;
    }

    [Test]
    public void test_A022()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("{}");
        flags = new KeyCls("command","+flag",jval,false);
        Assert.AreEqual(flags.prefix , "command_flag");
        Assert.AreEqual(flags.value, jval);
        Assert.AreEqual(flags.cmdname, null);
        Assert.AreEqual(flags.shortflag , null);
        Assert.AreEqual(flags.flagname , null);
        Assert.AreEqual(flags.varname, null);
        Assert.AreEqual(flags.function, null);
        Assert.AreEqual(flags.helpinfo, null);
        Assert.AreEqual(flags.isflag , true);
        Assert.AreEqual(flags.iscmd , false);
        Assert.AreEqual(flags.type,"prefix");
        this.__opt_fail_check(flags);
        return;
    }

    [Test]
    public void test_A023()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("{\"prefix\":\"good\",\"value\":3.9,\"nargs\":1}");
        flags = new KeyCls("","$flag## flag help ##",jval,false);
        Assert.AreEqual(flags.flagname , "flag");
        Assert.AreEqual(flags.prefix , "good");
        Assert.AreEqual(flags.value, (double) 3.9);
        Assert.AreEqual(flags.type,"float");
        Assert.AreEqual(flags.helpinfo, " flag help ");
        Assert.AreEqual(flags.nargs, 1);
        Assert.AreEqual(flags.shortflag , null);
        Assert.AreEqual(flags.cmdname, null);
        Assert.AreEqual(flags.function, null);
        Assert.AreEqual(flags.longopt, "--good-flag");
        Assert.AreEqual(flags.shortopt, null);
        Assert.AreEqual(flags.optdest, "good_flag");
        Assert.AreEqual(flags.varname, "good_flag");
        return;
    }

    [Test]
    public void test_A024()
    {
        KeyCls flags;
        JToken jval;
        int ok = 0;
        try{
            jval = JToken.Parse("{\"prefix\":\"good\",\"value\":false,\"nargs\":2}");
            flags = new KeyCls("","$flag## flag help ##",jval,false);
        }
        catch(KeyException ec){
            KeyException nec;
            nec = ec;
            ec = nec;
            ok = 1;
        }
        Assert.AreEqual(ok ,1);
        return;
    }

    [Test]
    public void test_A026()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("\"+\"");
        flags = new KeyCls("dep","$",jval,false);
        Assert.AreEqual(flags.flagname , "$");
        Assert.AreEqual(flags.prefix , "dep");
        Assert.AreEqual(flags.value, null);
        Assert.AreEqual(flags.type,"args");
        Assert.AreEqual(flags.helpinfo, null);
        Assert.AreEqual(flags.nargs, "+");
        Assert.AreEqual(flags.shortflag , null);
        Assert.AreEqual(flags.cmdname, null);
        Assert.AreEqual(flags.function, null);
        Assert.AreEqual(flags.varname, "subnargs");
        this.__opt_fail_check(flags);
        return;
    }

    [Test]
    public void test_A027()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("\"+\"");
        flags = new KeyCls("dep","verbose|v",jval,false);
        Assert.AreEqual(flags.flagname , "verbose");
        Assert.AreEqual(flags.shortflag , "v");
        Assert.AreEqual(flags.prefix , "dep");
        Assert.AreEqual(flags.type,"count");
        Assert.AreEqual(flags.value, (System.Int64)0);
        Assert.AreEqual(flags.helpinfo, null);
        Assert.AreEqual(flags.nargs, 0);
        Assert.AreEqual(flags.cmdname, null);
        Assert.AreEqual(flags.function, null);
        Assert.AreEqual(flags.longopt, "--dep-verbose");
        Assert.AreEqual(flags.shortopt, "-v");
        Assert.AreEqual(flags.optdest, "dep_verbose");
        Assert.AreEqual(flags.varname, "dep_verbose");
        return;
    }

    [Test]
    public void test_A028()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("\"+\"");
        flags = new KeyCls("","verbose|v## new help info ##",jval,false);
        Assert.AreEqual(flags.flagname , "verbose");
        Assert.AreEqual(flags.shortflag , "v");
        Assert.AreEqual(flags.prefix , "");
        Assert.AreEqual(flags.type,"count");
        Assert.AreEqual(flags.value, (System.Int64)0);
        Assert.AreEqual(flags.helpinfo, " new help info ");
        Assert.AreEqual(flags.nargs, 0);
        Assert.AreEqual(flags.cmdname, null);
        Assert.AreEqual(flags.function, null);
        Assert.AreEqual(flags.longopt, "--verbose");
        Assert.AreEqual(flags.shortopt, "-v");
        Assert.AreEqual(flags.optdest, "verbose");
        Assert.AreEqual(flags.varname, "verbose");
        return;
    }

    [Test]
    public void test_A029()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("true");
        flags = new KeyCls("","rollback|R## rollback not set ##",jval,false);
        Assert.AreEqual(flags.flagname , "rollback");
        Assert.AreEqual(flags.shortflag , "R");
        Assert.AreEqual(flags.prefix , "");
        Assert.AreEqual(flags.type,"bool");
        Assert.AreEqual(flags.value, true);
        Assert.AreEqual(flags.helpinfo, " rollback not set ");
        Assert.AreEqual(flags.nargs, 0);
        Assert.AreEqual(flags.cmdname, null);
        Assert.AreEqual(flags.function, null);
        Assert.AreEqual(flags.longopt, "--no-rollback");
        Assert.AreEqual(flags.shortopt, "-R");
        Assert.AreEqual(flags.optdest, "rollback");
        Assert.AreEqual(flags.varname, "rollback");
        return;
    }

    [Test]
    public void test_A030()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("0xffffffff");
        flags = new KeyCls("","maxval|m##max value set ##",jval,false);
        Assert.AreEqual(flags.flagname , "maxval");
        Assert.AreEqual(flags.shortflag , "m");
        Assert.AreEqual(flags.prefix , "");
        Assert.AreEqual(flags.type,"int");
        Assert.AreEqual(flags.value, (System.Int64)0xffffffff);
        Assert.AreEqual(flags.helpinfo, "max value set ");
        Assert.AreEqual(flags.nargs, 1);
        Assert.AreEqual(flags.cmdname, null);
        Assert.AreEqual(flags.function, null);
        Assert.AreEqual(flags.longopt, "--maxval");
        Assert.AreEqual(flags.shortopt, "-m");
        Assert.AreEqual(flags.optdest, "maxval");
        Assert.AreEqual(flags.varname, "maxval");
        return;
    }

    [Test]
    public void test_A031()
    {
        JToken jval;
        KeyCls flags;
        jval = JToken.Parse("[\"maxval\"]");
        flags = new KeyCls("","maxval|m",jval,false);
        Assert.AreEqual(flags.flagname , "maxval");
        Assert.AreEqual(flags.shortflag , "m");
        Assert.AreEqual(flags.prefix , "");
        Assert.AreEqual(flags.type,"list");
        Assert.AreEqual(flags.value, jval);
        Assert.AreEqual(flags.helpinfo, null);
        Assert.AreEqual(flags.nargs, 1);
        Assert.AreEqual(flags.cmdname, null);
        Assert.AreEqual(flags.function, null);
        Assert.AreEqual(flags.longopt, "--maxval");
        Assert.AreEqual(flags.shortopt, "-m");
        Assert.AreEqual(flags.optdest, "maxval");
        Assert.AreEqual(flags.varname, "maxval");
        return;
    }
}
}