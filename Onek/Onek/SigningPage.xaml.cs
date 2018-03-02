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

        Dictionary<long, SKPath> inProgressPathsResize = new Dictionary<long, SKPath>();
        List<SKPath> completedPathsResize = new List<SKPath>();

        Evaluation Eval;
        Candidate CurrentCandidate;
        SKCanvas canvasSaved;
        SKData data;

        private int widthExport = 200;
        private int heightExport = 400;

        public SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };
        
        /// <summary>
        /// Constructor of the Signing Page
        /// </summary>
        /// <param name="eval">Evaluation you wish to sign</param>
        /// <param name="c">Candidate you wish to sign</param>
        public SigningPage(Evaluation eval, Candidate c)
		{
            Eval = eval;
            CurrentCandidate = c;
            
			InitializeComponent();
        }

        /// <summary>
        /// Event thrown when touching the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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
                        SKPath pathResize = new SKPath();
                        pathResize.MoveTo(ConvertToPixelResize(args.Location));
                        inProgressPathsResize.Add(args.Id, pathResize);
                    }
                    break;

                case TouchActionType.Moved:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = inProgressPaths[args.Id];
                        path.LineTo(ConvertToPixel(args.Location));
                        canvasView.InvalidateSurface();
                        SKPath pathResize = inProgressPathsResize[args.Id];
                        pathResize.LineTo(ConvertToPixelResize(args.Location));
                    }
                    break;

                case TouchActionType.Released:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        completedPaths.Add(inProgressPaths[args.Id]);
                        inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                        completedPathsResize.Add(inProgressPathsResize[args.Id]);
                        inProgressPathsResize.Remove(args.Id);
                    }
                    break;

                case TouchActionType.Cancelled:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                        inProgressPathsResize.Remove(args.Id);
                    }
                    break;
            }
        }

        /// <summary>
        /// Convert a point to a pixel depending on the canvas Size
        /// </summary>
        /// <param name="pt">Point to convert to a pixel</param>
        /// <returns></returns>
        SKPoint ConvertToPixel(Point pt)
        {
            float x = (float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width);
            float y = (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height);
            
            return new SKPoint(x, y);
        }

        /// <summary>
        /// Convert a pont to a pixel depending on local size
        /// </summary>
        /// <param name="pt">Point to convert to a pixel</param>
        /// <returns></returns>
        SKPoint ConvertToPixelResize(Point pt)
        {
            float x = (float)(widthExport * pt.X / canvasView.Width);
            float y = (float)(heightExport * pt.Y / canvasView.Height);

            return new SKPoint(x, y);
        }

        /// <summary>
        /// Event thrown when something is drawned on the surface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKCanvas canvas = args.Surface.Canvas;
            canvas.Clear();

            canvasSaved = canvas;

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

        /// <summary>
        /// Event thrown when touching the "Retour" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        async Task OnRetourButtonClickedAsync(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Retour", "Voulez-vous arrêter la signature ?", "Oui", "Non");
            if (answer)
            {
                await Navigation.PopAsync();
            }
        }

        /// <summary>
        /// Event thrown when touching the "Valider" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        async Task OnValidateButtonClickedAsync(object sender, EventArgs e)
        {
            if (entryName.Text == null || entryName.Text.Equals(""))
            {
                await DisplayAlert("Erreur", "Le nom du signataire ne doit pas être vide", "OK");
                return;
            }

            SKImageInfo info = new SKImageInfo(widthExport, heightExport);
            var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            
            foreach (SKPath path in completedPathsResize)
                canvas.DrawPath(path, paint);

            foreach (SKPath path in inProgressPathsResize.Values)
                canvas.DrawPath(path, paint);


            SKImage snap = surface.Snapshot();
            data = snap.Encode();
            
            Eval.IsSigned = true;
            CurrentCandidate.IsSigned = true;


            Eval.Signatures.Add(new Signature(entryName.Text, data.ToArray()));
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
                    canvasSaved = null;
                }
                canvasView.InvalidateSurface();
                completedPaths.Clear();
                inProgressPaths.Clear();
                completedPathsResize.Clear();
                inProgressPathsResize.Clear();
                entryName.Text = "";
            }
        }
    }
}