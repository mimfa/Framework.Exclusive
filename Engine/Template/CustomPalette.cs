using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Engine.Template
{
    public class CustomPalette : PaletteBase
    {
        public CustomPalette(Color backColor, Color foreColor, Font font = null, Color? specialBackColor = null, Color? specialForeColor = null, Font specialFont = null)
        {
            UpdateFont(font);
            SpecialFont = specialFont?? SpecialFont;
            MiMFa.Graphic.ProcessColor pi = new Graphic.ProcessColor();
            BackColor = backColor;
            ForeColor = foreColor;
            MenuBackColor = pi.Contrast(backColor, 5);
            MenuForeColor = foreColor;
            InputBackColor = pi.Contrast(backColor,12);
            InputForeColor = pi.Contrast(foreColor, -10);
            ButtonBackColor = pi.Contrast(backColor, 8);
            ButtonForeColor = pi.Contrast(foreColor, -20);
            SpecialBackColor = specialBackColor?? pi.Contrast(backColor, 8);
            SpecialForeColor = specialForeColor?? pi.Contrast(foreColor, -20);


            specialBackColor = specialBackColor ?? backColor;
            specialForeColor = specialForeColor ?? foreColor;
            FirstSpecialBackColor = Color.FromArgb(specialBackColor.Value.A, specialBackColor.Value.R/2, specialBackColor.Value.G, specialBackColor.Value.B);
            FirstSpecialForeColor = Color.FromArgb(specialForeColor.Value.A, Math.Min(255, specialForeColor.Value.R*2), specialForeColor.Value.G, specialForeColor.Value.B);
            SecondSpecialBackColor = Color.FromArgb(specialBackColor.Value.A, specialBackColor.Value.R, specialBackColor.Value.G/2, specialBackColor.Value.B);
            SecondSpecialForeColor = Color.FromArgb(specialForeColor.Value.A, specialForeColor.Value.R, Math.Min(255, specialForeColor.Value.G*2), specialForeColor.Value.B);
            ThirdSpecialBackColor = Color.FromArgb(specialBackColor.Value.A, specialBackColor.Value.R, specialBackColor.Value.G, specialBackColor.Value.B / 2);
            ThirdSpecialForeColor = Color.FromArgb(specialForeColor.Value.A, specialForeColor.Value.R, specialForeColor.Value.G, Math.Min(255, specialForeColor.Value.B*2));
        }

    }
}
