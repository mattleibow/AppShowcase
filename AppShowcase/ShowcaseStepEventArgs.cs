using AppExtras.Showcases;

namespace AppExtras
{
    public class ShowcaseStepEventArgs
    {
        public ShowcaseStepEventArgs(int stepIndex, ShowcaseStep step)
        {
            StepIndex = stepIndex;
            Step = step;
        }

        public int StepIndex { get; private set; }

        public ShowcaseStep Step { get; private set; }
    }
}