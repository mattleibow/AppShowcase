using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;

using AppExtras;
using AppExtras.Showcases;
using AppExtras.ShowcaseAnimations;

namespace AppShowcaseSample
{
    [Activity(Label = "Showcase Example")]
    public class ShowcaseActivity : ActionBarActivity
    {
        private Button mButtonOne;
        private Button mButtonTwo;
        private Button mButtonThree;
        private const string SHOWCASE_ID = "showcase example";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_sequence_example);

            mButtonOne = FindViewById<Button>(Resource.Id.btn_one);
            mButtonTwo = FindViewById<Button>(Resource.Id.btn_two);
            mButtonThree = FindViewById<Button>(Resource.Id.btn_three);

            FindViewById(Resource.Id.btn_one).Click += delegate
            {
                PresentShowcaseShowcase();
            };

            FindViewById(Resource.Id.btn_two).Click += delegate
            {
                PresentShowcaseShowcase();
            };

            FindViewById(Resource.Id.btn_three).Click += delegate
            {
                PresentShowcaseShowcase();
            };

            FindViewById(Resource.Id.btn_reset).Click += delegate
            {
                AppShowcaseView.ResetShowcase(this, SHOWCASE_ID);
                Toast.MakeText(this, "Showcase reset", ToastLength.Short).Show();
            };

            PresentShowcaseShowcase(true);
        }

        private void PresentShowcaseShowcase(bool initial = false)
        {
            var showcase = new Showcase();
            showcase.ShowcaseId = SHOWCASE_ID;
            var step1 = showcase.AddStep(mButtonOne, "This is button one", "GOT IT");
            step1.FadeInDuration = 1000;
            var step2 = showcase.AddStep(mButtonTwo, "This is button two", "GOT IT");
            step2.FadeInDuration = 1000;
            step2.Delay = 500;
            var step3 = showcase.AddStep(mButtonThree, "This is button three", "GOT IT");
            step3.FadeInDuration = 1000;
            step3.Delay = 500;

            var showcaseView = AppShowcaseView.CreateShowcase(this, showcase);
            showcaseView.AnimationFactory = new AnticipateOvershootAnimationFactory();
            showcaseView.AnimateInitialStep = !initial;
            showcaseView.Show();
        }
    }
}
