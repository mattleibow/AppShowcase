using System;
using Android.Animation;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;

namespace AppExtras.ShowcaseAnimations
{
    public class BouncyAnimationFactory : IAnimationFactory
    {
        private const string AlphaPropertyName = "alpha";
        private const float InvisibleValue = 0f;
        private const float VisibleValue = 1f;

        private readonly AccelerateDecelerateInterpolator interpolator;
        private readonly AnticipateOvershootInterpolator bouncy;
        private readonly Paint eraserPaint;

        public BouncyAnimationFactory()
        {
            interpolator = new AccelerateDecelerateInterpolator();
            bouncy = new AnticipateOvershootInterpolator();

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
            ObjectAnimator oa = ObjectAnimator.OfFloat(target, AlphaPropertyName, InvisibleValue, VisibleValue);
            oa.SetDuration(duration);
            oa.AnimationStart += delegate
            {
                if (started != null)
                {
                    started();
                }
            };
            oa.Start();
        }

        public void FadeOutView(View target, long duration, Action ended)
        {
            ObjectAnimator oa = ObjectAnimator.OfFloat(target, AlphaPropertyName, InvisibleValue);
            oa.SetDuration(duration);
            oa.AnimationEnd += delegate
            {
                if (ended != null)
                {
                    ended();
                }
            };
            oa.Start();
        }

        public void DrawMask(View target, Canvas maskCanvas, Color maskColor, Point position, int radius)
        {
            // clear canvas
            maskCanvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);

            // draw solid background
            maskCanvas.DrawColor(maskColor);

            // erase focus area
            maskCanvas.DrawCircle(position.X, position.Y, radius * bouncy.GetInterpolation(target.Alpha), eraserPaint);
        }
    }
}
