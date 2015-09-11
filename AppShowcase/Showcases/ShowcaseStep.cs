using Android.Content;
using Android.Graphics;

namespace AppExtras.Showcases
{
    public class ShowcaseStep
    {
        private static Color DefaultMaskColor = Color.ParseColor("#dd335075");
        private static Color DefaultTextColor = Color.ParseColor("#ffffff");
        private static int DefaultRadius = 200;
        private static long DefaultFadeTime = 500;
        private static long DefaultDelay = 0;

        public ShowcaseStep()
        {
            DismissText = null;
            DismissTextColor = DefaultTextColor;
            DismissOnTouch = false;

            ContentText = null;
            ContentTextColor = DefaultTextColor;

            Position = new Point(int.MaxValue, int.MaxValue);
            Radius = DefaultRadius;
            MaskColor = DefaultMaskColor;

            Delay = DefaultDelay;
            FadeInDuration = DefaultFadeTime;
            FadeOutDuration = DefaultFadeTime;
        }

        // dismiss button

        public virtual string DismissText { get; set; }

        public virtual Color DismissTextColor { get; set; }

        public virtual bool DismissOnTouch { get; set; }

        // content text

        public virtual string ContentText { get; set; }

        public virtual Color ContentTextColor { get; set; }

        // layout

        public virtual Point Position { get; set; }

        public virtual int Radius { get; set; }

        public virtual Color MaskColor { get; set; }

        // animation

        public virtual long Delay { set; get; }

        public virtual long FadeInDuration { get; set; }

        public virtual long FadeOutDuration { get; set; }
    }
}
