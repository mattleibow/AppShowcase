using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;

using AppExtras;
using AppExtras.Showcases;

namespace AppShowcaseSample
{
    [Activity(Label = "Custom Example")]
    public class CustomActivity : ActionBarActivity
    {
        private Button mButtonShow;
        private const string SHOWCASE_ID = "custom example";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_custom_example);

            mButtonShow = FindViewById<Button>(Resource.Id.btn_show);

            mButtonShow.Click += delegate
            {
                PresentShowcaseView(0);
            };
            FindViewById(Resource.Id.btn_reset).Click += delegate
            {
                AppShowcaseView.ResetShowcase(this, SHOWCASE_ID);
                Toast.MakeText(this, "Showcase reset", ToastLength.Short).Show();
            };

            PresentShowcaseView(500); // half second delay
        }

        private void PresentShowcaseView(int withDelay)
        {
            var showcase = new Showcase();
            showcase.ShowcaseId = SHOWCASE_ID;
            var step = showcase.AddStep(mButtonShow, "This is some amazing feature you should know about", "GOT IT");
            step.Delay = withDelay;
            step.ContentTextColor = Resources.GetColor(Resource.Color.green);
            step.MaskColor = Resources.GetColor(Resource.Color.purple);

            var showcaseView = AppShowcaseView.CreateShowcase(this, showcase);
            showcaseView.Show();
        }
    }
}
