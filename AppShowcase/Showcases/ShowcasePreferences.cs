using Android.Content;

namespace AppExtras.Showcases
{
    internal static class ShowcasePreferences
    {
        private const int SequenceNeverStarted = -1;
        private const int SequenceFinished = int.MaxValue;

        private const string PreferencesName = "appshowcase_showcases";
        private const string StatusKeyPart = "status_";

        private static ISharedPreferences GetPreferences(Context context)
        {
            return context.GetSharedPreferences(PreferencesName, FileCreationMode.Private);
        }

        public static bool HasFired(Context context, string showcaseId)
        {
            if (context == null || showcaseId == null)
            {
                return false;
            }

            return GetStatus(context, showcaseId) == SequenceFinished;
        }

        public static void SetFired(Context context, string showcaseId, bool value)
        {
            if (context == null || showcaseId == null)
            {
                return;
            }

            SetStatus(context, showcaseId, value ? SequenceFinished : SequenceNeverStarted);
        }

        public static int GetStatus(Context context, string showcaseId)
        {
            if (context == null || showcaseId == null)
            {
                return SequenceNeverStarted;
            }

            return GetPreferences(context).GetInt(StatusKeyPart + showcaseId, SequenceNeverStarted);
        }

        public static void SetStatus(Context context, string showcaseId, int value)
        {
            if (context == null || showcaseId == null)
            {
                return;
            }

            GetPreferences(context).Edit().PutInt(StatusKeyPart + showcaseId, value).Apply();
        }

        public static void Reset(Context context, string showcaseId)
        {
            if (context == null || showcaseId == null)
            {
                return;
            }

            GetPreferences(context).Edit().PutInt(StatusKeyPart + showcaseId, SequenceNeverStarted).Apply();
        }

        public static void ResetAll(Context context)
        {
            if (context == null)
            {
                return;
            }

            GetPreferences(context).Edit().Clear().Apply();
        }
    }
}
