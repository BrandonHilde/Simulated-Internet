using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace SimulatedInternet
{
    public static class Extensions
    {
        public static void Save(this NetworkLogSaveState NetworkState)
        {
            try
            {
                string file = DateTime.Now.ToShortTimeString() + ".xml";

                var serializer = new XmlSerializer(NetworkState.GetType());

                using (var writer = XmlWriter.Create(file))
                {
                    serializer.Serialize(writer, NetworkState);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public static NetworkLogSaveState Load(string file)
        {
            NetworkLogSaveState state = null;

            if (File.Exists(file))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(NetworkLogSaveState));

                using (Stream reader = new FileStream(file, FileMode.Open))
                {
                    state = (NetworkLogSaveState)serializer.Deserialize(reader);
                }
            }

            return state;
        }

        //public static User Load(string file)
        //{
        //    User bt = null;

//            if (File.Exists(file))
//            {
//                XmlSerializer serializer = new XmlSerializer(typeof(User));

//                using (Stream reader = new FileStream(file, FileMode.Open))
//                {
//                    bt = (User) serializer.Deserialize(reader);
//}

//if (bt != null)
//    bt.filePath = file;
//            }

//            return bt;
        //}

        //public bool Save(string file = null)
        //{
        //    User temp = this;

        //    if (!File.Exists(file + ".swoblUser"))
        //    {
        //        if (file == null) file = this.Name + ".swoblUser";

        //        filePath = file;

//                try
//                {
//                    var serializer = new XmlSerializer(temp.GetType());

//                    using (var writer = XmlWriter.Create(file))
//                    {
//                        serializer.Serialize(writer, temp);
//                    }
//                }
//                catch (Exception ex)
//{
//    return false;
//}
        //    }

        //    return true;
        //}
    }
}
