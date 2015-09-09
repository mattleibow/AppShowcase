using Android.Graphics;

namespace AppExtras.Showcases
{
    public class ShowcaseStep
    {
        public static Color DEFAULT_MASK_COLOUR = Color.ParseColor("#dd335075");
        public static Color DEFAULT_TEXT_COLOUR = Color.ParseColor("#ffffff");
        public static int DEFAULT_RADIUS = 200;

        public static long DEFAULT_FADE_TIME = 300;
        public static long DEFAULT_DELAY = 0;

        public ShowcaseStep()
        {
            DismissText = null;
            DismissTextColor = DEFAULT_TEXT_COLOUR;
            DismissOnTouch = false;

            ContentText = null;
            ContentTextColor = DEFAULT_TEXT_COLOUR;

            Position = new Point(int.MaxValue, int.MaxValue);
            Radius = DEFAULT_RADIUS;
            MaskColor = DEFAULT_MASK_COLOUR;

            Delay = DEFAULT_DELAY;
            FadeInDuration = DEFAULT_FADE_TIME;
            FadeOutDuration = DEFAULT_FADE_TIME;
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
