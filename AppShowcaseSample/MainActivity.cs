using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;

namespace AppShowcaseSample
{
    [Activity(Label = "App Showcase", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.DarkActionBar")]
    public class MainActivity : ActionBarActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MainLayout);

            // only add the fragment on a fresh launch
            if (savedInstanceState == null)
            {
                SupportFragmentManager
                    .BeginTransaction()
                    .Add(Resource.Id.fragmentContainer, new MainFragment())
                    .Commit();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.OptionsMenu, menu);
            return true;
        }
    }
}
