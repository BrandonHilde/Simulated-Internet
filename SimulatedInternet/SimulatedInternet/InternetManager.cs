using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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

    /// <summary>
    /// Manages the simulated internet. Recieves, holds, and routes messages.
    /// </summary>
    public class InternetManager
    {
        public List<ByteStorage> Transit { get; set; }
        public List<ConnectionDetail> Details { get; set; }
        public List<DetailPackage> Identities { get; set; }
        public List<RawMessage> Messages { get; set; }
        private List<string> Log = new List<string>();

        public Random random = new Random();

        private bool running = false;
        private bool open = false;

        private object WriteLock = new object();

        public InternetManager()
        {
            Transit = new List<ByteStorage>();
            Details = new List<ConnectionDetail>();
            Identities = new List<DetailPackage>();
            Messages = new List<RawMessage>();
        }

        /// <summary>
        /// Starts the main thread for the simulated internet
        /// </summary>
        public void StartNetworkSimulation()
        {
            running = true;

            Thread th = new Thread(NetworkOpperation);
            th.IsBackground = true;
            th.Start();
        }

        /// <summary>
        /// Opperates the network in a thread loop
        /// </summary>
        private void NetworkOpperation()
        {
            while (running)
            {
                open = CompleteTransmission();

                if (open)
                {
                    Thread.Sleep(5);
                }
            }
        }

        /// <summary>
        /// Generates a Fake ip to identify the Member in the network
        /// </summary>
        /// <returns>A fake ip in string form</returns>
        public string RegisterIP()
        {
            string value = string.Empty;

            for(int x = 0; x < 3; x++)
            {
                value += random.Next(0, 256).ToString() + ".";
            }

            return  value + random.Next(0, 256).ToString();
        }

        /// <summary>
        /// Distance formula
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <returns>Returns the distance between two points (x, y) and (x1, y1) </returns>
        private double Distance(double x, double y, double x1, double y1)
        {
            return Math.Sqrt(((x - x1) * (x - x1)) + ((y - y1) * (y - y1)));
        }

        /// <summary>
        /// Gives a ping value for simulation between two Members. Location values must be set.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public int FakePing(DetailPackage p, DetailPackage p2)
        {
            return (int)Distance(p.locationX, p.locationY, p2.locationX, p2.locationY);
        }

        /// <summary>
        /// Update a detail package for a Member
        /// </summary>
        /// <param name="pack"></param>
        public void UpdateManager(DetailPackage pack)
        {
            List<ConnectionDetail> det = Details.Where(x => x.DestIP == pack.IP).ToList();

            foreach (ConnectionDetail d in det)
            {
                if (d.ConnectedMember == null) d.ConnectedMember = pack.member;
            }
        }

        /// <summary>
        /// Look up a network connection between Members
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="DestIP"></param>
        /// <returns></returns>
        public ConnectionDetail LookUpConnection(string IP, string DestIP)
        {
            return Details.Where
                (
                    x =>
                        ((x.IP == IP) &&               //this segment checks if the two ips match an existing connection
                        (x.DestIP == DestIP)) ||
                        ((x.IP == DestIP) &&
                        (x.DestIP == IP))
                ).FirstOrDefault();
        }

        /// <summary>
        /// Lookup Member information by SimulationID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public DetailPackage LookUpMember(string ID)
        {

            DetailPackage net =
                          Identities.Where(
                              x => x.member.SimulationID == ID
                          ).FirstOrDefault();

            return net;
        }

        public DetailPackage LookUpMemberByIP(string IP)
        {

            DetailPackage net =
                          Identities.Where(
                              x => x.IP == IP
                          ).FirstOrDefault();

            return net;
        }

        /// <summary>
        /// Connect member to the simulated internet and add location for calculating ping times
        /// </summary>
        /// <param name="member"></param>
        public void ConnectToInternet(NetworkMember member)
        {
            DetailPackage pack = LookUpMember(member.SimulationID);

            if (pack == null)
            {
                Identities.Add(new DetailPackage
                {
                    IP = RegisterIP(),
                    member = member,
                    locationX = random.Next(0, 700),
                    locationY = random.Next(0, 700)
                });
            }
        }

        /// <summary>
        /// Checks if the ip exists in the Network
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public bool IsConnected(string IP)
        {
            DetailPackage pack = LookUpMemberByIP(IP);

            return pack != null; // ((pack != null) == true) if it exists
        }

        /// <summary>
        /// Checks if the Connection has full information
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public bool IsCompletedDetail(ConnectionDetail detail)
        {
            if (detail == null) return false;
            if (detail.DestIP == string.Empty) return false;
            if (detail.IP == string.Empty) return false;
            if (detail.Member == null) return false;
            if (detail.ConnectedMember == null) return false;
            if (detail.ping < 0) return false;

            return true;
        }

        public void SendBytesToIP(NetworkMember member, byte[] raw, string IP, ushort Port)
        {
            lock (WriteLock)
            {
                try
                {
                    Messages.Add(new RawMessage
                    {
                        member = member,
                        raw = raw,
                        IP = IP,
                        port = Port
                    });
                }
                catch (Exception ex)
                {
                    // Console writes sometimes interfere with multi-threading slowing it down.
                    Log.Add(ex.ToString()); //logs to a list<string>
                }
            }
        }

        /// <summary>
        /// Send byte package to a specific ip and port
        /// </summary>
        /// <param name="member">The member that is sending the package</param>
        /// <param name="raw">Raw byte package</param>
        /// <param name="IP">The destination IP</param>
        /// <param name="Port">The destination port</param>
        /// <returns></returns>
        public void SendBytes(NetworkMember member, byte[] raw, string IP, ushort Port)
        {
            if (raw != null)
            {
                DetailPackage pack = LookUpMember(member.SimulationID); //look up details about the member that is sending bytes

                if (pack != null) // make sure the member is in the network
                {
                    UpdateManager(pack); // update  the member's information

                    if (!IsConnected(IP))
                    {
                        ConnectToInternet(member); // connect the member to the simulated internet
                    }
                }
                else
                {
                    ConnectToInternet(member); // connect the member to the simulated internet
                }


                if (pack != null)
                {
                    if (IsConnected(IP))
                    {
                        ConnectionDetail detail = LookUpConnection(IP, pack.IP); // get the connection details for the purpose of assessing ping time

                        if (detail != null)
                        {
                            if (detail.IP == pack.IP)
                            {
                                if (detail.Member == null) detail.Member = member; // update pack info if needed
                            }
                            else
                            {
                                if (detail.ConnectedMember == null) detail.ConnectedMember = member; // update pack info if needed
                            }

                            if (pack.IP != IP) // ban sending to self
                            {
                                // add necesary information to the transit list for simulating ping time
                                Transit.Add(new ByteStorage
                                {
                                    DestinationIP = IP,
                                    bytes = raw,
                                    OriginIP = pack.IP,
                                    Port = Port,
                                    Ping = detail.ping,
                                    TimeStamp = DateTime.UtcNow
                                });
                            }
                        }
                        else // run the else statement if the necesary details for transmission are lacking
                        {
                            DetailPackage packDest = LookUpMemberByIP(IP);

                            if (packDest != null) // add details if necesary
                            {
                                Details.Add(new ConnectionDetail
                                {
                                    DestIP = IP,
                                    IP = pack.IP,
                                    Member = member,
                                    ConnectedMember = null,
                                    ping = FakePing(pack, packDest)
                                });
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Send data to destination after ping time is simulated
        /// </summary>
        /// <returns></returns>
        public bool CompleteTransmission()
        {
            List<RawMessage> raws = new List<RawMessage>();

            lock (WriteLock) // lock for multi-threading
            {
                try
                {
                    for (int i = 0; i < Messages.Count; i++)
                    {
                        if (Messages[i] != null)
                        {
                            raws.Add(new RawMessage
                            {
                                IP = Messages[i].IP,
                                member = Messages[i].member,
                                port = Messages[i].port,
                                raw = Messages[i].raw
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Console writes sometimes interfere with multi-threading slowing it down.
                    Log.Add(ex.ToString()); //logs to a list<string>
                }

                Messages = new List<RawMessage>();
            }

            Thread.Sleep(1); // sleep gives multi-threading extra space (optional)

            foreach (RawMessage m in raws)
            {
                SendBytes(m.member, m.raw, m.IP, m.port);
            }

            open = false;

            List<ByteStorage> finished = Transit.Where(x => x.Completed).ToList();  //gets list of already sent data

            foreach (ByteStorage b in finished) Transit.Remove(b); // remove already sent data

            List<ByteStorage> complete = Transit.Where // gets list of data that ping time has been completed
                (
                    x => DateTime.UtcNow.Subtract(x.TimeStamp).TotalMilliseconds > x.Ping 
                    && !x.Completed
                ).ToList();

            if (complete != null)
            {
                if (complete.Count > 0) // if there are data transmissions to complete
                {
                    foreach (ByteStorage bs in complete)
                    {
                        if (IsConnected(bs.DestinationIP)) // make sure there is a open connection
                        {
                            ConnectionDetail net = LookUpConnection(bs.OriginIP, bs.DestinationIP);

                            if (IsCompletedDetail(net)) // make sure necesary information is present
                            {
                                if (net.DestIP == bs.DestinationIP)
                                {
                                    net.ConnectedMember.DataListener(bs.bytes, bs.DestinationIP); // send to destination after checking connection details
                                }
                                else
                                {
                                    net.Member.DataListener(bs.bytes, bs.OriginIP); // send to destination after checking connection details
                                }

                                bs.Completed = true; // set the data as a completed message
                            }
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false; //failed to send
        }
    }

    /// <summary>
    /// Stores information about the ping difference between two members
    /// </summary>
    public class ConnectionDetail
    {
        public string IP { get; set; }
        public string DestIP { get; set; }
        public NetworkMember Member { get; set; }
        public NetworkMember ConnectedMember { get; set; }
        public int ping { get; set; }
    }

    // Stores raw data from a member and destination information
    public class RawMessage
    {
        public NetworkMember member { get; set; }
        public byte[] raw { get; set; }
        public string IP { get; set; }
        public ushort port { get; set; }
    }

    // Stores raw data during transit while ping is being simulated
    public class ByteStorage
    {
        public DateTime TimeStamp { get; set; }
        public string DestinationIP { get; set; }
        public string OriginIP { get; set; }
        public int Port { get; set; }
        public int Ping { get; set; }
        public byte[] bytes { get; set; }
        public bool Completed = false;
    }

    // Stores details about a members location and address
    public class DetailPackage
    {
        public NetworkMember member { get; set; }
        public string IP { get; set; }
        public int locationX = 0;
        public int locationY = 0;
    }
}
