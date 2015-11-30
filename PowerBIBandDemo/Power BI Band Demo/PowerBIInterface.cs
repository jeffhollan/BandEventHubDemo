using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Web.Http;
using System.Net;

namespace Sensors
{
	class PowerBIInterface
	{

     
        string sas;
        private string serviceNamespace = "HARDCODED_NAMESPACE_HERE";
        private string key = @"HARDCODED_KEY_HERE";
        private string keyname = @"HARDCODED_KEY_NAME_HERE";
        private string hubName = "HARDCODED_EVENTHUB_NAME_HERE";

		private string publisherId = "WindowsPhone";
        private string customerId = "default";

        private Uri uri;
       
        /// <summary>
        /// Helper function to get SAS token for connecting to Azure Event Hub
        /// </summary>
        /// <returns></returns>
        private string SASTokenHelper()
        {
            int expiry = (int)DateTime.UtcNow.AddMinutes(99999).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            string stringToSign = WebUtility.UrlEncode(this.uri.ToString()) + "\n" + expiry.ToString();
            string signature = HmacSha256(key, stringToSign);
            string token = String.Format("sr={0}&sig={1}&se={2}&skn={3}", WebUtility.UrlEncode(this.uri.ToString()), WebUtility.UrlEncode(signature), expiry, keyname);

            return token;
        }

        /// <summary>
        /// Because Windows.Security.Cryptography.Core.MacAlgorithmNames.HmacSha256 doesn't
        /// exist in WP8.1 context we need to do another implementation
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string HmacSha256(string key, string value)
        {
            var keyStrm = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            var valueStrm = CryptographicBuffer.ConvertStringToBinary(value, BinaryStringEncoding.Utf8);

            var objMacProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
            var hash = objMacProv.CreateHash(keyStrm);
            hash.Append(valueStrm);

            return CryptographicBuffer.EncodeToBase64String(hash.GetValueAndReset());
        }

      

        public PowerBIInterface()
		{
           
		}


        public void setVariables(string _publisher, string _key, string _keyname, string _hubname, string _namespace)
        {
            if (_key != "")
         { 
                key = _key;
            keyname = _keyname;
            hubName = _hubname;
            serviceNamespace = _namespace;
        }

			if (_publisher != "")

            { 
				customerId = _publisher;
                publisherId = _publisher.ToLower();

             //   if (_publisher == "Josh")
               //     sas = JoshSAS;
             //   else if (_publisher == "Lukasz")
              //      sas = LukaszSAS;
                //
            }
            

        //    Debug.WriteLine("Name: {0} \n Hub: {1} \n Publish: {2} \n SAS: {3}", serviceNamespace, hubName, publisherId, sas);

		}

		private void responseCheck(string responseData)
		{
			if (responseData.Equals("Created"))
				MainPage.successfulCalls++;
		}

		async internal void CreateRow(Vital toSerialize)
		{
            try {
                toSerialize.DeviceId = publisherId;
                toSerialize.CustomerId = customerId;

                //	var eventHubUrl = string.Format("{0}/publishers/{1}/messages", hubName, publisherId);



                //	var baseAddress = new Uri(string.Format("https://{0}.servicebus.windows.net/", serviceNamespace));

                using (var httpClient = new HttpClient())
                {
                    //   string sas = sas1 + publisherId + sas2;
                    this.uri = new Uri("https://" + serviceNamespace + ".servicebus.windows.net/" + hubName +
                                     "/publishers/" + publisherId + "/messages");
                    this.sas = SASTokenHelper();
                    httpClient.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("SharedAccessSignature", sas);
                    //   httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sas);
                    //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                    Debug.WriteLine("The JSON I'm Sending: " + jObjects.Serialize(toSerialize));
                    using (var content = new HttpStringContent(jObjects.Serialize(toSerialize)))
                    {

                        content.Headers.ContentType.MediaType = "application/json";

                        using (var response = await httpClient.PostAsync(uri, content))

                        {
                            string responseData = response.StatusCode.ToString();
                            Debug.WriteLine("CREATE ROW response code: {0}", responseData);
                            responseCheck(responseData);

                        }
                    }
                }
            }
            catch(Exception ex)
            {
                
            }
		}

		//**OLD POWER BI INTERFACES I DONT NEED FOR EVENT HUB **//
		//public async void PostRequest(HttpWebRequest webRequest, string json)
		//{

		//	//webRequest.Headers["Authorization"] = "Bearer
		//	byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);

		//	using (Stream writer = await webRequest.GetRequestStreamAsync())
		//	{
		//		writer.Write(byteArray, 0, byteArray.Length);
		//	}

		//	var response = await webRequest.GetResponseAsync();


		//}


