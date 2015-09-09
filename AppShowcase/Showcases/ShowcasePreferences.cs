using Android.Content;

namespace AppExtras.Showcases
{
    public static class ShowcasePreferences
    {
        public static int SEQUENCE_NEVER_STARTED = -1;
        public static int SEQUENCE_FINISHED = int.MaxValue;

        private const string PREFS_NAME = "appshowcase_showcases";
        private const string STATUS = "status_";

        private static ISharedPreferences GetPreferences(Context context)
        {
            return context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private);
        }

        public static bool HasFired(Context context, string showcaseId)
        {
            if (context == null || showcaseId == null)
            {
                return false;
            }

            return GetStatus(context, showcaseId) == SEQUENCE_FINISHED;
        }

        public static void SetFired(Context context, string showcaseId, bool value)
        {
            if (context == null || showcaseId == null)
            {
                return;
            }

            SetStatus(context, showcaseId, value ? SEQUENCE_FINISHED : SEQUENCE_NEVER_STARTED);
        }

        public static int GetStatus(Context context, string showcaseId)
        {
            if (context == null || showcaseId == null)
            {
                return SEQUENCE_NEVER_STARTED;
            }

            return GetPreferences(context).GetInt(STATUS + showcaseId, SEQUENCE_NEVER_STARTED);
        }

        public static void SetStatus(Context context, string showcaseId, int value)
        {
            if (context == null || showcaseId == null)
            {
                return;
            }

            GetPreferences(context).Edit().PutInt(STATUS + showcaseId, value).Apply();
        }

        public static void Reset(Context context, string showcaseId)
        {
            if (context == null || showcaseId == null)
            {
                return;
            }

            GetPreferences(context).Edit().PutInt(STATUS + showcaseId, SEQUENCE_NEVER_STARTED).Apply();
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
