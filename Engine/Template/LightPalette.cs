using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Engine.Template
{
    public class LightPalette : PaletteBase
    {
        public override Font Font { get; set; } = new System.Drawing.Font("Dubai", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public override Font BigFont { get; set; } = new System.Drawing.Font("Dubai", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public override Font SmallFont { get; set; } = new System.Drawing.Font("Dubai", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public override Font MenuFont { get; set; } = new System.Drawing.Font("Dubai", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public override Font ButtonFont { get; set; } = new System.Drawing.Font("Dubai", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public override Font InputFont { get; set; } = new System.Drawing.Font("Dubai", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public override Font SpecialFont { get; set; } = new System.Drawing.Font("Dubai", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

        public override Color BackColor { get; set; } = Color.FromArgb(240, 240, 240);
        public override Color ForeColor { get; set; } = Color.FromArgb(40, 40, 40);

        public override Color MenuBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public override Color InputBackColor { get; set; } = Color.FromArgb(252, 252, 252);
        public override Color ButtonBackColor { get; set; } = Color.FromArgb(248, 248, 248);
        public override Color SpecialBackColor { get; set; } = Color.FromArgb(248, 248, 248);
        public override Color MenuForeColor { get; set; } = Color.FromArgb(40, 40, 40);
        public override Color InputForeColor { get; set; } = Color.FromArgb(30, 30, 30);
        public override Color ButtonForeColor { get; set; } = Color.FromArgb(20, 20, 20);
        public override Color SpecialForeColor { get; set; } = Color.FromArgb(20, 20, 20);

        public override Color FirstSpecialBackColor { get; set; } = Color.FromArgb(220, 127, 127);
        public override Color SecondSpecialBackColor { get; set; } = Color.FromArgb(127, 220, 127);
        public override Color ThirdSpecialBackColor { get; set; } = Color.FromArgb(127, 127, 220);
        public override Color FirstSpecialForeColor { get; set; } = Color.FromArgb(40, 40, 40);
        public override Color SecondSpecialForeColor { get; set; } = Color.FromArgb(40, 40, 40);
        public override Color ThirdSpecialForeColor { get; set; } = Color.FromArgb(40, 40, 40);
    }
}
