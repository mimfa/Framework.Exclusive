using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Aspose.Words.Drawing.Charts;

namespace MiMFa.Engine.Template
{
    public class Templator: ITemplator
    {
        public Control MainControl { get; set; } = null;
        public IPalette Palette { get; set; }
        public bool OnColors { get; set; } = true;
        public bool OnFonts { get; set; } = true;
        public bool OnTabIndices { get; set; } = true;

        public Templator() : this(new PaletteBase())
        {
        }
        public Templator(IPalette mainPalette)
        {
            Palette = mainPalette;
        }

        public ITemplator Update(IPalette mainPalette, Control mainControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            Palette = mainPalette?? Palette;
            return Update(mainControl, nest, toolstrip,exceptControls);
        }
        public ITemplator Update(Control mainControl, int nest = 10,bool toolstrip = true, params object[] exceptControls)
        {
            SetTo(MainControl = mainControl, nest,toolstrip, exceptControls);
            return this;
        }

        public int Get(int oldTabIndex, int newTabIndex) => !OnTabIndices || oldTabIndex < 0 || newTabIndex < 0 ? oldTabIndex :newTabIndex;
        public Font Get(Font oldFont, Font newFont) =>  !OnFonts || newFont == null ? oldFont : oldFont == null? newFont : new Font(newFont.FontFamily, newFont.Size,oldFont.Style);
        public Color Get(Color oldColor,Color newColor) => !OnColors || IsInherit(oldColor) ? oldColor: oldColor.A < 255?Color.FromArgb(oldColor.A, newColor): newColor;
        public bool IsInherit(Color oldColor) => oldColor == Color.Empty || oldColor == Color.Transparent ;

        public int CreateTabIndex(Control forControl)
        {
            var point = forControl.PointToScreen(new Point(forControl.Width/2, forControl.Height / 2));
            return Get(forControl.TabIndex, point.Y*point.X);
        }

