using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;

namespace AppExtras.Showcases
{
    public class Showcase
    {
        private List<ShowcaseStep> steps;
        private int position;

        public Showcase()
        {
            Setup();
            ShowcaseId = null;
        }

        public Showcase(string showcaseId)
        {
            Setup();
            ShowcaseId = showcaseId;
        }

        private void Setup()
        {
            position = -1;
            steps = new List<ShowcaseStep>();
        }

        internal int CurrentStepIndex
        {
            get { return position; }
        }

        internal ShowcaseStep CurrentStep
        {
            get
            {
                if (position >= 0 && position < steps.Count)
                {
                    return steps[position];
                }
                return null;
            }
        }

        public string ShowcaseId { get; set; }

        public ShowcaseStep AddStep(View targetView, string content, string dismissText)
        {
            ShowcaseStep step = new ViewShowcaseStep(targetView)
            {
                DismissText = dismissText,
                ContentText = content
            };

            AddStep(step);

            return step;
        }

        public ShowcaseStep AddStep(Activity activity, int targetViewId, string content, string dismissText)
        {
            ShowcaseStep step = new ViewShowcaseStep(activity, targetViewId)
            {
                DismissText = dismissText,
                ContentText = content
            };

            AddStep(step);

            return step;
        }

        public ShowcaseStep AddStep(View targetView, string content, string dismissText, long duration)
        {
            ShowcaseStep step = new ViewShowcaseStep(targetView)
            {
                DismissText = dismissText,
                ContentText = content,
                FadeInDuration = duration,
                FadeOutDuration = duration
            };

            AddStep(step);

            return step;
        }

        public ShowcaseStep AddStep(Activity activity, int targetViewId, string content, string dismissText, long duration)
        {
            ShowcaseStep step = new ViewShowcaseStep(activity, targetViewId)
            {
                DismissText = dismissText,
                ContentText = content,
                FadeInDuration = duration,
                FadeOutDuration = duration
            };

            AddStep(step);

            return step;
        }

        public ShowcaseStep AddStep(View targetView, string content, string dismissText, long duration, long delay)
        {
            ShowcaseStep step = new ViewShowcaseStep(targetView)
            {
                DismissText = dismissText,
                ContentText = content,
                FadeInDuration = duration,
                FadeOutDuration = duration,
                Delay = delay
            };

            AddStep(step);

            return step;
        }

        public ShowcaseStep AddStep(Activity activity, int targetViewId, string content, string dismissText, long duration, long delay)
        {
            ShowcaseStep step = new ViewShowcaseStep(activity, targetViewId)
            {
                DismissText = dismissText,
                ContentText = content,
                FadeInDuration = duration,
                FadeOutDuration = duration,
                Delay = delay
            };

            AddStep(step);

            return step;
        }

        public ShowcaseStep AddStep(ShowcaseStep step)
        {
            steps.Add(step);

            return step;
        }

        internal bool HasFired(Context context)
        {
            return ShowcasePreferences.HasFired(context, ShowcaseId);
        }

        internal void SetFired(Context context, bool value)
        {
            ShowcasePreferences.SetFired(context, ShowcaseId, value);
        }

        public static void Reset(Context context, string showcaseId)
        {
            ShowcasePreferences.Reset(context, showcaseId);
        }

        public static void ResetAll(Context context)
        {
            ShowcasePreferences.ResetAll(context);
        }

        internal void LastStep(Context context)
        {
            position = Math.Max(-1, ShowcasePreferences.GetStatus(context, ShowcaseId) - 1);
        }

        internal ShowcaseStep NextStep(Context context)
        {
            if (position >= -1 && position < steps.Count - 1)
            {
                ++position;
                ShowcasePreferences.SetStatus(context, ShowcaseId, position);
                return steps[position];
            }
            else
            {
                SetFired(context, true);
            }

            return null;
        }

        public static Showcase Create(string showcaseId, View targetView, string content, string dismissText)
        {
            var showcase = new Showcase(showcaseId);
            showcase.AddStep(targetView, content, dismissText);
            return showcase;
        }

        public static Showcase Create(string showcaseId, Activity activity, int targetViewId, string content, string dismissText)
        {
            var showcase = new Showcase(showcaseId);
            showcase.AddStep(activity, targetViewId, content, dismissText);
            return showcase;
        }

        public static Showcase Create(View targetView, string content, string dismissText)
        {
            var showcase = new Showcase();
            showcase.AddStep(targetView, content, dismissText);
            return showcase;
        }

        public static Showcase Create(Activity activity, int targetViewId, string content, string dismissText)
        {
            var showcase = new Showcase();
            showcase.AddStep(activity, targetViewId, content, dismissText);
            return showcase;
        }
    }
}
