using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Onek.utils
{
    class InputDialog
    {

        public static Task<string> InputBox(INavigation navigation, String title, String text, String placeholder)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();
            string temp = placeholder;

            Label lblTitle = new Label { Text = title, HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label lblMessage = new Label { Text = text };
            Editor txtInput = new Editor { Text = placeholder, WidthRequest = 300, HeightRequest=250, HorizontalOptions = LayoutOptions.Center };

            return makeBox(navigation, lblTitle, lblMessage, txtInput, tcs, temp);
        }

        public static Task<string> InputBoxWithSize(INavigation navigation, String title, String text, String placeholder, int size)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();
            string temp = placeholder;

            Label lblTitle = new Label { Text = title, HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label lblMessage = new Label { Text = text + " ("+ (size-placeholder.Length) + " caractères restants) :" };
            Editor txtInput = new Editor { Text = placeholder, WidthRequest = 300, HeightRequest = 250, HorizontalOptions = LayoutOptions.Center };

            txtInput.TextChanged += (sender, args) =>
            {
                string _text = txtInput.Text;     
                if (text.Length > size)       
                {
                    text = text.Remove(_text.Length - 1); 
                    txtInput.Text = text;    
                }
                lblMessage.Text = text + " (" + (size - text.Length) + " caractères restants) :";
            };

            return makeBox(navigation, lblTitle, lblMessage, txtInput, tcs, temp);
        }


        private static Task<string> makeBox(INavigation navigation, Label lblTitle, Label lblMessage, Editor txtInput, TaskCompletionSource<string> tcs, String temp)
        {
            var btnOk = new Button
            {
                Text = "Ok",
                WidthRequest = 100,
                BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8),
            };
            btnOk.Clicked += async (s, e) =>
            {
                // close page
                var result = txtInput.Text;
                await navigation.PopModalAsync();
                // pass result
                tcs.SetResult(result);
            };

            var btnCancel = new Button
            {
                Text = "Annuler",
                WidthRequest = 100,
                BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8)
            };
            btnCancel.Clicked += async (s, e) =>
            {
                // close page
                await navigation.PopModalAsync();
                // pass empty result
                tcs.SetResult(temp);
            };

            var btnErase = new Button
            {
                Text = "Effacer",
                WidthRequest = 200,
                BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8),
                HorizontalOptions = LayoutOptions.Center
            };
            btnErase.Clicked += (s, e) =>
            {
                txtInput.Text = "";
            };

            var slButtons = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { btnOk, btnCancel },
                HorizontalOptions = LayoutOptions.Center
            };

            var layout = new StackLayout
            {
                Padding = new Thickness(0, 40, 0, 0),
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = { lblTitle, lblMessage, txtInput, btnErase, slButtons },
            };

            // create and show page
            var page = new ContentPage();
            page.Content = layout;
            navigation.PushModalAsync(page);
            // open keyboard
            txtInput.Focus();

            // code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
            // then proc returns the result
            return tcs.Task;
        }
    }
}