        public virtual void SetTo(Control control, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (Palette == null || control == null || control is ITemplateIgnorer) return;
            //if (control.Controls == null || control.Controls.Count < 1)
            //{
            //    control.Font = Get(control.Font, Palette.InputFont);
            //    control.BackColor = Get(control.BackColor, Palette.InputBackColor);
            //    control.ForeColor = Get(control.ForeColor, Palette.InputForeColor);
            //}
            if (!exceptControls.Any(v => v == control))
            {
                control.Font = Get(control.Font, Palette.Font);
                control.BackColor = Get(control.BackColor, Palette.BackColor);
                control.ForeColor = Get(control.ForeColor, Palette.ForeColor);

                foreach (Control item in control.Controls)
                {
                    bool b = false;
                    if (b = item is Label) SetTo((Label)item, nest - 1, toolstrip, exceptControls);
                    else if (b = item is ListControl) SetTo((ListControl)item, nest - 1, toolstrip, exceptControls);
                    else if (b = item is RadioButton) SetTo((RadioButton)item, nest - 1, toolstrip, exceptControls);
                    else if (b = item is CheckBox) SetTo((CheckBox)item, nest - 1, toolstrip, exceptControls);
                    else if (b = item is TextBoxBase) SetTo((TextBoxBase)item, nest - 1, toolstrip, exceptControls);
                    else if (b = item is ButtonBase) SetTo((ButtonBase)item, nest - 1, toolstrip, exceptControls);
                    else if (b = item is DataGridView) SetTo((DataGridView)item, nest - 1, toolstrip, exceptControls);
                    else if (b = item is TabControl) SetTo((TabControl)item, nest - 1, toolstrip, exceptControls);
                    else if (toolstrip && item is ToolStrip) SetTo((ToolStrip)item, nest - 1, exceptControls);
                    else SetTo(item, nest - 1, toolstrip, exceptControls);
                    if (b && toolstrip)
                        foreach (var m in Service.ControlService.GetAllToolStrips(item, 1))
                            SetTo(m, nest, exceptControls);
                }
                if (toolstrip)
                    foreach (var item in Service.ControlService.GetAllToolStrips(control, 1))
                        SetTo(item, nest, exceptControls);
            }

            if (control is ITemplateApplier) ((ITemplateApplier)control).ApplyTemplate();
        }
        public virtual void SetTo(Label forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            forControl.Font = Get(forControl.Font, Palette.LabelFont);
            forControl.BackColor = Get(forControl.BackColor, Palette.LabelBackColor);
            forControl.ForeColor = Get(forControl.ForeColor, Palette.LabelForeColor);
        }
        public virtual void SetTo(RadioButton forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            forControl.Font = Get(forControl.Font, Palette.InputFont);
            forControl.BackColor = Get(forControl.BackColor, Palette.InputBackColor);
            forControl.ForeColor = Get(forControl.ForeColor, Palette.InputForeColor);
            forControl.FlatAppearance.CheckedBackColor = Get(forControl.FlatAppearance.CheckedBackColor, Palette.SpecialBackColor);
            forControl.TabIndex = CreateTabIndex(forControl);
        }
        public virtual void SetTo(CheckBox forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            forControl.Font = Get(forControl.Font, Palette.InputFont);
            forControl.BackColor = Get(forControl.BackColor, Palette.InputBackColor);
            forControl.ForeColor = Get(forControl.ForeColor, Palette.InputForeColor);
            forControl.FlatAppearance.CheckedBackColor = Get(forControl.FlatAppearance.CheckedBackColor, Palette.SpecialBackColor);
            forControl.TabIndex = CreateTabIndex(forControl);
        }
        public virtual void SetTo(TextBoxBase forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            forControl.Font = Get(forControl.Font, Palette.InputFont);
            forControl.BackColor = Get(forControl.BackColor, Palette.InputBackColor);
            forControl.ForeColor = Get(forControl.ForeColor, Palette.InputForeColor);
            forControl.TabIndex = CreateTabIndex(forControl);
        }
        public virtual void SetTo(ListControl forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            forControl.Font = Get(forControl.Font, Palette.InputFont);
            forControl.BackColor = Get(forControl.BackColor, Palette.InputBackColor);
            forControl.ForeColor = Get(forControl.ForeColor, Palette.InputForeColor);
            forControl.TabIndex = CreateTabIndex(forControl);
        }
        public virtual void SetTo(ButtonBase forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            forControl.Font = Get(forControl.Font, Palette.ButtonFont);
            forControl.BackColor = Get(forControl.BackColor, Palette.ButtonBackColor);
            forControl.ForeColor = Get(forControl.ForeColor, Palette.ButtonForeColor);
            forControl.FlatAppearance.MouseOverBackColor = Get(forControl.FlatAppearance.MouseOverBackColor, Palette.SpecialBackColor);
            forControl.FlatAppearance.CheckedBackColor = Get(forControl.FlatAppearance.CheckedBackColor, Palette.SpecialBackColor);
            forControl.TabIndex = CreateTabIndex(forControl);
        }
        public virtual void SetTo(DataGridView forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            forControl.Font = Get(forControl.Font, Palette.Font);
            var p = (forControl.Parent ?? forControl);
            forControl.BackgroundColor =
            forControl.BackColor = Get(forControl.BackColor, IsInherit(p.BackColor) ? Palette.BackColor : p.BackColor);
            forControl.ForeColor = Get(forControl.ForeColor, IsInherit(p.ForeColor) ? Palette.ForeColor : p.ForeColor);
            forControl.EnableHeadersVisualStyles = false;
            forControl.TabIndex = CreateTabIndex(forControl);

            foreach (DataGridViewColumn item in forControl.Columns)
                if (item.HasDefaultCellStyle)
                {
                    item.DefaultCellStyle.Font = Get(item.DefaultCellStyle.Font, Palette.Font);
                    item.DefaultCellStyle.BackColor = Get(item.DefaultCellStyle.BackColor, Palette.BackColor);
                    item.DefaultCellStyle.SelectionBackColor = Get(item.DefaultCellStyle.SelectionBackColor, Palette.SpecialBackColor);
                    item.DefaultCellStyle.ForeColor = Get(item.DefaultCellStyle.ForeColor, Palette.ForeColor);
                    item.DefaultCellStyle.SelectionForeColor = Get(item.DefaultCellStyle.SelectionForeColor, Palette.SpecialForeColor);
                }
            foreach (DataGridViewRow item in forControl.Rows)
                if (item.HasDefaultCellStyle)
                {
                    item.DefaultCellStyle.Font = Get(item.DefaultCellStyle.Font, Palette.InputFont);
                    item.DefaultCellStyle.BackColor = Get(item.DefaultCellStyle.BackColor, Palette.InputBackColor);
                    item.DefaultCellStyle.SelectionBackColor = Get(item.DefaultCellStyle.SelectionBackColor, Palette.SpecialBackColor);
                    item.DefaultCellStyle.ForeColor = Get(item.DefaultCellStyle.ForeColor, Palette.InputForeColor);
                    item.DefaultCellStyle.SelectionForeColor = Get(item.DefaultCellStyle.SelectionForeColor, Palette.SpecialForeColor);
                }

            var pc = new Graphic.ProcessColor();

            if (forControl.ReadOnly)
            {
                forControl.RowsDefaultCellStyle.Font =
                forControl.RowHeadersDefaultCellStyle.Font =
                forControl.ColumnHeadersDefaultCellStyle.Font =
                forControl.AlternatingRowsDefaultCellStyle.Font =
                forControl.DefaultCellStyle.Font =
                    Palette.Font;

                forControl.RowHeadersDefaultCellStyle.BackColor =
                forControl.ColumnHeadersDefaultCellStyle.BackColor =
                    Palette.BackColor;

                forControl.AlternatingRowsDefaultCellStyle.BackColor =
                forControl.RowsDefaultCellStyle.BackColor =
                forControl.DefaultCellStyle.BackColor =
                forControl.BackColor =
                    Palette.BackColor;

                forControl.RowsDefaultCellStyle.SelectionBackColor =
                forControl.RowHeadersDefaultCellStyle.SelectionBackColor =
                forControl.ColumnHeadersDefaultCellStyle.SelectionBackColor =
                forControl.AlternatingRowsDefaultCellStyle.SelectionBackColor =
                forControl.DefaultCellStyle.SelectionBackColor =
                    Palette.ButtonBackColor;

                forControl.RowHeadersDefaultCellStyle.ForeColor =
                forControl.ColumnHeadersDefaultCellStyle.ForeColor =
                    Palette.ForeColor;

                forControl.RowsDefaultCellStyle.ForeColor =
                forControl.AlternatingRowsDefaultCellStyle.ForeColor =
                forControl.DefaultCellStyle.ForeColor =
                forControl.ForeColor =
                    Palette.ForeColor;

                forControl.RowsDefaultCellStyle.SelectionForeColor =
                forControl.RowHeadersDefaultCellStyle.SelectionForeColor =
                forControl.ColumnHeadersDefaultCellStyle.SelectionForeColor =
                forControl.AlternatingRowsDefaultCellStyle.SelectionForeColor =
                forControl.DefaultCellStyle.SelectionForeColor =
                    Palette.ButtonForeColor;
            }
            else
            {
                forControl.RowsDefaultCellStyle.Font =
                forControl.RowHeadersDefaultCellStyle.Font =
                forControl.ColumnHeadersDefaultCellStyle.Font =
                    Palette.Font;

                forControl.AlternatingRowsDefaultCellStyle.Font =
                forControl.DefaultCellStyle.Font =
                    Palette.InputFont;

                forControl.RowHeadersDefaultCellStyle.BackColor =
                forControl.ColumnHeadersDefaultCellStyle.BackColor =
                    Palette.BackColor;

                forControl.AlternatingRowsDefaultCellStyle.BackColor =
                    pc.Light(Palette.InputBackColor, -8);

                forControl.RowsDefaultCellStyle.BackColor =
                forControl.DefaultCellStyle.BackColor =
                forControl.BackColor =
                    Palette.InputBackColor;

                forControl.AlternatingRowsDefaultCellStyle.SelectionBackColor =
                forControl.RowsDefaultCellStyle.SelectionBackColor =
                forControl.RowHeadersDefaultCellStyle.SelectionBackColor =
                forControl.ColumnHeadersDefaultCellStyle.SelectionBackColor =
                forControl.DefaultCellStyle.SelectionBackColor =
                    Palette.SpecialBackColor;

                forControl.RowHeadersDefaultCellStyle.ForeColor =
                forControl.ColumnHeadersDefaultCellStyle.ForeColor =
                    Palette.ForeColor;

                forControl.AlternatingRowsDefaultCellStyle.ForeColor =
                forControl.RowsDefaultCellStyle.ForeColor =
                forControl.DefaultCellStyle.ForeColor =
                forControl.ForeColor =
                    Palette.InputForeColor;

                forControl.AlternatingRowsDefaultCellStyle.SelectionForeColor =
                forControl.RowsDefaultCellStyle.SelectionForeColor =
                forControl.RowHeadersDefaultCellStyle.SelectionForeColor =
                forControl.ColumnHeadersDefaultCellStyle.SelectionForeColor =
                forControl.DefaultCellStyle.SelectionForeColor =
                    Palette.SpecialForeColor;
            }
        }
        public virtual void SetTo(TabControl forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            forControl.Font = Get(forControl.Font, Palette.Font);
            var p = (forControl.Parent ?? forControl);
            //forControl.BackColor = Get(forControl.BackColor, IsInherit(p.BackColor) ? Palette.MenuBackColor : p.BackColor);
            //forControl.ForeColor = Get(forControl.ForeColor, IsInherit(p.ForeColor) ? Palette.MenuForeColor : p.ForeColor);
            forControl.TabIndex = CreateTabIndex(forControl);
            var pis = forControl.GetType().GetProperties();
            var pb = IsInherit(p.BackColor) ? Palette.MenuBackColor : p.BackColor;
            var pf = IsInherit(p.ForeColor) ? Palette.MenuForeColor : p.ForeColor;
            var c = typeof(Color);
            foreach (var item in pis)
                if (item.PropertyType == c)
                    if (item.Name.EndsWith("BackColor"))
                        item.SetValue(forControl, Get((Color)item.GetValue(forControl), pb));
                    else if (item.Name.EndsWith("ForeColor"))
                        item.SetValue(forControl, Get((Color)item.GetValue(forControl), pf));

            foreach (var item in forControl.TabPages)
                       SetTo((Control)item, nest - 1, toolstrip, exceptControls);
        }

