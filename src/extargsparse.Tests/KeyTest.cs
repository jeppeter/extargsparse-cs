using NUnit.Framework;
using extargsparse;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
}
}