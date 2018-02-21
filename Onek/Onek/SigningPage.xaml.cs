using Onek.data;
using Onek.TouchTracking;
using SkiaSharp;
using SkiaSharp.Views.Forms;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Onek
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SigningPage : ContentPage
	{
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();
        Evaluation Eval;
        SKCanvas canvasSaved;
        SKData data;
        SKSurface newSurface;

        public SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };
        
        public SigningPage(Evaluation eval)
		{
            Eval = eval;
            
			InitializeComponent();
        }

        void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (!inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = new SKPath();
                        path.MoveTo(ConvertToPixel(args.Location));
                        inProgressPaths.Add(args.Id, path);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Moved:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = inProgressPaths[args.Id];
                        path.LineTo(ConvertToPixel(args.Location));
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Released:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        completedPaths.Add(inProgressPaths[args.Id]);
                        inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Cancelled:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;
            }
        }

        SKPoint ConvertToPixel(Point pt)
        {
            float x = (float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width);
            float y = (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height);
            
            return new SKPoint(x, y);
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKCanvas canvas = args.Surface.Canvas;
            newSurface = args.Surface;
            canvas.Clear();

            SKImageInfo info = new SKImageInfo((int)Math.Ceiling(canvasView.CanvasSize.Width), (int)Math.Ceiling(canvasView.CanvasSize.Height));
            newSurface = SKSurface.Create(info);

            foreach (SKPath path in completedPaths)
            {
                canvas.DrawPath(path, paint);
            }

            foreach (SKPath path in inProgressPaths.Values)
            {
                canvas.DrawPath(path, paint);
            }

            canvas.Flush();            
        }

        async Task OnRetourButtonClickedAsync(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Retour", "Voulez-vous arrêter la signature ?", "Oui", "Non");
            if (answer)
            {
                await Navigation.PopAsync();
            }
        }


        async Task OnValidateButtonClickedAsync(object sender, EventArgs e)
        {
            if (entryName.Text == null || entryName.Text.Equals(""))
            {
                await DisplayAlert("Erreur", "Le nom du signataire ne doit pas être vide", "OK");
                return;
            }
            SKImage snap = newSurface.Snapshot();
            canvasSaved = newSurface.Canvas;
            data = snap.Encode();
            
            Eval.isSigned = true;
            
            Eval.Signatures.Add(new Signature(Eval.Id, entryName.Text, data.ToArray()));
            bool answer = await DisplayAlert("Signature", "Voulez-vous effectuer une autre signature ?", "Oui", "Non");
            if (!answer)
            {
                await Navigation.PopAsync();
            }
            else
            {
                data = null;
                if(canvasSaved != null)
                {
                    canvasSaved.Clear();
                }
                canvasView.InvalidateSurface();
                completedPaths.Clear();
                inProgressPaths.Clear();
                entryName.Text = "";
            }
        }
    }
}