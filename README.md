# Simulated-Internet
This project lets you simulate internet so you can test network applications and blockchain systems.

Code Examples:

```

  public void CreateSimulation()
  {
    InternetManager testInternet = new InternetManager();

    List<NetworkMember> members = GenerateNetworks(); // you need to create a method for creating your network members

    testInternet.StartNetworkSimulation();
    foreach (NetworkMember net in members)
    {
        net.SimulatedInternet = testInternet; 
        // you will need to add code to start your members function
    }   
  }
   
   /////////////////////////////////////////////////////////////////////////////////////////////////////////////
   
  // How To Set Up Your Network Application Class
  public class NetworkApplication:NetworkMember
  {
    public enum NetworkType { Live = 22222, Simulated = 1337 };
    
    public NetworkType Network = NetworkType.Simulated;
    
    public InternetManager SimulatedInternet { get; set; }
    
    public override void DataListener(byte[] data, string SourceIP)
    {
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
