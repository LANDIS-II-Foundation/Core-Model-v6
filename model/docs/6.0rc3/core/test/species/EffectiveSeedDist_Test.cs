using Landis.Species;
using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;

namespace Landis.Test.Species
{
    [TestFixture]
    public class EffectiveSeedDist_Test
    {
        InputVar<int> inputVar;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            inputVar = new InputVar<int>("Effective Seed Dist", EffectiveSeedDist.ReadMethod);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ReadMethod_JustDigits()
        {
            StringReader reader = new StringReader("1234");
            inputVar.ReadValue(reader);
            Assert.AreEqual(1234, inputVar.Value.Actual);
            Assert.AreEqual("1234", inputVar.Value.String);
            Assert.AreEqual(0, inputVar.Index);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ReadMethod_PlusDigits()
        {
            StringReader reader = new StringReader("+1,234");
            inputVar.ReadValue(reader);
            Assert.AreEqual(1234, inputVar.Value.Actual);
            Assert.AreEqual("+1,234", inputVar.Value.String);
            Assert.AreEqual(0, inputVar.Index);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ReadMethod_MinusDigits()
        {
            StringReader reader = new StringReader("-1234");
            inputVar.ReadValue(reader);
            Assert.AreEqual(-1234, inputVar.Value.Actual);
            Assert.AreEqual("-1234", inputVar.Value.String);
            Assert.AreEqual(0, inputVar.Index);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ReadMethod_LeadingWhiteSpace()
        {
            StringReader reader = new StringReader(" \t -1234");
            inputVar.ReadValue(reader);
            Assert.AreEqual(-1234, inputVar.Value.Actual);
            Assert.AreEqual("-1234", inputVar.Value.String);
            Assert.AreEqual(3, inputVar.Index);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ReadMethod_TrailingWhiteSpace()
        {
            StringReader reader = new StringReader("-1234 \n ");
            inputVar.ReadValue(reader);
            Assert.AreEqual(-1234, inputVar.Value.Actual);
            Assert.AreEqual("-1234", inputVar.Value.String);
            Assert.AreEqual(0, inputVar.Index);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ReadMethod_NumWithWhiteSpace()
        {
            StringReader reader = new StringReader(" \t -1,234 \n ");
            inputVar.ReadValue(reader);
            Assert.AreEqual(-1234, inputVar.Value.Actual);
            Assert.AreEqual("-1,234", inputVar.Value.String);
            Assert.AreEqual(3, inputVar.Index);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ReadMethod_Uni()
        {
            StringReader reader = new StringReader("uni");
            inputVar.ReadValue(reader);
            Assert.AreEqual(EffectiveSeedDist.Universal, inputVar.Value.Actual);
            Assert.AreEqual("uni", inputVar.Value.String);
            Assert.AreEqual(0, inputVar.Index);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ReadMethod_UniWithWhitespace()
        {
            StringReader reader = new StringReader(" \t uni \n ");
            inputVar.ReadValue(reader);
            Assert.AreEqual(EffectiveSeedDist.Universal, inputVar.Value.Actual);
            Assert.AreEqual("uni", inputVar.Value.String);
            Assert.AreEqual(3, inputVar.Index);
        }

        //---------------------------------------------------------------------

        private void TryRead(string input)
        {
            try {
                StringReader reader = new StringReader(input);
                inputVar.ReadValue(reader);
            }
            catch (InputVariableException exc) {
                Data.Output.WriteLine(exc.Message);
                Data.Output.WriteLine();
                Assert.AreEqual((InputVariable)inputVar, exc.Variable);
                throw;
            }
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(InputVariableException))]
        public void ReadMethod_Bad()
        {
            TryRead(" \t 2+2 \n ");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(InputVariableException))]
        public void ReadMethod_TooLarge()
        {
            TryRead(" \t 9,999,999,999 \n ");
        }
    }
}
