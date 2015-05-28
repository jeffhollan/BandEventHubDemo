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
        private string serviceNamespace;
        private string key;
        private string keyname;
        private string hubName;

		private string publisherId = "WindowsPhone";

        //private string customerId = Windows.Devices.Enumeration.DeviceInformation.Id
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
            key = _key;
            keyname = _keyname;
            hubName = _hubname;
            serviceNamespace = _namespace;

			if (_publisher != "")

            { 
				customerId = _publisher;
                publisherId = _publisher.ToLower();

           
            }

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

                using (var httpClient = new HttpClient())
                {
                    //   string sas = sas1 + publisherId + sas2;
                    this.uri = new Uri("https://" + serviceNamespace + ".servicebus.windows.net/" + hubName +
                                     "/publishers/" + publisherId + "/messages");
                    this.sas = SASTokenHelper();
                    httpClient.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("SharedAccessSignature", sas);

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

	}
}
