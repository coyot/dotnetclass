using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1Serialization
{
    [ProtoContract]
    [Serializable]
    public class PayloadDto
    {
        [ProtoMember(1)]
        public int Number { get; set; }

        [ProtoMember(2)]
        public string Text { get; set; }

        [ProtoMember(3)]
        public DateTime TimeStamp { get; set; }

        [ProtoMember(4)]
        public TimeSpan Span { get; set; }

        [ProtoMember(5)]
        public int[] List { get; set; }

        [ProtoMember(6)]
        public Dictionary<int, string> Dict { get; set; }

        public PayloadDto()
        {

        }

        public static PayloadDto Create()
        {
            var dto = new PayloadDto();
            Random r = new Random();
            dto.Number = r.Next() % 1000;
            dto.Text = "str" + (r.Next()% 100).ToString();
            dto.TimeStamp = DateTime.Now;
            dto.List = Enumerable.Range(0, 3).Select(o => r.Next() % 100).ToArray();
            dto.Dict = Enumerable.Range('a', 3).ToDictionary(o => (int)o, o => ((char)o).ToString());
            dto.Span = TimeSpan.FromMilliseconds(r.NextDouble() * 1000);
            return dto;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2:d/M/yyyy HH:mm:ss.ffff} {3} [{4}] [{5}]", 
                Number, Text, TimeStamp, Span.TotalMilliseconds,
                string.Join(",", List), 
                string.Join(";", Dict.Select(o => string.Format("{0},{1}", o.Key, o.Value))));
        }

    }
}
