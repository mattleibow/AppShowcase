using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

using AppExtras;
using AppExtras.Showcases;

namespace AppShowcaseSample
{

    [Activity(Label = "Simple Single Example")]
    public class SimpleSingleExample : ActionBarActivity
    {
        private Button mButtonShow;

        private const string SHOWCASE_ID = "simple example";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_simple_single_example);

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

            // only show the showcase after half a second
            PresentShowcaseView(500);
        }

        private void PresentShowcaseView(int withDelay)
        {
            var showcase = new Showcase();
            showcase.ShowcaseId = SHOWCASE_ID; // provide a unique ID used to ensure it is only shown once
            var step = showcase.AddStep(mButtonShow, "This is some amazing feature you should know about", "GOT IT");
            step.Delay = withDelay;

            var showcaseView = AppShowcaseView.CreateShowcase(this, showcase);
            showcaseView.Show();
        }
    }
}
