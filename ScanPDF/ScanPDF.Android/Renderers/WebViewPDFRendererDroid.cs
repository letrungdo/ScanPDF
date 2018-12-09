using System;
using System.ComponentModel;
using Android.Content;
using ScanPDF.ControlsRenderer;
using ScanPDF.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(WebViewPDF), typeof(WebViewPDFRendererDroid))]
namespace ScanPDF.Droid.Renderers
{
    public class WebViewPDFRendererDroid : WebViewRenderer
    {
        public WebViewPDFRendererDroid(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                // because using library pdf.js, so allow webview open pdf downloaded from temp folder
                Control.Settings.AllowUniversalAccessFromFileURLs = true;

                // Clear background
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
                if (Element.Source is UrlWebViewSource sourceWeb)
                {
                    Control?.LoadUrl($"file:///android_asset/pdfjs/web/viewer.html?file=file://{sourceWeb.Url}");
                }
            }
        }
    }
}
