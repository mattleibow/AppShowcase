using System;
using Android.Animation;
using Android.Graphics;
using Android.Views;

namespace AppExtras.ShowcaseAnimations
{
    public interface IAnimationFactory
    {
        ValueAnimator FadeInView(View target, long duration, Action started, Action ended);

        ValueAnimator FadeOutView(View target, long duration, Action started, Action ended);

        void DrawMask(View showcaseView, Canvas maskCanvas, Color maskColor, Point position, int radius, float? fadeInValue, float? fadeOutValue);
    }
}
