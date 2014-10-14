using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Lab1Serialization
{
    class BinaryFormatterPayloadSerializer: IPayloadSerializer
    {


        public byte[] Serialize(PayloadDto dto)
        {
            // tutaj wrzucamy kod serializacji
            using (var stream = new MemoryStream())
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(stream, dto);
                return stream.GetBuffer();
            }
        }

        public PayloadDto Deserialize(byte[] data)
        {
            // tutaj wrzucamy kod deserializacji
            using (var stream = new MemoryStream(data))
            {
                var serializer = new BinaryFormatter();
                return serializer.Deserialize(stream) as PayloadDto;
            }
        }

    }
}
