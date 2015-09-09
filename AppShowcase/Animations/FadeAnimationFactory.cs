using System;
using Android.Animation;
using Android.Graphics;
using Android.Views;

namespace AppExtras.ShowcaseAnimations
{
    public class FadeAnimationFactory : IAnimationFactory
    {
        private readonly Paint eraserPaint;

        public FadeAnimationFactory()
        {
            eraserPaint = AnimationFactoryHelpers.CreateEraser();
        }

        public ValueAnimator FadeInView(View target, long duration, Action started, Action ended)
        {
            AnimationFactoryHelpers.AnimateAlphaProperty(target, duration, true, started, ended);
            return null;
        }

        public ValueAnimator FadeOutView(View target, long duration, Action started, Action ended)
        {
            AnimationFactoryHelpers.AnimateAlphaProperty(target, duration, false, started, ended);
            return null;
        }

        public void DrawMask(View target, Canvas maskCanvas, Color maskColor, Point position, int radius, float? fadeInValue, float? fadeOutValue)
        {
            // draw solid background
            maskCanvas.DrawColor(maskColor);

            // erase focus area
            maskCanvas.DrawCircle(position.X, position.Y, radius, eraserPaint);
        }
    }
}
