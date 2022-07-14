using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedInternet
{
    /// <summary>
    /// This class manages the network connection to a Member
    /// </summary>
    public class NetworkMember
    {
        /// <summary>
        /// Used to send data to the simulated internet
        /// </summary>
        public InternetManager NetManager { get; set; }
        /// <summary>
        /// Identifies the Member to the simulated internet
        /// </summary>
        public string SimulationID { get; set; }

        /// <summary>
        /// Used to recieve both simulated data and network data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="SourceIP"></param>
        public virtual void DataListener(byte[] data, string SourceIP)
        {

        }
    }
}
