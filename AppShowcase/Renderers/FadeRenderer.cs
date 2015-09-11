using System;
using Android.Animation;
using Android.Graphics;
using Android.Views;

namespace AppExtras.Renderers
{
    public class FadeRenderer : IRenderer
    {
        private readonly Paint eraserPaint;

        public FadeRenderer()
        {
            eraserPaint = RenderingHelpers.CreateEraser();
        }

        public Animator FadeInView(View target, long duration, Action started, Action ended)
        {
            RenderingHelpers.AnimateAlphaProperty(target, duration, true, started, ended);
            return null;
        }

        public Animator FadeOutView(View target, long duration, Action started, Action ended)
        {
            RenderingHelpers.AnimateAlphaProperty(target, duration, false, started, ended);
            return null;
        }

        public void DrawMask(View target, Canvas maskCanvas, Color maskColor, Point position, int radius, Animator animator)
        {
            // draw solid background
            maskCanvas.DrawColor(maskColor);

            // erase focus area
            maskCanvas.DrawCircle(position.X, position.Y, radius, eraserPaint);
        }
    }
}
