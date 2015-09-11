using System;
using Android.Animation;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;

namespace AppExtras.Renderers
{
    public class AnticipateOvershootRenderer : IRenderer
    {
        private readonly AnticipateOvershootInterpolator bouncy;
        private readonly Paint eraserPaint;

        public AnticipateOvershootRenderer()
        {
            bouncy = new AnticipateOvershootInterpolator();
            eraserPaint = RenderingHelpers.CreateEraser();
        }

        public Animator FadeInView(View target, long duration, Action started, Action ended)
        {
            var animator = RenderingHelpers.CreateValueAnimator(duration, 0f, 1f, started, ended, v => target.Invalidate());
            RenderingHelpers.AnimateAlphaProperty(target, (long)(duration * 0.666), true, null, null);
            return animator;
        }

        public Animator FadeOutView(View target, long duration, Action started, Action ended)
        {
            var animator = RenderingHelpers.CreateValueAnimator(duration, 1f, 0f, started, ended, v => target.Invalidate());
            RenderingHelpers.AnimateAlphaProperty(target, (long)(duration * 0.333), (long)(duration * 0.666), false, null, null);
            return animator;
        }

        public void DrawMask(View target, Canvas maskCanvas, Color maskColor, Point position, int radius, Animator animator)
        {
            var valueAnimator = animator as ValueAnimator;
            var bounce = valueAnimator != null ? (float)valueAnimator.AnimatedValue : target.Alpha;

            // draw solid background
            maskCanvas.DrawColor(maskColor);

            // erase focus area
            maskCanvas.DrawCircle(position.X, position.Y, radius * bouncy.GetInterpolation(bounce), eraserPaint);
        }
    }
}
