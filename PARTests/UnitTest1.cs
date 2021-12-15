using Microsoft.VisualStudio.TestTools.UnitTesting;
using StandardLibrary;

namespace PARTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestEmptyMessage()
        {
            byte[] message = new byte[] { };
            Response response = ResponseBuilder.CreateResponse(message);
            Assert.IsNull(response);
        }
        [TestMethod]
        public void TestLengthMessage()
        {
            string message = "a";
            byte[] bytesMessage = System.Text.Encoding.UTF8.GetBytes(message);
            Response response = ResponseBuilder.CreateResponse(bytesMessage);
            Assert.AreEqual(response.messageLength, message.Length);

            message = "aa";
            bytesMessage = System.Text.Encoding.UTF8.GetBytes(message);
            response = ResponseBuilder.CreateResponse(bytesMessage);
            Assert.AreEqual(response.messageLength, message.Length);
        }
        [TestMethod]
        public void TestNotSecondCharacter()
        {
            string message = "a";
            byte[] bytesMessage = System.Text.Encoding.UTF8.GetBytes(message);
            Response response = ResponseBuilder.CreateResponse(bytesMessage);
            Assert.AreEqual(response.secondCharacter, string.Empty);
        }
        [TestMethod]
        public void TestSecondCharacter()
        {
            string message = "ab";
            byte[] bytesMessage = System.Text.Encoding.UTF8.GetBytes(message);
            Response response = ResponseBuilder.CreateResponse(bytesMessage);
            Assert.AreEqual(response.secondCharacter, "b");
        }
        [TestMethod]
        public void TestSecondCharacterCapital()
        {
            string message = "aA";
            byte[] bytesMessage = System.Text.Encoding.UTF8.GetBytes(message);
            Response response = ResponseBuilder.CreateResponse(bytesMessage);
            Assert.AreEqual(response.secondCharacter, "A");
        }
        [TestMethod]
        public void TestSecondCharacterNumberic()
        {
            string message = "a1";
            byte[] bytesMessage = System.Text.Encoding.UTF8.GetBytes(message);
            Response response = ResponseBuilder.CreateResponse(bytesMessage);
            Assert.AreEqual(response.secondCharacter, "1");
        }
        [TestMethod]
        public void TestNotCapital()
        {
            string message = "a";
            byte[] bytesMessage = System.Text.Encoding.UTF8.GetBytes(message);
            Response response = ResponseBuilder.CreateResponse(bytesMessage);
            Assert.IsFalse(response.messageContainCapital);
            Assert.AreEqual(response.messageCapitalCount, 0);
        }
        [TestMethod]
        public void TestCapital() { 
            string message = "aA";
            byte[] bytesMessage = System.Text.Encoding.UTF8.GetBytes(message);
            Response response = ResponseBuilder.CreateResponse(bytesMessage);
            Assert.IsTrue(response.messageContainCapital);
            Assert.AreEqual(response.messageCapitalCount, 1);
        }

        [TestMethod]
        public void TestEncodingUTF8()
        {
            string message = "abCdeF 12 Ñh";
            byte[] bytesMessage = System.Text.Encoding.ASCII.GetBytes(message);
            Response response = ResponseBuilder.CreateResponse(bytesMessage);
            Assert.AreNotEqual(response.messageCapitalCount, 3);
        }
    }
}