        public virtual void SetTo(ContextMenuStrip forControl, int nest = 10, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == forControl))
            {
                forControl.Font = Get(forControl.Font, Palette.MenuFont);
                forControl.BackColor = Get(forControl.BackColor, Palette.MenuBackColor);
                forControl.ForeColor = Get(forControl.ForeColor, Palette.MenuForeColor);
                forControl.TabIndex = CreateTabIndex(forControl);
                SetTo(forControl.Items, nest - 1, exceptControls);
            }
        }
        public virtual void SetTo(ToolStrip forControl, int nest = 10, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == forControl))
            {
                forControl.Font = Get(forControl.Font, Palette.MenuFont);
                forControl.BackColor = Get(forControl.BackColor, Palette.MenuBackColor);
                forControl.ForeColor = Get(forControl.ForeColor, Palette.MenuForeColor);
                forControl.TabIndex = CreateTabIndex(forControl);
                SetTo(forControl.Items, nest - 1, exceptControls);
            }
        }
        public virtual void SetTo(ToolStripItemCollection forControl, int nest = 10, params object[] exceptControls)
        {
            foreach (ToolStripItem item in forControl)
                SetTo(item, nest, exceptControls);
        }
        public virtual void SetTo(ToolStripItem forControl, int nest = 10, params object[] exceptControls)
        {
            if (forControl is ToolStripDropDownItem) SetTo((ToolStripDropDownItem)forControl, nest, exceptControls);
            if (forControl is ToolStripMenuItem) SetTo((ToolStripMenuItem)forControl, nest, exceptControls);
            else if (forControl is ToolStripButton) SetTo((ToolStripButton)forControl, nest, exceptControls);
            else if (forControl is ToolStripTextBox) SetTo((ToolStripTextBox)forControl, nest, exceptControls);
            else if (forControl is ToolStripComboBox) SetTo((ToolStripComboBox)forControl, nest, exceptControls);
        }
        public virtual void SetTo(ToolStripDropDownItem forControl, int nest = 10, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == forControl))
            {
                forControl.Font = Get(forControl.Font, Palette.MenuFont);
                forControl.BackColor = Get(forControl.BackColor, Palette.MenuBackColor);
                forControl.ForeColor = Get(forControl.ForeColor, Palette.MenuForeColor);
                SetTo(forControl.DropDownItems, nest - 1, exceptControls);
            }
        }
        public virtual void SetTo(ToolStripMenuItem forControl, int nest = 10, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == forControl))
            {
                forControl.Font = Get(forControl.Font, Palette.MenuFont);
                forControl.BackColor = Get(forControl.BackColor, Palette.MenuBackColor);
                forControl.ForeColor = Get(forControl.ForeColor, Palette.MenuForeColor);
                SetTo(forControl.DropDownItems, nest - 1, exceptControls);
            }
        }
        public virtual void SetTo(ToolStripButton forControl, int nest = 10, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == forControl))
            {
                forControl.Font = Get(forControl.Font, Palette.MenuFont);
                forControl.BackColor = Get(forControl.BackColor, Palette.MenuBackColor);
                forControl.ForeColor = Get(forControl.ForeColor, Palette.MenuForeColor);
            }
        }
        public virtual void SetTo(ToolStripTextBox forControl, int nest = 10, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == forControl))
            {
                forControl.Font = Get(forControl.Font, Palette.MenuFont);
                forControl.BackColor = Get(forControl.BackColor, Palette.MenuBackColor);
                forControl.ForeColor = Get(forControl.ForeColor, Palette.MenuForeColor);
            }
        }
        public virtual void SetTo(ToolStripComboBox forControl, int nest = 10, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == forControl))
            {
                forControl.Font = Get(forControl.Font, Palette.MenuFont);
                forControl.BackColor = Get(forControl.BackColor, Palette.MenuBackColor);
                forControl.ForeColor = Get(forControl.ForeColor, Palette.MenuForeColor);
            }
        }

    }
}
