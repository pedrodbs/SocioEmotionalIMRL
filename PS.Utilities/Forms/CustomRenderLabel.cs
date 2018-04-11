// ------------------------------------------
// CustomRenderLabel.cs, PS.Utilities.Forms
// 
// Last updated: 2016/02/03
// 
// Pedro Sequeira
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using System.Drawing.Text;
using System.Windows.Forms;

namespace PS.Utilities.Forms
{
    public class CustomRenderLabel : Label
    {
        #region Properties & Indexers

        public TextRenderingHint TextRenderingHint { get; set; } = TextRenderingHint.ClearTypeGridFit;

        #endregion

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.TextRenderingHint = TextRenderingHint;
            base.OnPaint(pe);
        }
    }
}