using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedInternet
{

    /// <summary>
    /// Stores information about the ping difference between two members
    /// </summary>
    public class ConnectionDetail
    {
        public string IP { get; set; }
        public string DestIP { get; set; }
        public NetworkMember Member { get; set; }
        public NetworkMember ConnectedMember { get; set; }
        public int Ping { get; set; }
    }

    /// <summary>
    /// Stores raw data from a member and destination information
    /// </summary>
    public class RawMessage
    {
        public NetworkMember Member { get; set; }
        public byte[] Raw { get; set; }
        public string IP { get; set; }
        public ushort Port { get; set; }
    }

    /// <summary>
    ///  Stores raw data during transit while ping is being simulated
    /// </summary>
    public class ByteStorage
    {
        public DateTime TimeStamp { get; set; }
        public string DestinationIP { get; set; }
        public string OriginIP { get; set; }
        public int Port { get; set; }
        public int Ping { get; set; }
        public byte[] Bytes { get; set; }
        public bool Completed = false;
    }

    /// <summary>
    /// Stores details about a members location and address
    /// </summary>
    public class DetailPackage
    {
        public NetworkMember Member { get; set; }
        public string IP { get; set; }
        public int locationX = 0;
        public int locationY = 0;
    }

    /// <summary>
    /// Allows you to save a network timeline
    /// </summary>
    public class NetworkLogSaveState
    {
        public List<DetailPackage> Members { get; set; }
        public List<ByteStorage> Data { get; set; }

        
    }
}
