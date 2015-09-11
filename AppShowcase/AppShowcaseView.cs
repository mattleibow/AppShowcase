using System;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

using AppExtras.Renderers;
using AppExtras.Showcases;

namespace AppExtras
{
    public class AppShowcaseView : FrameLayout
    {
        private int oldHeight;
        private int oldWidth;
        private Bitmap maskBitmap;
        private Canvas maskCanvas;

        private View contentContainer;
        private TextView contentView;
        private TextView dismissView;
        private GravityFlags contentGravity;
        private int contentBottomMargin;
        private int contentTopMargin;

        private IRenderer animationFactory;
        private Animator animator;

        private LayoutListener layoutListener;
        private Handler handler;

        private Showcase currentShowcase;

        public AppShowcaseView(Context context)
            : base(context)
        {
            Setup(context);
        }

        //public AppShowcaseView(Context context, IAttributeSet attrs)
        //    : base(context, attrs)
        //{
        //    Setup(context);
        //}
        //
        //public AppShowcaseView(Context context, IAttributeSet attrs, int defStyleAttr)
        //    : base(context, attrs, defStyleAttr)
        //{
        //    Setup(context);
        //}
        //
        //// TODO: @TargetApi(android.os.Build.VERSION_CODES.LOLLIPOP) 
        //public AppShowcaseView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
        //    : base(context, attrs, defStyleAttr, defStyleRes)
        //{
        //    Setup(context);
        //}

        private void Setup(Context context)
        {
            AnimateInitialStep = true;
            handler = new Handler();
            Visibility = ViewStates.Invisible;
            SetWillNotDraw(false);
            // create our default animation factory
            animationFactory = new FadeRenderer();

            // make sure we add a global layout listener so we can adapt to changes
            AttachLayoutListener();

            // consume touch events
            Touch += (sender, e) =>
            {
                if (e.Event.Action == MotionEventActions.Up && CurrentStep.DismissOnTouch)
                {
                    DismissStep();
                }
                e.Handled = true;
            };

            View contentView = LayoutInflater.From(Context).Inflate(Resource.Layout.showcase_content, this, true);
            contentContainer = contentView.FindViewById(Resource.Id.content_box);
            this.contentView = contentView.FindViewById<TextView>(Resource.Id.tv_content);
            dismissView = contentView.FindViewById<TextView>(Resource.Id.tv_dismiss);
            dismissView.Click += delegate
            {
                DismissStep();
            };
        }

        public bool AnimateInitialStep { get; set; }

        public Showcase CurrentShowcase
        {
            get { return currentShowcase; }
            set { currentShowcase = value; }
        }

        public ShowcaseStep CurrentStep
        {
            get { return CurrentShowcase == null ? null : CurrentShowcase.CurrentStep; }
        }

        public int CurrentStepIndex
        {
            get { return CurrentShowcase == null ? -1 : CurrentShowcase.CurrentStepIndex; }
        }

        public IRenderer Animation
        {
            get { return animationFactory; }
            set { animationFactory = value ?? new NullRenderer(); }
        }

        public event EventHandler ShowcaseStarted;

        public event EventHandler<ShowcaseStepEventArgs> StepDisplayed;

        public event EventHandler<ShowcaseStepEventArgs> StepDismissed;

        public event EventHandler ShowcaseCompleted;

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

            // clear canvas
            maskCanvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);

            // ask the animator to draw the mask
            animationFactory.DrawMask(this, maskCanvas, CurrentStep.MaskColor, CurrentStep.Position, CurrentStep.Radius, animator);

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

