using NUnit.Framework;
using RhinoInt;
using Interfaces;

namespace RhinoInt.Test
{
    [TestFixture]
    public class RhinoServiceTests
    {
        [Test]
        public void RunTestCommand_DoesNotThrow()
        {
            IRhinoService service = new RhinoService();
            Assert.DoesNotThrow(() => service.RunTestCommand());
        }
    }
}