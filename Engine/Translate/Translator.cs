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
using System.Text.RegularExpressions;
using MiMFa.Engine.Translate;

namespace MiMFa.Engine.Translate
{
    [Serializable]
    public class Translator : ITranslator
    {
        public virtual Language Engine { get; set; } = new Language();
        public virtual string Language { get => Engine.Name; set => Engine = new Language(null,value); }
        public virtual string Lang { get => Engine.Alias; set => Engine = new Language(null,value); }
        public virtual string CharSet { get => Engine.CharSet; set => Engine.CharSet = value; }
        public virtual bool RightToLeft { get => Engine.IsRightToLeft; set => Engine.IsRightToLeft = value; } 

        public Translator() 
        {
            Load();
        }
        public Translator(string path,string name = null)
        {
            Load(path, name);
        }
        public Translator(Language engine)
        {
            Load(engine);
        }

        public ITranslator Load() => Load(new Language());
        public ITranslator Load(string path, string name = null) => Load(new Language(path, name));
        public virtual ITranslator Load(Language engine)
        {
            Engine = engine;
            return this;
        }

        public virtual ITranslator Update(Control mainControl, int nest = 10,bool toolstrip = true, params object[] exceptControls)
        {
            SetTo(mainControl,nest,toolstrip, exceptControls);
            return this;
        }

        public virtual object Get(object key) => Get(key+"");
        public virtual string Get(params string[] keys) => string.Join("",from v in keys select Get(v));
        public virtual string Get(string key) => Engine[key];
        public virtual bool Set(string key, string value) => Engine.AddOrSet(key,value);
        public virtual void SetDirections(Control control)
        {
            if (control.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
            {
                switch (control.Dock)
                {
                    case DockStyle.Left:
                        control.Dock = DockStyle.Right;
                        control.Padding = new Padding(control.Padding.Right, control.Padding.Top, control.Padding.Left, control.Padding.Bottom);
                        control.Margin = new Padding(control.Margin.Right, control.Margin.Top, control.Margin.Left, control.Margin.Bottom);
                        break;
                    case DockStyle.Right:
                        control.Dock = DockStyle.Left;
                        control.Padding = new Padding(control.Padding.Right, control.Padding.Top, control.Padding.Left, control.Padding.Bottom);
                        control.Margin = new Padding(control.Margin.Right, control.Margin.Top, control.Margin.Left, control.Margin.Bottom);
                        break;
                    default:
                        break;
                }

                if (control is FlowLayoutPanel)
                    switch (((FlowLayoutPanel)control).FlowDirection)
                    {
                        case FlowDirection.LeftToRight:
                            ((FlowLayoutPanel)control).FlowDirection = FlowDirection.RightToLeft;
                            break;
                        case FlowDirection.RightToLeft:
                            ((FlowLayoutPanel)control).FlowDirection = FlowDirection.LeftToRight;
                            break;
                        default:
                            break;
                    }
            }
        }

        public virtual IEnumerable<string> GetLanguagesPath()
        {
            if(Engine == null|| Engine.Source == null || Engine.Source.Directory == null) return new string[0];
            return Directory.GetFiles(Engine.Source.Directory, "*"+ Engine.Source.Extension);
        }
        public virtual IEnumerable<string> GetLanguagesName()
        {
            return from v in GetLanguagesPath() select Path.GetFileNameWithoutExtension(v);
        }
        public virtual IEnumerable<Language> GetLanguages()
        {
            return from v in GetLanguagesPath() select new Language(v);
        }

        public virtual void SetTo(Control control, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == control)) return;
            control.Text = Get(control.Text);

            if (control is Form) control.RightToLeft = Engine.IsRightToLeft ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
            else SetDirections(control);
            
