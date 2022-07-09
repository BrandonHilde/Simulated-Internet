# Simulated-Internet
This project lets you simulate internet so you can test network applications and blockchain systems.

**Project Implementation: https://github.com/BrandonHilde/Simulated-Internet-Example**

Code Examples:

```C#

  public List<NetworkMember> GenerateNetworks()
  {
      List<NetworkMember> mems = new List<NetworkMember>
      {
          new NetworkMember { SimulationID = "bob" },
          new NetworkMember { SimulationID = "fred" },
          new NetworkMember { SimulationID = "sally" },
          new NetworkMember { SimulationID = "mary" },
          new NetworkMember { SimulationID = "john" }
      };
      
      return mems;
  }

  public void CreateSimulation()
  {
      InternetManager testInternet = new InternetManager();

      List<NetworkMember> members = GenerateNetworks(); // you need to create a method for creating your network members

      foreach (NetworkMember net in members)
      {
          net.SimulatedInternet = testInternet; 
      }   

      foreach (NodeRelay net in members) testInternet.ConnectToInternet(net);

      foreach(NodeRelay net in members)
      {
          DetailPackage dp = new DetailPackage();
          foreach (string id in ids)
          {
              if (net.SimulationID != id)
              {
                  dp = testInternet.LookUpMember(id);
                  net.Peers.Add(dp.IP);
              }
          }
      }
      
      testInternet.StartNetworkSimulation();
      
      foreach(NodeRelay net in members) net.Run();     
  }
   
   /////////////////////////////////////////////////////////////////////////////////////////////////////////////
   
  // How To Set Up Your Network Application Class
  public class NetworkApplication:NetworkMember
  {
    public enum NetworkType { Live = 22222, Simulated = 1337 };
    
    public NetworkType Network = NetworkType.Simulated;
    
    public InternetManager SimulatedInternet { get; set; }
    
    private ConcurrentQueue<byte[]> dataRecieved = new ConcurrentQueue<byte[]>();
    
    private List<byte[]> RecievedBytes = new List<byte[]>();
    
    public void Run()
    {
        new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
           
            while (!dataRecieved.IsEmpty)
            {
                byte[] nextRecieved = { };
                
                if (dataRecieved.TryDequeue(out nextRecieved))
                {
                    RecievedBytes.Add(nextRecieved);
                }
            }
            
            /// add application core logic here
            
            
        }).Start();
    }
    
    public override void DataListener(byte[] data, string SourceIP)
    {
        dataRecieved.Enqueue(data);
        
        // use the same function that handles reception of data from your internet client class
        // this way you can code the applcation in the exact same way without having to build your
        // code around the simulation. If you do this you can seamlessly test your application
        // without having to change any code to swap between simulation and the live test.
    }
    
    public void SendData(byte[] data, string DestinationIP, NetworkMember Self)
    {
    
        //use this method to determine where to send the data
        if(Network == NetworkType.Simulated)
        {
            SimulatedInternet.SendBytesToIP(
                                    this,
                                    data,
                                    DestinationIP,
                                    (ushort)Network); // Live = 22222, Simulated = 1337
        }
        else
        {
            //internetClient.Send(data, DestinationIP);
        }
    }
  }
 
```
