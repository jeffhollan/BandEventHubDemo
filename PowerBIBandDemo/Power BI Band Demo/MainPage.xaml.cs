/*
    Copyright (c) Microsoft Corporation All rights reserved.  
 
    MIT License: 
 
    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
    documentation files (the  "Software"), to deal in the Software without restriction, including without limitation
    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
    and to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
 
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
 
    THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using Microsoft.Band;
using Microsoft.Band.Sensors;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.Web.Http;

namespace Sensors
{
    public sealed partial class MainPage : Page, IWebAuthenticationContinuable
    {


        public static int totalCalls = 0;
        public static int successfulCalls = 0;

        private IBandInfo[] pairedBands;
        private IBandClient bandClient;
        bool firstStep = true;
        private long startSteps;


        private string tableId = null;
        private PowerBIInterface pbi = new PowerBIInterface();
        private string accessToken = String.Empty;

        private static string currentWearStatus = "Worn";

       

        private Uri redirectU = null;

        //Client app ID 
        private const string clientID = "fa523afa-86f2-4a89-8a55-e6943c4c3f15";

        private Vital currentVitals = new Vital();

		private DateTime startTime;


		public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

			//authContext = AuthenticationContext.CreateAsync(authority).GetResults();


			redirectU = Windows.Security.Authentication.Web.WebAuthenticationBroker.GetCurrentApplicationCallbackUri();

			this.Loaded += MainPage_Loaded;


			

		}

		private async void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
            //Delay 500 miliseconds so the UI has time to load before authentication pops up

            //await Task.Delay(500);
            ////if (accessToken.Equals(String.Empty))
            ////	GetToken();

            accessToken = "FILLER";
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if(localSettings.Values.ContainsKey("namespace") && localSettings.Values.ContainsKey("key") && localSettings.Values.ContainsKey("keyname") && localSettings.Values.ContainsKey("hubname") && localSettings.Values.ContainsKey("publisher"))
            {
                Namespace.Text = localSettings.Values["namespace"] as string;
                Key.Text = localSettings.Values["key"] as string;
                KeyName.Text = localSettings.Values["keyname"] as string;
                HubName.Text = localSettings.Values["hubname"] as string;
                Publisher.Text = localSettings.Values["publisher"] as string;
            }
        }

		private async void GetToken()
		{
			// Try to get a token without triggering any user prompt. 
			// ADAL will check whether the requested token wis in the cache or can be obtained without user itneraction (e.g. via a refresh token).
			//AuthenticationResult result = await authContext.AcquireTokenSilentAsync(resourceUri, clientID);
			//if (result != null && result.Status == AuthenticationStatus.Success)
			//{
			//	// A token was successfully retrieved. Get the To Do list for the current user
			//	//DisplayToken(result);
			//}
			//else
			//{
			//	try
			//	{
			//		//authContext.TokenCache.Clear();

			//		// Acquiring a token without user interaction was not possible. 
			//		// Trigger an authentication experience and specify that once a token has been obtained the DisplayToken method should be called
			//		//authContext.AcquireTokenAndContinue(resourceUri, clientID, redirectU, DisplayToken);
			//	}
			//	catch (Exception ex)
			//	{
			//		Debug.WriteLine(ex);
			//	}
			//}

			
		}


		/// <summary>
		/// Method to store the access token from Azure AD once it gets it
		/// </summary>
		/// <param name="result"></param>
		//private void DisplayToken(string result)
		//{
		//	Debug.WriteLine("Got Token: {0}", result.AccessToken);
		//	accessToken = result.AccessToken;
		//}
		///// <summary>
		/// Connect to Microsoft Band and read Accelerometer data.
		/// </summary>
		private async void Button_Click(object sender, RoutedEventArgs e)
		{

           

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["publisher"] = Publisher.Text;
            localSettings.Values["key"] = Key.Text;
            localSettings.Values["keyname"] = KeyName.Text;
            localSettings.Values["hubname"] = HubName.Text;
            localSettings.Values["namespace"] = Namespace.Text;
            startTime = DateTime.Now;
            pbi.setVariables(Publisher.Text, Key.Text, KeyName.Text, HubName.Text, Namespace.Text);


            //Tell the screen not to fall asleep
            KeepAlive.SendKeepAlive();

			//Show the loading prompt
			LoadingBar.IsEnabled = true;
			LoadingBar.Visibility = Visibility.Visible;
			FadeBorder.Visibility = Visibility.Visible;
			LoadingText.Text = "Connecting to Power BI and Band....";
			LoadingText.Visibility = Visibility.Visible;



			//If you don't know what the tableID is for the users Band tables, then go find it.
			if (tableId == null)
			{
				await GetTableID();
			}

			//If you don't have the table ID or the access token, skip all the running stuff
			if (accessToken != String.Empty && tableId != null)
			{
               

                    this.generalText.Text = "Connecting to band....";
				LoadingText.Text = "Connecting to band....";
				pairedBands = await BandClientManager.Instance.GetBandsAsync();
				if (pairedBands.Length < 1)
				{
					this.LoadingText.Text = "This sample app requires a Microsoft Band paired to your phone. Also make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";
					return;
				}

				//Connect to the band
				bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]);
				
				this.generalText.Text = "Connected to band";

				LoadingBar.IsEnabled = false;
				LoadingBar.Visibility = Visibility.Collapsed;
				LoadingText.Visibility = Visibility.Collapsed;
				LoadingText.Text = "Clearing Microsoft Band Tables....";
				FadeBorder.Visibility = Visibility.Collapsed;
				
				this.StopButton.Visibility = Visibility.Visible;

				this.packetText.Text = String.Format("Packet success rate: {0} / {1}", successfulCalls.ToString(), totalCalls.ToString());
				packetText.Visibility = Visibility.Visible;

                // check current user heart rate consent 
                if (bandClient.SensorManager.HeartRate.GetCurrentUserConsent() != UserConsent.Granted)

                {  // user has not consented, request it  
                    await bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
                }


                //Tell the band to listen to these sensors
                TemperatureListen();
                HeartRateListen();
                PedometerListen();
                WornListen();
                DistanceListen();
                AccelListen();
                GyroListen();
            }
        }

		/// <summary>
		/// Attempt to get the datasets for the user.  If they don't have the My Vitals dataset, prompt them to create one
		/// </summary>
		/// <returns></returns>
		private async Task GetTableID()
		{
			string answer = "";
			//keeping this so I don't have to redifine the logic.
			tableId = "FILLER";
			//tableId = await pbi.GetDatasets(accessToken);
			if (tableId == null)
			{
				var dialog = new Windows.UI.Popups.MessageDialog("You do not yet have tables in your Power BI Dashboard for Microsoft Band data.  Can we create this dataset for you?", "Table Not Found");
				dialog.Commands.Add(new UICommand("Yes", (s) => { answer = "yes"; }));
				dialog.Commands.Add(new UICommand("No"));

				await dialog.ShowAsync();
				if (answer == "yes")
				{
					await CreateDataset();
				}

			}
		}

		/// <summary>
		/// Set a listener to changes in temperature from the band.  Start reading
		/// </summary>
		private async void TemperatureListen()
		{

			//subscribe to skin temperature

			bandClient.SensorManager.SkinTemperature.ReadingChanged += SkinTemperature_ReadingChanged;
            await bandClient.SensorManager.SkinTemperature.StartReadingsAsync();

		}

        private async void AccelListen()
        {

            //subscribe to skin temperature

            bandClient.SensorManager.Accelerometer.ReadingChanged += Accel_ReadingChanged;
            await bandClient.SensorManager.Accelerometer.StartReadingsAsync();

        }

        private void Accel_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandAccelerometerReading> e)
        {
            try
            {
                if (e.SensorReading != null)
                {
                    IBandAccelerometerReading acc = e.SensorReading;
                    currentVitals.accelX = acc.AccelerationX;
                    currentVitals.accelY = acc.AccelerationY;
                    currentVitals.accelZ = acc.AccelerationZ;

                }
            }
            catch (Exception ex)
            {
            }
        }

        private async void GyroListen()
        {

            //subscribe to skin temperature

            bandClient.SensorManager.Gyroscope.ReadingChanged += Gyro_ReadingChanged;
            await bandClient.SensorManager.Gyroscope.StartReadingsAsync();

        }

        private void Gyro_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandGyroscopeReading> e)
        {
            try
            {
                if (e.SensorReading != null)
                {
                    IBandGyroscopeReading gyro = e.SensorReading;
                    currentVitals.gyroX = gyro.AngularVelocityX;
                    currentVitals.gyroY = gyro.AngularVelocityY;
                    currentVitals.gyroZ = gyro.AngularVelocityZ;



                }
            }
            catch (Exception ex)
            {
            }
        }

        private async void DistanceListen()
		{
			bandClient.SensorManager.Distance.ReadingChanged += Distance_ReadingChange;
			await bandClient.SensorManager.Distance.StartReadingsAsync();

		}

	

		/// <summary>
		/// Set a listener to changes in heart rate from the band.  Start reading
		/// </summary>
		private async void HeartRateListen()
		{
            bandClient.SensorManager.HeartRate.ReadingChanged += HeartRate_ReadingChanged;
          //  var timespans = bandClient.SensorManager.HeartRate.SupportedReportingIntervals;
            await bandClient.SensorManager.HeartRate.StartReadingsAsync();
        }

		/// <summary>
		/// Set a listener to changes in pedometer from the band.  Start reading
		/// </summary>
		private async void PedometerListen()
		{
			bandClient.SensorManager.Pedometer.ReadingChanged += Pedometer_ReadingChanged;
			await bandClient.SensorManager.Pedometer.StartReadingsAsync();
		}

		/// <summary>
		/// Set a listener to changes in contact in the band (worn or not worn), start reading
		/// </summary>
		private async void WornListen()
		{
			bandClient.SensorManager.Contact.ReadingChanged += Contact_ReadingChanged;
			await bandClient.SensorManager.Contact.StartReadingsAsync();

		}

        public async static void ShowError()
        {
            
            var dialog = new MessageDialog("There was an exception.  Check your Namespace settings and try again.");
            await dialog.ShowAsync();
            App.Current.Exit();
        }

		/// <summary>
		/// When the heart rate changes, if it is being worn, send the heartrate up to Power BI.  If it isn't being worn, sent heart rate 0 since we know it isn't accurate.
		/// Update the UI
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void HeartRate_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandHeartRateReading> e)
		{
			
			try
			{
				if (e.SensorReading != null)
				{
					MainPage.totalCalls++;

					IBandHeartRateReading hr = e.SensorReading;

					//if (firstLog)
					//{
					//firstLog = false;
					if (!currentWearStatus.Equals("Worn"))
					{
                        //pbi.CreateRow(accessToken, tableId, "Heartrate", new HeartRate { heartrate = 0, quality = hr.Quality.ToString(), timestamp = hr.Timestamp.LocalDateTime.Add(hr.Timestamp.Offset) });
                        currentVitals.heartrate = 0;
                        currentVitals.quality = hr.Quality.ToString();
						currentVitals.eventtime = hr.Timestamp.UtcDateTime.ToString("o");
                        pbi.CreateRow(currentVitals);
						currentVitals.steps = 0;
                    }
					else
					{
                        //pbi.CreateRow(accessToken, tableId, "Heartrate", new HeartRate { heartrate = hr.HeartRate, quality = hr.Quality.ToString(), timestamp = hr.Timestamp.LocalDateTime.Add(hr.Timestamp.Offset) });
                        currentVitals.heartrate = hr.HeartRate;
                        currentVitals.quality = hr.Quality.ToString();
						currentVitals.eventtime = hr.Timestamp.UtcDateTime.ToString("o");
                        pbi.CreateRow(currentVitals);
						currentVitals.steps = 0;
                    }



					//}

					await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
					{
						this.hrTextBlock.Text = hr.HeartRate.ToString();
						this.hrqTextBlock.Text = hr.Quality.ToString();
						this.hrTimeStamp.Text = hr.Timestamp.ToString();
						this.packetText.Text = String.Format("Packet success rate: {0} / {1}", successfulCalls.ToString(), totalCalls.ToString());
						string ElapsedTime = "";
						TimeSpan duration = DateTime.Now - startTime;
						ElapsedTime += duration.Hours.ToString("00") + ":" + duration.Minutes.ToString("00") + ":" + duration.Seconds.ToString("00");
						this.runTimer.Text = ElapsedTime;
					}).AsTask();
				}
			}
			catch (Exception ex)
			{
                
				Debug.WriteLine("Error with event: {0}", ex);
			}

		}

		private async void Distance_ReadingChange(object sender, BandSensorReadingEventArgs<IBandDistanceReading> e)
		{
			try
			{
				if (e.SensorReading != null)
				{
					IBandDistanceReading dis = e.SensorReading;
                    currentVitals.speed = dis.Speed / 100;

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
					{
                        //distTextBlock.Text = dis.TotalDistance.ToString();
                        //dispTextBlock.Text = dis.Pace.ToString();
                        dissTextBlock.Text = (dis.Speed / 100).ToString();
                        
						//discmTextBlock.Text = dis.CurrentMotion.ToString();
						//disTimeStamp.Text = dis.Timestamp.ToString();
					});

				}
			}
			catch (Exception ex)
			{
			}

		}

        

		/// <summary>
		/// Just like heart rate, only skin temp
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void SkinTemperature_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandSkinTemperatureReading> e)
		{
			//MainPage.totalCalls++;

			IBandSkinTemperatureReading temp = e.SensorReading;

            //pbi.CreateRow(accessToken, tableId, "Temperature", new Temperature { temperature = (temp.Temperature * 1.8 + 32), timestamp = temp.Timestamp.LocalDateTime.Add(temp.Timestamp.Offset) });
            currentVitals.temperature = (temp.Temperature * 1.8 + 32);
            //currentVitals.timestamp = temp.Timestamp.LocalDateTime.Add(temp.Timestamp.Offset);
            //pbi.CreateRow(currentVitals);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				this.tempTextBlock.Text = temp.Temperature.ToString() + " degrees C";
				this.tempTimeStamp.Text = temp.Timestamp.ToString();
				this.packetText.Text = String.Format("Packet success rate: {0} / {1}", successfulCalls.ToString(), totalCalls.ToString());

			}).AsTask();

		}

		private long prevSteps = -1;
		private long newSteps = 0;

		/// <summary>
		/// Just like heartrate, only difference is the band returns the total steps it's EVER taken.  That's a big number, so we subtract what steps were there from the start, and only push up the net new steps
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Pedometer_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandPedometerReading> e)
		{


			IBandPedometerReading ped = e.SensorReading;
			if (firstStep)
			{
				startSteps = ped.TotalSteps;
				firstStep = false;
			}
			newSteps = ped.TotalSteps - startSteps;

			if (newSteps != prevSteps)
			{
				//MainPage.totalCalls++;
                //pbi.CreateRow(accessToken, tableId, "Pedometer", new Pedometer { steps = (int)(newSteps - prevSteps), timestamp = ped.Timestamp.LocalDateTime.Add(ped.Timestamp.Offset) });
                currentVitals.steps = (int)(newSteps - prevSteps);
                //currentVitals.timestamp = ped.Timestamp.LocalDateTime.Add(ped.Timestamp.Offset);
             //   pbi.CreateRow(currentVitals);

                prevSteps = newSteps;
			}

			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				this.pedTextBlock.Text = (ped.TotalSteps - startSteps).ToString();
				this.pedTimeStamp.Text = ped.Timestamp.ToString();
				this.packetText.Text = String.Format("Packet success rate: {0} / {1}", successfulCalls.ToString(), totalCalls.ToString());
			}).AsTask();

		}

		/// <summary>
		/// If the contact status changes, push it up to Power BI and update the UI
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Contact_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandContactReading> e)
		{
			//MainPage.totalCalls++;

			IBandContactReading status = e.SensorReading;

            //pbi.CreateRow(accessToken, tableId, "Worn", new Worn { status = status.State.ToString(), timestamp = status.Timestamp.LocalDateTime.Add(status.Timestamp.Offset) });
            currentVitals.status = status.State.ToString();
            //currentVitals.timestamp = status.Timestamp.LocalDateTime.Add(status.Timestamp.Offset);
       //     pbi.CreateRow(currentVitals);

            if (!status.State.ToString().Equals(currentWearStatus))
			{
				currentWearStatus = status.State.ToString();
			}

			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				this.statusTextBlock.Text = status.State.ToString();
				this.statusTimeStamp.Text = status.Timestamp.ToString();
				this.packetText.Text = String.Format("Packet success rate: {0} / {1}", successfulCalls.ToString(), totalCalls.ToString());
			}).AsTask();

		}

		/// <summary>
		/// This is the "CLEAR TABLES" button.  Send the clear data protocol to power BI for the 4 sensor tables we have provisioned.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//private async void Button_Click_1(object sender, RoutedEventArgs e)
		//{



		//	LoadingBar.IsEnabled = true;
		//	LoadingBar.Visibility = Visibility.Visible;
		//	FadeBorder.Visibility = Visibility.Visible;
		//	LoadingText.Visibility = Visibility.Visible;

		//	//if (accessToken.Equals(String.Empty))
		//	//	await GetToken();

		//	if (tableId == null)
		//	{
		//		await GetTableID();
		//	}

		//	string message = "Tables Cleared Successfully!";

		//	if (!await pbi.ClearData(accessToken, tableId, "Heartrate") || !await pbi.ClearData(accessToken, tableId, "Pedometer") || !await pbi.ClearData(accessToken, tableId, "Temperature") || !await pbi.ClearData(accessToken, tableId, "Worn"))
		//		message = "Something went wrong, please try again.";

		//	LoadingBar.IsEnabled = false;
		//	LoadingBar.Visibility = Visibility.Collapsed;
		//	LoadingText.Visibility = Visibility.Collapsed;

		//	totalCalls = 0;
		//	successfulCalls = 0;
		//	packetText.Visibility = Visibility.Collapsed;

		//	await new MessageDialog(message, "Information").ShowAsync();

		//	FadeBorder.Visibility = Visibility.Collapsed;
		//}




		private async void Disconnect(object sender, RoutedEventArgs e)
		{

			LoadingBar.IsEnabled = true;
			LoadingBar.Visibility = Visibility.Visible;
			FadeBorder.Visibility = Visibility.Visible;



			bandClient.SensorManager.HeartRate.ReadingChanged -= HeartRate_ReadingChanged;
			await bandClient.SensorManager.HeartRate.StopReadingsAsync();

			bandClient.SensorManager.Pedometer.ReadingChanged -= Pedometer_ReadingChanged;
			await bandClient.SensorManager.Pedometer.StopReadingsAsync();

			bandClient.SensorManager.SkinTemperature.ReadingChanged -= SkinTemperature_ReadingChanged;
			await bandClient.SensorManager.SkinTemperature.StopReadingsAsync();

			bandClient.SensorManager.Contact.ReadingChanged -= Contact_ReadingChanged;
			await bandClient.SensorManager.Contact.StopReadingsAsync();
			
			bandClient.SensorManager.Distance.ReadingChanged -= Distance_ReadingChange;
			await bandClient.SensorManager.Distance.StopReadingsAsync();

			bandClient.Dispose();

			LoadingBar.IsEnabled = false;
			LoadingBar.Visibility = Visibility.Collapsed;

			this.generalText.Text = "Disconnected";
			this.StopButton.Visibility = Visibility.Collapsed;

			await new MessageDialog("Live relay disconnected", "Information").ShowAsync();



			FadeBorder.Visibility = Visibility.Collapsed;
		}


		private async Task CreateDataset()
		{


			//tableId = await pbi.CreateDataset(accessToken);
			tableId = "FILLER";
			if (tableId == null)
			{
				await new MessageDialog("Error creating dataset, please try again", "Information").ShowAsync();
			}

			LoadingBar.IsEnabled = false;
			LoadingBar.Visibility = Visibility.Collapsed;
			LoadingText.Visibility = Visibility.Collapsed;
			LoadingText.Text = "Clearing Microsoft Band Tables....";

			totalCalls = 0;
			successfulCalls = 0;


			FadeBorder.Visibility = Visibility.Collapsed;
		}

		/// <summary>
		/// Needed for Azure Active Directory to work with Windows Phone. 
		/// </summary>
		/// <param name="args"></param>

		#region IWebAuthenticationContinuable implementation
		public async void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
		{
			Debug.WriteLine("Continuation Working.");
			//await authContext.ContinueAcquireTokenAsync(args);
		}


        #endregion

        private void SettingClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
