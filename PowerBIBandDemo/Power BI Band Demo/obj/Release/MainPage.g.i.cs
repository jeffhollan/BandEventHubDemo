﻿

#pragma checksum "D:\Development\My Code\Repos\EventHubPowerBI\PowerBIBandDemo\Power BI Band Demo\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "36E35D2BD6C07CB944960A0208E20600"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sensors
{
    partial class MainPage : global::Windows.UI.Xaml.Controls.Page
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Border FadeBorder; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.ProgressBar LoadingBar; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock LoadingText; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid MyGrid; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button RunButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button StopButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock packetText; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock tempTextBlock; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock tempTimeStamp; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock hrTextBlock; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock hrqTextBlock; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock hrTimeStamp; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock pedTextBlock; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock pedTimeStamp; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock statusTextBlock; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock statusTimeStamp; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock generalText; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock runTimer; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBox Publisher; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBox Namespace; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBox Key; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBox KeyName; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBox HubName; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///MainPage.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            FadeBorder = (global::Windows.UI.Xaml.Controls.Border)this.FindName("FadeBorder");
            LoadingBar = (global::Windows.UI.Xaml.Controls.ProgressBar)this.FindName("LoadingBar");
            LoadingText = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("LoadingText");
            MyGrid = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("MyGrid");
            RunButton = (global::Windows.UI.Xaml.Controls.Button)this.FindName("RunButton");
            StopButton = (global::Windows.UI.Xaml.Controls.Button)this.FindName("StopButton");
            packetText = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("packetText");
            tempTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("tempTextBlock");
            tempTimeStamp = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("tempTimeStamp");
            hrTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("hrTextBlock");
            hrqTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("hrqTextBlock");
            hrTimeStamp = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("hrTimeStamp");
            pedTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("pedTextBlock");
            pedTimeStamp = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("pedTimeStamp");
            statusTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("statusTextBlock");
            statusTimeStamp = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("statusTimeStamp");
            generalText = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("generalText");
            runTimer = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("runTimer");
            Publisher = (global::Windows.UI.Xaml.Controls.TextBox)this.FindName("Publisher");
            Namespace = (global::Windows.UI.Xaml.Controls.TextBox)this.FindName("Namespace");
            Key = (global::Windows.UI.Xaml.Controls.TextBox)this.FindName("Key");
            KeyName = (global::Windows.UI.Xaml.Controls.TextBox)this.FindName("KeyName");
            HubName = (global::Windows.UI.Xaml.Controls.TextBox)this.FindName("HubName");
        }
    }
}



