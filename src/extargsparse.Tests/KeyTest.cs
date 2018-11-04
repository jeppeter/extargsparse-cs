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


        [Test]
        public void test_A001()
        {
            KeyCls flags;
            JToken  jval;
            jval = JToken.Parse("\"string\"");
            flags = new KeyCls("","$flag|f+type",jval,false);
            Assert.AreEqual(flags.flagname,"flag");
            Assert.AreEqual(flags.longopt,"--type-flag");
            Assert.AreEqual(flags.shortopt,"-f");
            Assert.AreEqual(flags.optdest, "type_flag");
            Assert.AreEqual(flags.value,"string");
            Assert.AreEqual(flags.type,"string");
            Assert.AreEqual(flags.shortflag,"f");
            Assert.AreEqual(flags.prefix,"type");
            Assert.AreEqual(flags.cmdname, null);
            Assert.AreEqual(flags.helpinfo,null);
            Assert.AreEqual(flags.isflag, true);
            Assert.AreEqual(flags.iscmd, false);
            Assert.AreEqual(flags.varname, "type_flag");
            return;
        }
    }
}