using System.Drawing;
using System.Windows.Forms;

namespace VPaged.WF.VProperties
{
    public class ButtonStyle
    {
        public ButtonStyle(FlatStyle style = FlatStyle.Flat, int borderRadius = 5, Color? textColor = null, Color? backColor = null, Color? colorActive = null, Color? borderColor = null)
        {
            _Style = style;
            _BorderRadius = borderRadius;
            if (textColor.HasValue)
                _TextColor = textColor.Value;
            else
                _TextColor = Color.Black;

            if (backColor.HasValue)
                _BackColor = backColor.Value;
            else
                _BackColor = Color.SkyBlue;

            if (colorActive.HasValue)
                _ColorActive = colorActive.Value;
            else
                _ColorActive = Color.Orange;

            if (borderColor.HasValue)
                _BorderColor = borderColor.Value;
        }

        private FlatStyle _Style { get; set; }
        private int _BorderRadius { get; set; }
        private Color _BackColor { get; set; }
        private Color _TextColor { get; set; }
        private Color _ColorActive { get; set; }
        private Color? _BorderColor { get; set; }

        public Color? BorderColor
        {
            get
            {
                return _BorderColor;
            }
        }

        public Color ColorActive
        {
            get
            {
                return _ColorActive;
            }
        }

        public FlatStyle Style
        {
            get
            {
                return _Style;
            }
        }

        public int BorderRadius
        {
            get
            {
                return _BorderRadius;
            }
        }

        public Color BackColor
        {
            get
            {
                return _BackColor;
            }
        }

        public Color TextColor
        {
            get
            {
                return _TextColor;
            }
        }
    }
}