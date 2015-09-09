using System;
using Android.Animation;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;

namespace AppExtras.ShowcaseAnimations
{
    public class AnticipateOvershootAnimationFactory : IAnimationFactory
    {
        private readonly AnticipateOvershootInterpolator bouncy;
        private readonly Paint eraserPaint;

        public AnticipateOvershootAnimationFactory()
        {
            bouncy = new AnticipateOvershootInterpolator();
            eraserPaint = AnimationFactoryHelpers.CreateEraser();
        }

        public ValueAnimator FadeInView(View target, long duration, Action started, Action ended)
        {
            var animator = AnimationFactoryHelpers.CreateValueAnimator(duration, 0f, 1f, started, ended, v => target.Invalidate());
            AnimationFactoryHelpers.AnimateAlphaProperty(target, duration / 2, true, null, null);
            return animator;
        }

        public ValueAnimator FadeOutView(View target, long duration, Action started, Action ended)
        {
            var animator = AnimationFactoryHelpers.CreateValueAnimator(duration, 1f, 0f, started, ended, v => target.Invalidate());
            AnimationFactoryHelpers.AnimateAlphaProperty(target, duration / 2, duration / 2, false, null, null);
            return animator;
        }

        public void DrawMask(View target, Canvas maskCanvas, Color maskColor, Point position, int radius, float? fadeInValue, float? fadeOutValue)
        {
            // draw solid background
            maskCanvas.DrawColor(maskColor);

            // erase focus area
            maskCanvas.DrawCircle(position.X, position.Y, radius * bouncy.GetInterpolation(fadeInValue ?? fadeOutValue ?? target.Alpha), eraserPaint);
        }
    }
}
