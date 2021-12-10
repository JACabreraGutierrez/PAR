using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Response
    {
        public DateTime receivedDate { get; set; }
        public int messageLength { get; set; }
        public string secondCharacter { get; set; }
        public bool messageContainCapital { get; set; }
        public int messageCapitalCount { get; set; }
        public bool messageContainNumber { get; set; }
        public int messageNumberCount { get; set; }
    }
    public static class ResponseBuilder
    {
        public static Response CreateResponse(string message)
        {
            return new Response
            {
                receivedDate = DateTime.Now,
                messageLength = message.Length,
                secondCharacter = message.Substring(1, 1),
                messageContainCapital = message.Where(c => Char.IsUpper(c)).Count() > 0,
                messageCapitalCount = message.Where(c => Char.IsUpper(c)).Count(),
                messageContainNumber = message.Where(c => Char.IsNumber(c)).Count() > 0,
                messageNumberCount = message.Where(c => Char.IsNumber(c)).Count()
            };
        }
    }
}
