// ------------------------------------------
// CustomToolStripRenderer.cs, PS.Utilities
// 
// Last updated: 2016/02/12
// 
// Pedro Sequeira
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using System.Drawing;
using System.Windows.Forms;

namespace PS.Utilities.Forms
{
    public class CustomToolStripRenderer : ToolStripProfessionalRenderer
    {
        #region Properties & Indexers

        /// <summary>
        ///     Gets or sets the margin to leave between the separator line and the tool strip item bounds.
        /// </summary>
        public int LineMargin { get; set; } = 10;

        #endregion

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            if (e.Vertical || !(e.Item is ToolStripSeparator))
            {
                base.OnRenderSeparator(e);
            }
            else
            {
                var bounds = new Rectangle(Point.Empty, e.Item.Size);
                using (var brush = new SolidBrush(e.Item.BackColor))
                    e.Graphics.FillRectangle(brush, bounds);

                using (var pen = new Pen(e.Item.ForeColor))
                {
                    var y = e.Item.Height/2;
                    e.Graphics.DrawLine(pen, LineMargin, y, e.Item.Width - LineMargin, y);
                }
            }
        }
    }
}