		//public HttpWebRequest DatsetRequest(string datasetsUri, string method, string authorizationToken)
		//{
		//	HttpWebRequest request = System.Net.WebRequest.Create(datasetsUri) as System.Net.HttpWebRequest;
		//	//request.KeepAlive = true;
		//	request.Method = method;
		//	//request.ContentLength = 0;
		//	request.ContentType = "application/json";

		//	//Add an Authorization header to an HttpWebRequest object
		//	request.Headers["Authorization"] = String.Format("Bearer {0}", authorizationToken);



		//	return request;
		//}

		//public async Task<string> GetResponse(HttpWebRequest request)
		//{
		//	string response = string.Empty;

		//	using (HttpWebResponse httpResponse = await request.GetResponseAsync() as System.Net.HttpWebResponse)
		//	{
		//		//Get StreamReader that holds the response stream
		//		using (StreamReader reader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
		//		{
		//			response = reader.ReadToEnd();
		//		}
		//	}

		//	return response;
		//}

		//public async Task<string> CreateDataset(string accessToken)
		//{
		//	var baseAddress = new Uri("https://api.powerbi.com/beta/myorg/");

		//	using (var httpClient = new HttpClient { BaseAddress = baseAddress })
		//	{
		//		httpClient.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", accessToken));
		//		//httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

		//		using (var content = new StringContent(jObjects.BandBigBulp))
		//		{

		//			content.Headers.ContentType.MediaType = "application/json";
		//			using (var response = await httpClient.PostAsync("datasets", content))
		//			{
		//				string responseContent = await response.Content.ReadAsStringAsync();
		//				if (response.StatusCode.ToString().Equals("Created"))
		//				{

		//					byte[] byteArray = Encoding.UTF8.GetBytes(responseContent);
		//					MemoryStream stream1 = new MemoryStream(byteArray);
		//					stream1.Position = 0;
		//					DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(DatasetMember));
		//					DatasetMember d = (DatasetMember)ser.ReadObject(stream1);

		//					Debug.WriteLine(responseContent);
		//					Debug.WriteLine("ID: {0}", d.id);


		//					return d.id;
		//				}

		//			}
		//		}
		//	}
		//	return null;
		//}

		//public async Task<string> GetDatasets(string accessToken)
		//{
		//	var baseAddress = new Uri("https://api.powerbi.com/beta/myorg/");

		//	using (var httpClient = new HttpClient { BaseAddress = baseAddress })
		//	{
		//		httpClient.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", accessToken));
		//		using (var response = await httpClient.GetAsync("datasets"))
		//		{

		//			string responseData = await response.Content.ReadAsStringAsync();
		//			Debug.WriteLine("GET DATASET response: \n{0}", responseData);
		//			Dataset datasets = new jObjects().DeserializeDataset(responseData);
		//			foreach (DatasetMember d in datasets.datasets)
		//			{
		//				if (d.name.Equals("My Vitals"))
		//				{
		//					return d.id;
		//				}
		//			}

		//		}

		//	}
		//	return null;
		//}


		//internal async Task<bool> ClearData(string accessToken, string tableId, string tableName)
		//{
		//	//Debug.WriteLine("About to clear the data with accesstoken {0}", accessToken);
		//	var baseAddress = new Uri("https://api.powerbi.com/beta/myorg/");

		//	using (var httpClient = new HttpClient { BaseAddress = baseAddress })
		//	{
		//		httpClient.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", accessToken));
		//		using (var response = await httpClient.DeleteAsync(String.Format("datasets/{0}/tables/{1}/rows", tableId, tableName)))
		//		{

		//			string responseData = response.StatusCode.ToString();
		//			return responseData.Equals("OK");
		//		}

		//	}
		//}



		//public async void CreateRow(string accessToken, string tableId, string tableName, object toSerialize)
		//{
		//	var eventHubUrl = string.Format("{0}/publishers/{1}/messages", hubName, publisherId);



		//	var baseAddress = new Uri(string.Format("https://{0}.servicebus.windows.net/", serviceNamespace));

		//	using (var httpClient = new HttpClient { BaseAddress = baseAddress })
		//	{
		//		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sas);
		//              //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
		//              Debug.WriteLine("The JSON I'm SEnding: " + jObjects.Serialize(toSerialize));
		//		using (var content = new StringContent(jObjects.Serialize(toSerialize)))
		//		{

		//			content.Headers.ContentType.MediaType = "application/json";

		//			using (var response = await httpClient.PostAsync(eventHubUrl, content))
		//			{
		//				string responseData = response.StatusCode.ToString();
		//				Debug.WriteLine("CREATE ROW response code: {0}", responseData);
		//				responseCheck(responseData);

		//			}
		//		}
		//	}
		//}

	}
}
