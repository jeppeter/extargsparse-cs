using NUnit.Framework;
using extargsparse;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace extargsparse_Tests
{
    public class extargsparse_Tests
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
            return;
        }
    }
}