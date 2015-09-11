using Android.OS;
using Android.Views;
using Android.Widget;
using Fragment = Android.Support.V4.App.Fragment;

using AppExtras;
using AppExtras.Showcases;
using AppExtras.Renderers;

namespace AppShowcaseSample
{
    public class MainFragment : Fragment
    {
        private const string ShowcaseId = "app-intro";

        private Button customizedButton;
        private Button resetButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;

            var view = inflater.Inflate(Resource.Layout.FragmentLayout, container, false);

            customizedButton = view.FindViewById<Button>(Resource.Id.customizedButton);
            customizedButton.Click += delegate
            {
                DisplayCustomizedShowcase();
            };

            resetButton = view.FindViewById<Button>(Resource.Id.resetButton);
            resetButton.Click += delegate
            {
                Showcase.ResetAll(Activity);
                Toast.MakeText(Activity, "The showcase was reset", ToastLength.Short).Show();
            };

            DisplayShowcase();

            return view;
        }

        private void DisplayShowcase()
        {
            var showcase = new Showcase(ShowcaseId);

            // highlight an action bar using a resource ID
            var actionBarStep = showcase.AddStep(Activity, Resource.Id.addMenuItem, "This button was added by the Activity.", "GOT IT");
            var fragmentActionBarStep = showcase.AddStep(Activity, Resource.Id.closeMenuItem, "This is button was added by the fragment.", "GOT IT");

            // a normal steps for a view reference
            var dialogStep = showcase.AddStep(customizedButton, "This button will display a showcase that has customizations.", "GOT IT");
            var resetStep = showcase.AddStep(resetButton, "This button will reset the showcases so that they will appear again.", "GOT IT");

            var showcaseView = AppShowcaseView.Create(Activity, showcase);

            // show with a delay so the app UI can finish rendering
            showcaseView.Show(500);
        }

        private void DisplayCustomizedShowcase()
        {
            // no ID means that we won't know if we have already launched this showcase
            var showcase = new Showcase();

            // highlight an action bar using a resource ID
            var actionBarStep = showcase.AddStep(Activity, Resource.Id.addMenuItem, "Tapping anywhere will dismiss this step.", null);
            actionBarStep.DismissOnTouch = true;

            // a customized mask
            var dialogStep = showcase.AddStep(customizedButton, "This step uses different colors for the text.", "GOT IT");
            dialogStep.ContentTextColor = Resources.GetColor(Resource.Color.green);
            dialogStep.DismissTextColor = Resources.GetColor(Resource.Color.orange);

            // a normal step for a view reference
            var resetStep = showcase.AddStep(resetButton, "This step has a different mask color.", "GOT IT");
            resetStep.MaskColor = Resources.GetColor(Resource.Color.purple);

            var showcaseView = AppShowcaseView.Create(Activity, showcase);

            // use a bouncy animation
            showcaseView.Animation = new AnticipateOvershootRenderer();

            // show events
            showcaseView.ShowcaseStarted += delegate
            {
                Toast.MakeText(Activity, "Showcase started", ToastLength.Short).Show();
            };
            showcaseView.StepDisplayed += (sender, e) =>
            {
                Toast.MakeText(Activity, "Showing step: " + e.StepIndex, ToastLength.Short).Show();
            };
            showcaseView.StepDismissed += (sender, e) =>
            {
                Toast.MakeText(Activity, "Hiding step: " + e.StepIndex, ToastLength.Short).Show();
            };
            showcaseView.ShowcaseCompleted += delegate
            {
                Toast.MakeText(Activity, "Showcase completed", ToastLength.Short).Show();
            };

            // no need for the delay as everything is done
            showcaseView.Show();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.FragmentOptionsMenu, menu);
        }
    }
}
