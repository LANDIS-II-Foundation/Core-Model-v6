using Landis.Ecoregions;
using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;

namespace Landis.Test.Ecoregions
{
    [TestFixture]
    public class EditableParameters_Test
    {
        private EditableParameters parameters;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            parameters = new EditableParameters();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(InputValueException))]
        public void NameEmpty()
        {
            InputValue<string> name = new InputValue<string>("", "");
            parameters.Name = name;
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(InputValueException))]
        public void NameWhitespace()
        {
            InputValue<string> name = new InputValue<string>("   ", "   ");
            parameters.Name = name;
        }
    }
}
