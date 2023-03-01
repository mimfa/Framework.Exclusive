using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace MiMFa.Graphic
{
    public class Highlight
    {
        public enum Mode
        {
            BackColor,ForeColor,Border, Shadow, Hybrid
        }

        public Mode HilightMode
        {
            get { return _HilightMode; }
            set { _HilightMode = value; }
        }
        public Color HilightBackColor = Color.WhiteSmoke;
        public Color HilightForeColor = Color.Black;
        public Image HilightBackImage = MiMFa.Properties.Resources.Hover;
        public BorderStyle HilightBorderStyle = BorderStyle.FixedSingle;

        public Highlight(Color hilightBackColor, Color hilightForeColor, Image hilightBackImage = null, bool prevBackImageView = false)
        {
            PrevBackImageView = prevBackImageView;
            HilightBackColor = hilightBackColor;
            HilightForeColor = hilightForeColor;
            HilightBackImage = hilightBackImage;
            if (HilightBackImage == null) HilightMode = Mode.BackColor;
            else if (HilightForeColor == Color.Transparent) HilightMode = Mode.Shadow;
            else HilightMode = Mode.Hybrid;
        }
        public Highlight(Image hilightBackImage = null, bool prevBackImageView = false)
        {
            PrevBackImageView = prevBackImageView;
            HilightBackImage = hilightBackImage;
            if (HilightBackImage == null) HilightMode = Mode.BackColor;
            else HilightMode = Mode.Shadow;
        }
        public Highlight(Mode hilightMode, bool prevBackImageView = false)
        {
            PrevBackImageView = prevBackImageView;
            HilightMode = hilightMode;
            if (HilightMode != Mode.BackColor) HilightBackImage = Properties.Resources.Light3;
        }

        public void AddControls(params Control[] controls)
        {
            if(controls != null)
                for (int i = 0; i < controls.Length; i++)
                {
                    controls[i].MouseEnter += MouseEnter;
                    controls[i].MouseLeave += MouseLeave;
                }
        }

        //target
        public void MouseEnter(object sender, EventArgs e)
        {
            MouseEnter((Control)sender);
        }
        public void MouseLeave(object sender, EventArgs e)
        {
            MouseLeave((Control)sender);
        }

        //action
        public void MouseEnter(Control sender)
        {
            ((UserControl)sender).SuspendLayout();
            switch (_HilightMode)
            {
                case Mode.BackColor:
                    PrevBackColor = sender.BackColor;
                    sender.BackColor = HilightBackColor;
                    break;
                case Mode.ForeColor:
                    PrevForeColor = sender.ForeColor;
                    sender.ForeColor = HilightForeColor;
                    break;
                case Mode.Shadow:
                   if(PrevBackImageView) PrevBackImage = sender.BackgroundImage;
                    sender.BackgroundImage = HilightBackImage;
                    break;
                case Mode.Border:
                    PrevBorderStyle = ((PictureBox)sender).BorderStyle;
                    ((PictureBox)sender).BorderStyle = HilightBorderStyle;
                    break;
                case Mode.Hybrid:
                    PrevForeColor = sender.ForeColor;
                    sender.ForeColor = HilightForeColor;
                    if (PrevBackImageView) PrevBackImage = sender.BackgroundImage;
                    sender.BackgroundImage = HilightBackImage;
                    break;
            }
            ((UserControl)sender).ResumeLayout(true);
        }
        public void MouseLeave(Control sender)
        {
            ((UserControl)sender).SuspendLayout();
            switch (_HilightMode)
            {
                case Mode.BackColor:
                    sender.BackColor = PrevBackColor;
                    break;
                case Mode.ForeColor:
                    sender.ForeColor = PrevForeColor;
                    break;
                case Mode.Shadow:
                    if (PrevBackImageView) sender.BackgroundImage = PrevBackImage;
                    else sender.BackgroundImage = null;
                    break;
                case Mode.Border:
                    ((PictureBox)sender).BorderStyle = PrevBorderStyle;
                    break;
                case Mode.Hybrid:
                    sender.ForeColor = PrevForeColor;
                    if (PrevBackImageView) sender.BackgroundImage = PrevBackImage;
                    else sender.BackgroundImage = null;
                    break;
            }
            ((UserControl)sender).ResumeLayout(true);
        }

        #region Private

        private Mode _HilightMode = Mode.Shadow;
        private Color PrevBackColor = Color.WhiteSmoke;
        private Color PrevForeColor = Color.Black;
        private Image PrevBackImage = null;
        private BorderStyle PrevBorderStyle = BorderStyle.None;
        private bool PrevBackImageView = false;

        #endregion
    }
}
