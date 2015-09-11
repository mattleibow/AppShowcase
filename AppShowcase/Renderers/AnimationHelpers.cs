using System;
using Android.Animation;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;

namespace AppExtras.Renderers
{
    internal static class RenderingHelpers
    {
        public const string AlphaPropertyName = "alpha";
        public const float InvisibleValue = 0f;
        public const float VisibleValue = 1f;

        public static Paint CreateEraser()
        {
            var eraserPaint = new Paint();
            eraserPaint.Color = Color.Transparent;
            eraserPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));
            eraserPaint.Flags = PaintFlags.AntiAlias;

            return eraserPaint;
        }
        
        public static ValueAnimator CreateValueAnimator(long duration, float initial, float destination, Action started, Action ended, Action<float> update)
        {
            return CreateValueAnimator(duration, 0, initial, destination, new AccelerateDecelerateInterpolator(), started, ended, update);
        }

        public static ValueAnimator CreateValueAnimator(long duration, long delay, float initial, float destination, IInterpolator interpolator, Action started, Action ended, Action<float> update)
        {
            var animator = ValueAnimator.OfFloat(initial, destination);
            animator.SetDuration(duration);
            animator.SetInterpolator(interpolator);
            animator.StartDelay = delay;
            if (update != null)
            {
                animator.Update += delegate
                {
                    update((float)animator.AnimatedValue);
                };
            }
            AttachEvents(animator, started, ended);
            animator.Start();
            return animator;
        }

        public static void AnimateAlphaProperty(View target, long duration, bool fadeIn, Action started, Action ended)
        {
            AnimateAlphaProperty(target, duration, 0, fadeIn, started, ended);
        }

        public static void AnimateAlphaProperty(View target, long duration, long delay, bool fadeIn, Action started, Action ended)
        {
            AnimateAlphaProperty(target, duration, delay, fadeIn, new AccelerateDecelerateInterpolator(), started, ended);
        }

        public static void AnimateAlphaProperty(View target, long duration, long delay, bool fadeIn, IInterpolator interpolator, Action started, Action ended)
        {
            var initial = fadeIn ? InvisibleValue : VisibleValue;
            var destination = fadeIn ? VisibleValue : InvisibleValue;
            AnimateAlphaProperty(target, duration, delay, initial, destination, interpolator, started, ended);
        }

        public static void AnimateAlphaProperty(View target, long duration, long delay, float initial, float destination, IInterpolator interpolator, Action started, Action ended)
        {
            var animator = ObjectAnimator.OfFloat(target, AlphaPropertyName, initial, destination);
            animator.SetInterpolator(interpolator);
            animator.StartDelay = delay;
            animator.SetDuration(duration);
            AttachEvents(animator, started, ended);
            animator.Start();
        }

        public static void AttachEvents(Animator animator, Action started, Action ended)
        {
            if (started != null)
            {
                animator.AnimationStart += delegate
                {
                    started();
                };
            }
            if (ended != null)
            {
                animator.AnimationEnd += delegate
                {
                    ended();
                };
            }
        }

        public static void SetAlphaProperty(View target, bool makeVisible, Action started, Action ended)
        {
            var initial = makeVisible ? InvisibleValue : VisibleValue;
            var destination = makeVisible ? VisibleValue : InvisibleValue;
            if (started != null)
            {
                started();
            }
            target.Alpha = destination;
            if (ended != null)
            {
                ended();
            }
        }
    }
}
