using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    public class Response
    {
        public DateTime receivedDate { get; set; }
        public int messageLength { get; set; }
        public string secondCharacter { get; set; }
        public bool messageContainCapital { get; set; }
        public int messageCapitalCount { get; set; }
        public bool messageContainNumber { get; set; }
        public ICollection<int> messageNumberList { get; set; }
        public string message { get; set; }

        public override string ToString()
        {
            return $"{this.receivedDate} -> lenght: {this.messageLength}; capital: {this.messageContainCapital}; numbers: {this.messageContainNumber}";
        }
    }
    public static class ResponseBuilder
    {
        public static Response CreateResponse(byte[] content)
        {
            if(content.Length == 0) return null;
            string message = System.Text.Encoding.UTF8.GetString(content).Replace("\0", "");
            return new Response
            {
                receivedDate = DateTime.Now,
                messageLength = message.Length,
                secondCharacter = (message.Length >= 2) ? message.Substring(1, 1) : String.Empty,
                messageContainCapital = message.Where(c => Char.IsUpper(c)).Count() > 0,
                messageCapitalCount = message.Where(c => Char.IsUpper(c)).Count(),
                messageContainNumber = message.Where(c => Char.IsNumber(c)).Count() > 0,
                messageNumberList = message.Where(c => Char.IsNumber(c)).Select(c => Convert.ToInt32(c)).OrderBy(i => i).ToList<int>(),
                message = message
            };
        }

        public static byte[] Encode(Response response)
        {
            string stJson = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            return System.Text.Encoding.UTF8.GetBytes(stJson);
        }
        public static Response Decode(byte[] stream)
        {
            string stJson = System.Text.Encoding.UTF8.GetString(stream);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(stJson);
        }
    }
}
