using UI.POI;

namespace Game.POI.Deco
{
    public class PicturesquePoint: GenericDecoPoi
    {
        protected override void LoadForInspect(PanelViewPOI panel)
        {
            base.LoadForInspect(panel);
            panel.AddScreenshotting();
        }
    }
}