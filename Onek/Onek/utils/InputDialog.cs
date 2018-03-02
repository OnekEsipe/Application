using System;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Onek.utils
{
    /// <summary>
    /// Custom input dialog
    /// </summary>
    class InputDialog
    {

        /// <summary>
        /// Create a simple input box
        /// </summary>
        /// <param name="navigation">Navigation of the page (this.Navigation)</param>
        /// <param name="title">Title of the pop up window</param>
        /// <param name="text">Text of the pop up label</param>
        /// <param name="placeholder">Place holder of the editor</param>
        /// <returns></returns>
        public static Task<string> InputBox(INavigation navigation, String title, String text, String placeholder)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();
            string temp = placeholder;

            Label lblTitle = new Label { Text = title, HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label lblMessage = new Label { Text = text };
            Editor txtInput = new Editor { Text = placeholder, WidthRequest = 300, HorizontalOptions = LayoutOptions.Center };

            txtInput.TextChanged += (sender, args) =>
            {
                var method = typeof(View).GetMethod("InvalidateMeasure", BindingFlags.Instance | BindingFlags.NonPublic);
                method.Invoke(txtInput, null);
            };

            return makeBox(navigation, lblTitle, lblMessage, txtInput, tcs, temp);
        }

        /// <summary>
        /// Create a simple input box
        /// </summary>
        /// <param name="navigation">Navigation of the page (this.Navigation)</param>
        /// <param name="title">Title of the pop up window</param>
        /// <param name="text">Text of the pop up label</param>
        /// <param name="placeholder">Place holder of the editor</param>
        /// <param name="size">Maximum size of the editor</param>
        /// <returns></returns>
        public static Task<string> InputBoxWithSize(INavigation navigation, String title, String text, String placeholder, int size)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();
            string temp = placeholder;

            Label lblTitle = new Label { Text = title, HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label lblMessage = new Label { Text = text + " ("+ (size-placeholder.Length) + " caractères restants) :" };
            Editor txtInput = new Editor { Text = placeholder, WidthRequest = 300, HorizontalOptions = LayoutOptions.Center };

            txtInput.TextChanged += (sender, args) =>
            {
                string input = txtInput.Text;     
                if (input.Length > size)       
                {
                    input = input.Substring(0, 500);
                    txtInput.Text = input;    
                }
                lblMessage.Text = text + " (" + (size - input.Length) + " caractères restants) :";
                var method = typeof(View).GetMethod("InvalidateMeasure", BindingFlags.Instance | BindingFlags.NonPublic);
                method.Invoke(txtInput, null);
            };

            return makeBox(navigation, lblTitle, lblMessage, txtInput, tcs, temp);
        }

        /// <summary>
        ///  Create a box 
        /// </summary>
        /// <param name="navigation">Navigation of the page (this.Navigation)</param>
        /// <param name="lblTitle">Label View for the title</param>
        /// <param name="lblMessage">Label View for the label</param>
        /// <param name="txtInput">Editor View for the text</param>
        /// <param name="tcs">Task Completion Source used to call the async task</param>
        /// <param name="temp">Placeholder</param>
        /// <returns></returns>
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

            Frame frame = new Frame
            {
                IsClippedToBounds = false,
                HasShadow = true,
                CornerRadius = 10,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Margin = new Thickness(0, 10),
                Content = layout,
            };

            var mainLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Children = { frame },
            };


            // create and show page
            var page = new ContentPage();
            page.BackgroundColor = Color.FromHex("#80000000");
            page.Content = mainLayout;
            navigation.PushModalAsync(page, true);
            // open keyboard
            txtInput.Focus();

            // code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
            // then proc returns the result
            return tcs.Task;
        }
    }
}
