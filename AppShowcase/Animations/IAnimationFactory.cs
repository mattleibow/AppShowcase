using System;
using Android.Graphics;
using Android.Views;

namespace AppExtras.ShowcaseAnimations
{
    public interface IAnimationFactory
    {
        void FadeInView(View target, long duration, Action started);

        void FadeOutView(View target, long duration, Action ended);

        void DrawMask(View showcaseView, Canvas maskCanvas, Color maskColor, Point position, int radius);
    }
}
