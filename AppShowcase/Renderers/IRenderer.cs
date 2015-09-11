using System;
using Android.Animation;
using Android.Graphics;
using Android.Views;

namespace AppExtras.Renderers
{
    public interface IRenderer
    {
        Animator FadeInView(View target, long duration, Action started, Action ended);

        Animator FadeOutView(View target, long duration, Action started, Action ended);

        void DrawMask(View showcaseView, Canvas maskCanvas, Color maskColor, Point position, int radius, Animator animator);
    }
}
