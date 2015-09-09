using System;
using Android.Graphics;
using Android.Views;

namespace AppExtras.ShowcaseAnimations
{
    public class NullAnimationFactory : IAnimationFactory
    {
        private const float InvisibleValue = 0f;
        private const float VisibleValue = 1f;

        private readonly Paint eraserPaint;

        public NullAnimationFactory()
        {
            // Erase a circle
            if (eraserPaint == null)
            {
                eraserPaint = new Paint();
                eraserPaint.Color = Color.Transparent;
                eraserPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));
                eraserPaint.Flags = PaintFlags.AntiAlias;
            }
        }

        public void FadeInView(View target, long duration, Action started)
        {
            target.Alpha = VisibleValue;
            if (started != null)
            {
                started();
            }
        }

        public void FadeOutView(View target, long duration, Action ended)
        {
            target.Alpha = InvisibleValue;
            if (ended != null)
            {
                ended();
            }
        }

        public void DrawMask(View target, Canvas maskCanvas, Color maskColor, Point position, int radius)
        {
            // clear canvas
            maskCanvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);

            // draw solid background
            maskCanvas.DrawColor(maskColor);

            // erase focus area
            maskCanvas.DrawCircle(position.X, position.Y, radius, eraserPaint);
        }
    }
}
