using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;


namespace Sensors
{
	class jObjects
	{
		public jObjects()
		{
		}

		
		
	
		internal static string Serialize(object toSerialze)
		{
			//Rows r = new Rows
			//{
			//	rows = (Data) toSerialze
			//};
			MemoryStream stream1 = new MemoryStream();
			DataContractJsonSerializer ser = new DataContractJsonSerializer(toSerialze.GetType());
			ser.WriteObject(stream1, toSerialze);
			stream1.Position = 0;
			StreamReader sr = new StreamReader(stream1);

			string toRet = sr.ReadToEnd();
			//string modToRet = toRet.Insert(0, "{\n  \"rows\": [\n    ");
			//modToRet += " \n   ]\n}";
            //Debug.WriteLine("JSON-ized: \n{0}", modToRet);
			return toRet;
		}

		
	}



	[DataContract]
	public class Vital
	{
		[DataMember]
		public string CustomerId { get; set; }
		[DataMember]
		public string DeviceId { get; set; }
		[DataMember]
		public int heartrate { get; set; }
		[DataMember]
		public string quality { get; set; }

        [DataMember]
        public int steps { get; set; }

        [DataMember]
        public string status { get; set; }

        [DataMember]
        public double temperature { get; set; }
		[DataMember]
		public string eventtime { get; set; }
	}



}
