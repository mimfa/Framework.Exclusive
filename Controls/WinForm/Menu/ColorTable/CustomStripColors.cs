using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Controls.WinForm.Menu.ColorTable
{
    public class CustomStripColors : PaletteStripColors
    {
        public CustomStripColors(Color backColor, Color foreColor, Font font = null) : base(new MiMFa.Engine.Template.CustomPalette(backColor, foreColor, font))
        { }
    }
}
