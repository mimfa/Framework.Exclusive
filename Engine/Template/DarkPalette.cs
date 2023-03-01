using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Engine.Template
{
    public class DarkPalette : PaletteBase
    {
        public override Font Font { get; set; } = new System.Drawing.Font("Dubai", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public override Font BigFont { get; set; } = new System.Drawing.Font("Dubai", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public override Font SmallFont { get; set; } = new System.Drawing.Font("Dubai", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public override Font MenuFont { get; set; } = new System.Drawing.Font("Dubai", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public override Font ButtonFont { get; set; } = new System.Drawing.Font("Dubai", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public override Font InputFont { get; set; } = new System.Drawing.Font("Dubai", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public override Font SpecialFont { get; set; } = new System.Drawing.Font("Dubai", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

        public override Color BackColor { get; set; } = Color.FromArgb(55, 55, 55);
        public override Color ForeColor { get; set; } = Color.FromArgb(235, 235, 235);
        public override Color MenuBackColor { get; set; } = Color.FromArgb(55, 55, 55);
        public override Color InputBackColor { get; set; } = Color.FromArgb(45, 45, 45);
        public override Color ButtonBackColor { get; set; } = Color.FromArgb(65, 65, 65);
        public override Color SpecialBackColor { get; set; } = Color.FromArgb(65, 65, 65);
        public override Color MenuForeColor { get; set; } = Color.FromArgb(215, 215, 215);
        public override Color InputForeColor { get; set; } = Color.FromArgb(225, 225, 225);
        public override Color ButtonForeColor { get; set; } = Color.FromArgb(245, 245, 245);
        public override Color SpecialForeColor { get; set; } = Color.FromArgb(245, 245, 245);

        public override Color FirstSpecialBackColor { get; set; } = Color.FromArgb(35, 127, 127);
        public override Color SecondSpecialBackColor { get; set; } = Color.FromArgb(127, 35, 127);
        public override Color ThirdSpecialBackColor { get; set; } = Color.FromArgb(127, 127, 35);
        public override Color FirstSpecialForeColor { get; set; } = Color.FromArgb(235, 235, 235);
        public override Color SecondSpecialForeColor { get; set; } = Color.FromArgb(235, 235, 235);
        public override Color ThirdSpecialForeColor { get; set; } = Color.FromArgb(235, 235, 235);
    }
}