        protected virtual void OnShowcaseStarted()
        {
            var handler = ShowcaseStarted;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnStepDisplayed(ShowcaseStepEventArgs e)
        {
            var handler = StepDisplayed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnStepDismissed(ShowcaseStepEventArgs e)
        {
            var handler = StepDismissed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnShowcaseCompleted()
        {
            var handler = ShowcaseCompleted;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public static AppShowcaseView Create(Activity activity)
        {
            var showcase = new AppShowcaseView(activity);
            ((ViewGroup)activity.Window.DecorView).AddView(showcase);
            return showcase;
        }

        public static AppShowcaseView Create(Activity activity, Showcase showcase)
        {
            var showcaseView = Create(activity);
            showcaseView.CurrentShowcase = showcase;
            return showcaseView;
        }

        public static AppShowcaseView Create(Activity activity, View targetView, string content, string dismissText)
        {
            return Create(activity, Showcase.Create(targetView, content, dismissText));
        }

        public static AppShowcaseView Create(Activity activity, int targetViewId, string content, string dismissText)
        {
            return Create(activity, Showcase.Create(activity, targetViewId, content, dismissText));
        }

        /// <summary>
        /// Reveal the AppShowcaseView after a delay. 
        /// </summary>
        public void Show(long delay)
        {
            handler.PostDelayed(() => Show(), delay);
        }

        /// <summary>
        /// Reveal the AppShowcaseView.
        /// </summary>
        /// <returns><c>true</c> if the view did show something, <c>false</c> otherwise.</returns>
        public bool Show()
        {
            // if we're in single use mode and have already shot our bolt then do nothing
            if (CurrentShowcase == null || CurrentShowcase.HasFired(Context))
            {
                return false;
            }

            OnShowcaseStarted();

            // See if we have started this showcase before, if so then skip to the point we reached before
            // instead of showing the user everything from the start
            CurrentShowcase.LastStep(Context);

            return ShowNext(AnimateInitialStep);
        }

        public void DismissStep()
        {
            if (CurrentStep.FadeOutDuration > 0)
            {
                FadeOut();
            }
            else
            {
                OnStepDismissed(new ShowcaseStepEventArgs(CurrentStepIndex, CurrentStep));
                ShowNext();
            }
        }

        public void FadeIn()
        {
            Visibility = ViewStates.Invisible;

            animator = animationFactory.FadeInView(this, CurrentStep.FadeInDuration, () =>
            {
                Visibility = ViewStates.Visible;
                OnStepDisplayed(new ShowcaseStepEventArgs(CurrentStepIndex, CurrentStep));
            }, () =>
            {
                animator = null;
            });
        }

        public void FadeOut()
        {
            animator = animationFactory.FadeOutView(this, CurrentStep.FadeOutDuration, null, () =>
            {
                animator = null;
                Visibility = ViewStates.Invisible;
                OnStepDismissed(new ShowcaseStepEventArgs(CurrentStepIndex, CurrentStep));
                ShowNext();
            });
        }

        public bool ShowNext()
        {
            return ShowNext(true);
        }

        private bool ShowNext(bool animate)
        {
            if (CurrentShowcase == null)
            {
                return false;
            }

            var next = CurrentShowcase.NextStep(Context);
            if (next != null)
            {
                UpdateShowcaseContent(next);
                LayoutShowcaseContent();

                if (CurrentStep.Delay > 0 && animate)
                {
                    handler.PostDelayed(() =>
                    {
                        ShowCurrentStep(animate);
                    }, CurrentStep.Delay);
                }
                else
                {
                    ShowCurrentStep(animate);
                }

                return true;
            }
            else
            {
                var parent = Parent as ViewGroup;
                if (parent != null)
                {
                    parent.RemoveView(this);
                }

                OnShowcaseCompleted();

                return false;
            }
        }

        private void ShowCurrentStep(bool animate)
        {
            if (CurrentStep.FadeInDuration > 0 && animate)
            {
                FadeIn();
            }
            else
            {
                Visibility = ViewStates.Visible;
                OnStepDisplayed(new ShowcaseStepEventArgs(CurrentStepIndex, CurrentStep));
            }
        }

        private void UpdateShowcaseContent(ShowcaseStep next)
        {
            contentView.Text = next.ContentText;
            contentView.SetTextColor(next.ContentTextColor);
            dismissView.Text = next.DismissText;
            dismissView.SetTextColor(next.DismissTextColor);
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
                contentTopMargin = 0;
                contentBottomMargin = (height - yPos) + CurrentStep.Radius;
                contentGravity = GravityFlags.Bottom;
            }
            else
            {
                // value is in upper half of screen, we'll sit below it
                contentTopMargin = yPos + CurrentStep.Radius;
                contentBottomMargin = 0;
                contentGravity = GravityFlags.Top;
            }
            if (contentContainer != null && contentContainer.LayoutParameters != null)
            {
                FrameLayout.LayoutParams contentLP = (LayoutParams)contentContainer.LayoutParameters;

                bool layoutParamsChanged = false;

                if (contentLP.BottomMargin != contentBottomMargin)
                {
                    contentLP.BottomMargin = contentBottomMargin;
                    layoutParamsChanged = true;
                }

                if (contentLP.TopMargin != contentTopMargin)
                {
                    contentLP.TopMargin = contentTopMargin;
                    layoutParamsChanged = true;
                }

                if (contentLP.Gravity != contentGravity)
                {
                    contentLP.Gravity = contentGravity;
                    layoutParamsChanged = true;
                }

                // Only apply the layout params if we've actually changed them, otherwise we'll get stuck in a layout loop
                if (layoutParamsChanged)
                {
                    contentContainer.LayoutParameters = contentLP;
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
                ViewTreeObserver.RemoveGlobalOnLayoutListener(layoutListener);
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