            foreach (Control item in control.Controls)
            {
                bool b = false;
                if (b = item is Label) SetTo((Label)item, nest - 1, toolstrip, exceptControls);
                else if (b = item is ComboBox) SetTo((ComboBox)item, nest - 1, toolstrip, exceptControls);
                else if (b = item is RadioButton) SetTo((RadioButton)item, nest - 1, toolstrip, exceptControls);
                else if (b = item is CheckBox) SetTo((CheckBox)item, nest - 1, toolstrip, exceptControls);
                else if (b = item is ListBox) SetTo((ListBox)item, nest - 1, toolstrip, exceptControls);
                else if (b = item is TextBoxBase) SetTo((TextBoxBase)item, nest - 1, toolstrip, exceptControls);
                else if (b = item is ButtonBase) SetTo((ButtonBase)item, nest - 1, toolstrip, exceptControls);
                else if (b = item is DataGridView) SetTo((DataGridView)item, nest - 1, toolstrip, exceptControls);
                else if (b = item is ToolStrip) SetTo((ToolStrip)item);
                else SetTo(item, nest - 1, toolstrip, exceptControls);
                if (b)
                {
                    SetDirections(item);
                    if (toolstrip)
                        foreach (var m in Service.ControlService.GetAllToolStrips(item, 1))
                            SetTo(m, nest, exceptControls);
                }
            }

            if (toolstrip)
                foreach (var item in Service.ControlService.GetAllToolStrips(control, 1))
                    SetTo(item, nest, exceptControls);
        }
        public virtual void SetTo(Label forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            forControl.Text = Get(forControl.Text);
        }
        public virtual void SetTo(RadioButton forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            forControl.Text = Get(forControl.Text);
        }
        public virtual void SetTo(CheckBox forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            forControl.Text = Get(forControl.Text);
        }
        public virtual void SetTo(TextBoxBase forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            //forControl.Text = Get(forControl.Text);
        }
        public virtual void SetTo(ComboBox forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            forControl.Text = Get(forControl.Text);
            if (forControl.Items.Count > 0 && forControl.Items[0] is string)
                for (int i = 0; i < forControl.Items.Count; i++)
                    forControl.Items[i] = Get(forControl.Items[i]);
        }
        public virtual void SetTo(ListBox forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            forControl.Text = Get(forControl.Text);
            if (forControl.Items.Count > 0 && forControl.Items[0] is string)
                for (int i = 0; i < forControl.Items.Count; i++)
                    forControl.Items[i] = Get(forControl.Items[i]);
        }
        public virtual void SetTo(ButtonBase forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;
            forControl.Text = Get(forControl.Text);
        }
        public virtual void SetTo(DataGridView forControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (exceptControls.Any(v => v == forControl)) return;

            foreach (DataGridViewColumn item in forControl.Columns)
            {
                item.HeaderText = Get(item.HeaderText);
                item.ToolTipText = Get(item.ToolTipText);
            }
            foreach (DataGridViewRow item in forControl.Rows)
            {
                item.ErrorText = Get(item.ErrorText);
            }
        }

        public virtual void SetTo(ContextMenuStrip forControl, int nest = 10, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == forControl))
            {
                forControl.Text = Get(forControl.Text);
                SetTo(forControl.Items, nest - 1, exceptControls);
            }
        }
        public virtual void SetTo(ToolStrip forControl, int nest = 10, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == forControl))
            {
                forControl.Text = Get(forControl.Text);
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
                forControl.Text = Get(forControl.Text);
                forControl.ToolTipText = Get(forControl.ToolTipText);
                SetTo(forControl.DropDownItems, nest - 1, exceptControls);
            }
        }
        public virtual void SetTo(ToolStripMenuItem forControl, int nest = 10, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == forControl))
            {
                forControl.Text = Get(forControl.Text);
                forControl.ToolTipText = Get(forControl.ToolTipText);
                SetTo(forControl.DropDownItems, nest - 1, exceptControls);
            }
        }
        public virtual void SetTo(ToolStripButton forControl, int nest = 10, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == forControl))
            {
                forControl.Text = Get(forControl.Text);
                forControl.ToolTipText = Get(forControl.ToolTipText);
            }
        }
        public virtual void SetTo(ToolStripTextBox forControl, int nest = 10, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == forControl))
            {
                forControl.Text = Get(forControl.Text);
                forControl.ToolTipText = Get(forControl.ToolTipText);
            }
        }
        public virtual void SetTo(ToolStripComboBox forControl, int nest = 10, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == forControl))
            {
                forControl.Text = Get(forControl.Text);
                forControl.ToolTipText = Get(forControl.ToolTipText);
            }
        }

    }
}
