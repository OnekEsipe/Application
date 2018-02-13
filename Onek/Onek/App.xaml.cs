﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Onek
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
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
