using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1Serialization
{
    interface IPayloadSerializer
    {
        byte[] Serialize(PayloadDto dto);
        PayloadDto Deserialize(byte[] data);
    }
}
