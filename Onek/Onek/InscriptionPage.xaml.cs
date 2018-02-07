using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Onek
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InscriptionPage : ContentPage
	{
		public InscriptionPage ()
		{
			InitializeComponent ();
		}

        async void OnButtonClicked(object sender, EventArgs e)
        {
            //Retour et Quitter
        }
	}
}