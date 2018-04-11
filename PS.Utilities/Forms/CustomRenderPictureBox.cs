// ------------------------------------------
// CustomRenderPictureBox.cs, PS.Utilities.Forms
// 
// Last updated: 2016/02/03
// 
// Pedro Sequeira
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PS.Utilities.Forms
{
    public class CustomRenderPictureBox : PictureBox
    {
        #region Properties & Indexers

        public CompositingQuality? CompositingQuality { get; set; }

        public InterpolationMode? InterpolationMode { get; set; }

        public SmoothingMode? SmoothingMode { get; set; }

        #endregion

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (SmoothingMode.HasValue)
                pe.Graphics.SmoothingMode = SmoothingMode.Value;
            if (CompositingQuality.HasValue)
                pe.Graphics.CompositingQuality = CompositingQuality.Value;
            if (InterpolationMode.HasValue)
                pe.Graphics.InterpolationMode = InterpolationMode.Value;

            // this line is needed for .net to draw the contents.
            base.OnPaint(pe);
        }
    }
}