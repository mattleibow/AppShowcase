using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;

using AppExtras;

[assembly: Application(Label = "App Showcase", Theme = "@style/Theme.AppCompat.Light.DarkActionBar", Icon = "@drawable/ic_launcher")]

namespace AppShowcaseSample
{
    [Activity(Label = "App Showcase", MainLauncher = true)]
    public class MainActivity : ActionBarActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            FindViewById(Resource.Id.btn_simple_example).Click += delegate
            {
                StartActivity(new Intent(this, typeof(SimpleSingleExample)));
            };
            FindViewById(Resource.Id.btn_custom_example).Click += delegate
            {
                StartActivity(new Intent(this, typeof(CustomActivity)));
            };
            FindViewById(Resource.Id.btn_showcase_example).Click += delegate
            {
                StartActivity(new Intent(this, typeof(ShowcaseActivity)));
            };
            FindViewById(Resource.Id.btn_reset_all).Click += delegate
            {
                AppShowcaseView.ResetAllShowcases(this);
                Toast.MakeText(this, "All Showcases reset", ToastLength.Short).Show();
            };
        }
    }
}
