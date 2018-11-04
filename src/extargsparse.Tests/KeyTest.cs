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
            Assert.AreEqual(flags.Flagname,"flag");
            Assert.AreEqual(flags.Longopt,"--type-flag");
            Assert.AreEqual(flags.Shortopt,"-f");
            Assert.AreEqual(flags.Optdest, "type_flag");
            Assert.AreEqual(flags.Value,"string");
            Assert.AreEqual(flags.Type,"string");
            Assert.AreEqual(flags.Shortflag,"f");
            return;
        }
    }
}