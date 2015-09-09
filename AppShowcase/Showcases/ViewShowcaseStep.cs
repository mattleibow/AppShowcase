using System;
using Android.Views;
using Android.App;
using Android.Graphics;

namespace AppExtras.Showcases
{
    public class ViewShowcaseStep : ShowcaseStep
    {
        private readonly View view;

        public ViewShowcaseStep(View view)
        {
            this.view = view;
            Setup();
        }

        public ViewShowcaseStep(Activity activity, int viewId)
        {
            view = activity.FindViewById(viewId);
            Setup();
        }

        private void Setup()
        {
            Padding = 10;
            UseAutoRadius = true;
        }

        public virtual int Padding { get; set; }

        public override Point Position
        {
            get
            {
                var position = base.Position;
                if (view != null)
                {
                    int[] location = new int[2];
                    view.GetLocationInWindow(location);
                    int x = location[0] + view.Width / 2;
                    int y = location[1] + view.Height / 2;
                    position = new Point(x, y);
                }
                return position;
            }
            set { base.Position = value; }
        }

        public virtual bool UseAutoRadius { get; set; }

        public override int Radius
        {
            get
            {
                int radius = base.Radius;
                if (view != null && UseAutoRadius)
                {
                    radius = Math.Max(view.MeasuredHeight, view.MeasuredWidth) / 2;
                    radius += Padding; // add a 10 pixel padding to circle
                }
                return radius;
            }
            set { base.Radius = value; }
        }
    }
}
