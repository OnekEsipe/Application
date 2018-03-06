using Onek.utils;
using System;
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
                    EvaluationSender.LoadJsons();
                    EvaluationSender.SendJsonEvalToServer();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            });

            //Display the first Page of the app (MainPage => LoginPage)
            MainPage = new NavigationPage(new Onek.MainPage());
		}

		protected override void OnStart ()
		{
            
            // Handle when your app starts
        }

		protected override void OnSleep ()
		{
            // Handle when your app sleeps
            //Load and send json which were not sended to server
            Task.Run(() =>
            {
                try
                {
                    EvaluationSender.LoadJsons();
                    EvaluationSender.SendJsonEvalToServer();
                }
                catch (Exception)
                {
                }
            });
        }

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
        
	}
}
