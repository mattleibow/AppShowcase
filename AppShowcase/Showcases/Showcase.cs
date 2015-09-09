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

        public int CurrentStepIndex
        {
            get { return position; }
        }

        public ShowcaseStep CurrentStep
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

        public bool IsRunOnce
        {
            get { return ShowcaseId == null; }
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

        public ShowcaseStep AddStep(ShowcaseStep step)
        {
            steps.Add(step);

            return step;
        }

        public bool HasFired(Context context)
        {
            return ShowcasePreferences.HasFired(context, ShowcaseId);
        }

        public void SetFired(Context context, bool value)
        {
            ShowcasePreferences.SetFired(context, ShowcaseId, value);
        }

        public void Reset(Context context)
        {
            ShowcasePreferences.Reset(context, ShowcaseId);
        }

        public static void ResetAll(Context context)
        {
            ShowcasePreferences.ResetAll(context);
        }

        public void LastStep(Context context)
        {
            position = Math.Max(-1, ShowcasePreferences.GetStatus(context, ShowcaseId) - 1);
        }

        public ShowcaseStep NextStep(Context context)
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
    }
}
