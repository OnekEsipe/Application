using Onek.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Onek
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

            //For https connexion
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;

            //Read config file to load server URL
            ApplicationConstants.ReadConfigFile();

            //Load and send json which were not sended to server
            Task.Run(() =>
            {
                try
                {
                    EvaluationSender.Test();
                    EvaluationSender.LoadJsons();
                    EvaluationSender.SendJsonEvalToServer();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            });

            MainPage = new NavigationPage(new Onek.MainPage());
		}

		protected override void OnStart ()
		{
            
            // Handle when your app starts
        }

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
