using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Display;

namespace Sensors
{
	class KeepAlive
	{
		private static DisplayRequest g_DisplayRequest = null;
		public static void SendKeepAlive()
		{

			try
			{
				if (g_DisplayRequest == null)
				{
					// This call creates an instance of the displayRequest object 
					g_DisplayRequest = new DisplayRequest();
				}
			}
			catch (Exception ex)
			{
				//rootPage.NotifyUser("Error Creating Display Request: " + ex.Message, NotifyType.ErrorMessage);
			}

			if (g_DisplayRequest != null)
			{
				try
				{
					// This call activates a display-required request. If successful,  
					// the screen is guaranteed not to turn off automatically due to user inactivity. 
					g_DisplayRequest.RequestActive();
					//drCount += 1;
					Debug.WriteLine("Display request activated");
				}
				catch (Exception ex)
				{
					//rootPage.NotifyUser("Error: " + ex.Message, NotifyType.ErrorMessage);
				}
			}

		}
	}
}
