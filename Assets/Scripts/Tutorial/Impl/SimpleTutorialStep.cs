using UnityEngine;

namespace Tutorial.Impl
{
    public class SimpleTutorialStep: TextTutorialStep
    {
        public override void ReceiveModalClose()
        {
            MarkDone();
        }
    }
}