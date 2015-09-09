using System;
using Android.Views;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.Graphics;
using Android.OS;
using Android.Util;

using AppExtras.ShowcaseAnimations;
using AppExtras.Showcases;

namespace AppExtras
{
    public class AppShowcaseView : FrameLayout
    {
        private int oldHeight;
        private int oldWidth;
        private Bitmap maskBitmap;
        private Canvas maskCanvas;

        private View mContentBox;
        private TextView mContentTextView;
        private TextView mDismissButton;
        private GravityFlags mGravity;
        private int mContentBottomMargin;
        private int mContentTopMargin;
        private IAnimationFactory animationFactory;

        private Handler mHandler;
        private LayoutListener layoutListener;

        private Showcase currentShowcase;

        public AppShowcaseView(Context context)
            : base(context)
        {
            Setup(context);
        }

        public AppShowcaseView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Setup(context);
        }

        public AppShowcaseView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            Setup(context);
        }

        // TODO: @TargetApi(android.os.Build.VERSION_CODES.LOLLIPOP) 
        public AppShowcaseView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Setup(context);
        }

        private void Setup(Context context)
        {
            mHandler = new Handler();

            SetWillNotDraw(false);

            // create our animation factory
            animationFactory = new AnimationFactory();

            // make sure we add a global layout listener so we can adapt to changes
            AttachLayoutListener();

            // consume touch events
            Touch += (sender, e) =>
            {
                if (CurrentStep.DismissOnTouch)
                {
                    DismissStep();
                }
                e.Handled = true;
            };

            Visibility = ViewStates.Invisible;

            View contentView = LayoutInflater.From(Context).Inflate(Resource.Layout.showcase_content, this, true);
            mContentBox = contentView.FindViewById(Resource.Id.content_box);
            mContentTextView = contentView.FindViewById<TextView>(Resource.Id.tv_content);
            mDismissButton = contentView.FindViewById<TextView>(Resource.Id.tv_dismiss);
            mDismissButton.Click += delegate
            {
                DismissStep();
            };
        }

        public bool AutoRemoveOnCompletion { get; set; }

        public Showcase CurrentShowcase
        {
            get { return currentShowcase; }
            set { currentShowcase = value; }
        }

        public ShowcaseStep CurrentStep
        {
            get { return CurrentShowcase.CurrentStep; }
        }

        public IAnimationFactory AnimationFactory
        {
            get { return animationFactory; }
            set { animationFactory = value ?? new NullAnimationFactory(); }
        }

        public event EventHandler ShowcaseDisplayed;

        public event EventHandler ShowcaseDismissed;

        /// <summary>
        /// Interesting drawing stuff.
        /// We draw a block of semi transparent colour to fill the whole screen then we draw of transparency
        /// to create a circular "viewport" through to the underlying content
        /// </summary>
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            // don't bother drawing if we're not ready
            if (CurrentStep == null)
            {
                return;
            }

            // get current dimensions
            int width = MeasuredWidth;
            int height = MeasuredHeight;

            // build a new canvas if needed i.e first pass or new dimensions
            if (maskBitmap == null || maskCanvas == null || oldHeight != height || oldWidth != width)
            {
                if (maskBitmap != null)
                {
                    maskBitmap.Recycle();
                    maskBitmap.Dispose();
                }

                maskBitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
                maskCanvas = new Canvas(maskBitmap);
            }

            // save our 'old' dimensions
            oldWidth = width;
            oldHeight = height;

            // ask the animator to draw the mask
            animationFactory.DrawMask(this, maskCanvas, CurrentStep.MaskColor, CurrentStep.Position, CurrentStep.Radius);

            // Draw the bitmap on our views  canvas.
            canvas.DrawBitmap(maskBitmap, 0, 0, null);
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            AttachLayoutListener();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();

            DetachLayoutListener();
        }

        protected virtual void OnShowcaseDisplayed()
        {
            var handler = ShowcaseDisplayed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnShowcaseDismissed()
        {
            var handler = ShowcaseDismissed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public static AppShowcaseView CreateShowcase(Activity activity)
        {
            var showcase = new AppShowcaseView(activity);
            ((ViewGroup)activity.Window.DecorView).AddView(showcase);
            return showcase;
        }

        public static AppShowcaseView CreateShowcase(Activity activity, Showcase showcase)
        {
            var showcaseView = CreateShowcase(activity);
            showcaseView.CurrentShowcase = showcase;
            return showcaseView;
        }

        public static void ResetShowcase(Activity activity, string showcaseId)
        {
            ResetShowcase(activity, new Showcase(showcaseId));
        }

        public static void ResetShowcase(Activity activity, Showcase showcase)
        {
            showcase.Reset(activity);
        }

        public static void ResetAllShowcases(Activity activity)
        {
            Showcase.ResetAll(activity);
        }

        /// <summary>
        /// Reveal the showcaseview. 
        /// Returns a boolean telling us whether we actually did show anything
        /// </summary>
        public bool Show()
        {
            // if we're in single use mode and have already shot our bolt then do nothing
            if (CurrentShowcase.IsRunOnce)
            {
                if (CurrentShowcase.HasFired(Context))
                {
                    return false;
                }
                else
                {
                    CurrentShowcase.SetFired(Context, true);
                }
            }

            // See if we have started this showcase before, if so then skip to the point we reached before
            // instead of showing the user everything from the start
            CurrentShowcase.LastStep(Context);

            return ShowNext();
        }

        public void DismissStep()
        {
            if (CurrentStep.FadeOutDuration > 0)
            {
                FadeOut();
            }
            else
            {
                ShowNext();
            }
        }

        public void FadeIn()
        {
            Visibility = ViewStates.Invisible;

            animationFactory.FadeInView(this, CurrentStep.FadeInDuration, () =>
            {
                Visibility = ViewStates.Visible;
                OnShowcaseDisplayed();
            });
        }

        public void FadeOut()
        {
            animationFactory.FadeOutView(this, CurrentStep.FadeOutDuration, () =>
            {
                Visibility = ViewStates.Invisible;
                ShowNext();
            });
        }

        public bool ShowNext()
        {
            var next = CurrentShowcase.NextStep(Context);
            if (next != null)
            {
                mContentTextView.Text = next.ContentText;
                mContentTextView.SetTextColor(next.ContentTextColor);
                mDismissButton.Text = next.DismissText;
                mDismissButton.SetTextColor(next.DismissTextColor);

                LayoutShowcaseContent();

                if (CurrentStep.Delay > 0)
                {
                    mHandler.PostDelayed(() =>
                    {
                        ShowCurrentStep();
                    }, CurrentStep.Delay);
                }
                else
                {
                    ShowCurrentStep();
                }

                return true;
            }
            else
            {
                if (AutoRemoveOnCompletion)
                {
                    var parent = Parent as ViewGroup;
                    if (parent != null)
                    {
                        parent.RemoveView(this);
                    }
                }
                return false;
            }
        }

        private void ShowCurrentStep()
        {
            if (CurrentStep.FadeInDuration > 0)
            {
                FadeIn();
            }
            else
            {
                Visibility = ViewStates.Visible;
                OnShowcaseDisplayed();
            }
        }

        private void LayoutShowcaseContent()
        {
            // don't bother laying out if we're not ready
            if (CurrentStep == null)
            {
                return;
            }

            // now figure out whether to put content above or below it
            int height = MeasuredHeight;
            int midPoint = height / 2;
            int yPos = CurrentStep.Position.Y;

            if (yPos > midPoint)
            {
                // value is in lower half of screen, we'll sit above it
                mContentTopMargin = 0;
                mContentBottomMargin = (height - yPos) + CurrentStep.Radius;
                mGravity = GravityFlags.Bottom;
            }
            else
            {
                // value is in upper half of screen, we'll sit below it
                mContentTopMargin = yPos + CurrentStep.Radius;
                mContentBottomMargin = 0;
                mGravity = GravityFlags.Top;
            }
            if (mContentBox != null && mContentBox.LayoutParameters != null)
            {
                FrameLayout.LayoutParams contentLP = (LayoutParams)mContentBox.LayoutParameters;

                bool layoutParamsChanged = false;

                if (contentLP.BottomMargin != mContentBottomMargin)
                {
                    contentLP.BottomMargin = mContentBottomMargin;
                    layoutParamsChanged = true;
                }

                if (contentLP.TopMargin != mContentTopMargin)
                {
                    contentLP.TopMargin = mContentTopMargin;
                    layoutParamsChanged = true;
                }

                if (contentLP.Gravity != mGravity)
                {
                    contentLP.Gravity = mGravity;
                    layoutParamsChanged = true;
                }

                // Only apply the layout params if we've actually changed them, otherwise we'll get stuck in a layout loop
                if (layoutParamsChanged)
                {
                    mContentBox.LayoutParameters = contentLP;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            // clean up
            if (maskBitmap != null)
            {
                maskBitmap.Recycle();
                maskBitmap.Dispose();
                maskBitmap = null;
            }
            animationFactory = null;
            maskCanvas = null;
            mHandler = null;

            // free event handlers
            DetachLayoutListener();

            base.Dispose(disposing);
        }

        private void AttachLayoutListener()
        {
            layoutListener = new LayoutListener(this);
            ViewTreeObserver.AddOnGlobalLayoutListener(layoutListener);
        }

        private void DetachLayoutListener()
        {
            if (layoutListener != null)
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBean)
                {
                    ViewTreeObserver.RemoveOnGlobalLayoutListener(layoutListener);
                }
                else
                {
                    ViewTreeObserver.RemoveOnGlobalLayoutListener(layoutListener);
                }
                layoutListener = null;
            }
        }

        /// <summary>
        /// Ensure that we redraw after activity finishes laying out
        /// </summary>
        private class LayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
        {
            private readonly AppShowcaseView outerInstance;

            public LayoutListener(AppShowcaseView outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public void OnGlobalLayout()
            {
                outerInstance.LayoutShowcaseContent();
            }
        }
    }
}
