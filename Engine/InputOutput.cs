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
using MiMFa.Engine.Template;
using Aspose.Words.Drawing.Charts;
using System.Text.RegularExpressions;
using MiMFa.Model;
using MiMFa.Service;
using MiMFa.General;

namespace MiMFa.Engine
{
    [Serializable]
    public class InputOutput
    {
        public string Path { get; set; }
        public bool HasPath => !string.IsNullOrWhiteSpace(Path);
        public bool ExistsPath => HasPath && File.Exists(Path);
        public string ImportFilter { get; set; } = "All Files (*.*)|*.*";
        public string ExportFilter { get; set; } = "All Files (*.*)|*.*";
        public string Separator { get; set; } = "-->";
        public char InlineSeparator { get; set; } = ';';
        public char InItemSeparator { get; set; } = ',';
        public Control MainControl { get => _MainControl; set
                {
                _MainControl = value;
                if (_MainControl != null)
                {
                    _MainControl.AllowDrop = true;
                    _MainControl.DragOver -= DragOver;
                    _MainControl.DragDrop -= DragDrop;
                    _MainControl.DragOver += DragOver;
                    _MainControl.DragDrop += DragDrop;
                }
            } }
        private Control _MainControl  = null;
        public string ConfigurationsPath { get; set; }
        public SmartDictionary<string, string> Configurations { get; set; } = new SmartDictionary<string, string>();
        public bool UnStoredChanges { get; set; } = true;

        public event GenericEventHandler<InputOutput,bool> CreationHandle = (s) => true;
        public event GenericEventHandler<InputOutput, string,string> ImportationHandle = (s, a) => a;
        public event GenericEventHandler<InputOutput, string, string> ExportationHandle = (s, a) => a;
        public event GenericEventHandler<InputOutput, string, string> UpdatingPath = (s, a) => a;

        public InputOutput(string configPath, Control mainControl,
            GenericEventHandler<InputOutput, bool> creationHandle = null,
            GenericEventHandler<InputOutput, string, string> importationHandle = null,
            GenericEventHandler<InputOutput, string, string> exportationHandle = null)
            : this(configPath, "All Files (*.*)|*.*", "All Files (*.*)|*.*", mainControl, creationHandle, importationHandle, exportationHandle)
        {
        }
        public InputOutput(Control mainControl = null,
            GenericEventHandler<InputOutput, bool> creationHandle = null,
            GenericEventHandler<InputOutput, string, string> importationHandle = null,
            GenericEventHandler<InputOutput, string, string> exportationHandle = null)
            : this(Config.ConfigurationPath, "All Files (*.*)|*.*", "All Files (*.*)|*.*", mainControl, creationHandle, importationHandle, exportationHandle)
        {
        }
        public InputOutput(string configPath, string filter, 
            Control mainControl = null,
            GenericEventHandler<InputOutput, bool> creationHandle = null,
            GenericEventHandler<InputOutput, string, string> importationHandle = null,
            GenericEventHandler<InputOutput, string, string> exportationHandle = null)
            : this(Config.ConfigurationPath, filter, filter, mainControl, creationHandle, importationHandle, exportationHandle)
        {
        }
        public InputOutput(string configPath, string inputFilter, string outputFilter,
            Control mainControl = null,
            GenericEventHandler<InputOutput, bool> creationHandle = null,
            GenericEventHandler<InputOutput, string, string> importationHandle = null,
            GenericEventHandler<InputOutput, string, string> exportationHandle = null)
        {
            ConfigurationsPath = configPath;
            ImportFilter = inputFilter;
            ExportFilter = outputFilter;
            CreationHandle = creationHandle ?? CreationHandle;
            ImportationHandle = importationHandle ?? ImportationHandle;
            ExportationHandle = exportationHandle ?? ExportationHandle;
            MainControl = mainControl;
        }


        public virtual bool New()
        {
            if (!CreationHandle(this)) return false;
            UnStoredChanges = false;
            UpdatingPath(this, Path = null);
            return true;
        }
        public virtual string Open()
        {
            return UpdatePath(Import());
        }
        public virtual string Import()
        {
            return Import(DialogService.OpenFile("", ImportFilter));
        }
        public virtual IEnumerable<string> Import(IEnumerable<string> pathes)
        {
            foreach (var item in pathes)
                yield return Import(item);
        }
        public virtual string Import(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return null;
            return ImportationHandle(this, path);
        }
        public virtual bool Save()
        {
            if (HasPath)
                return Export(Path)!=null;
            return SaveAs()!=null;
        }
        public virtual string SaveAs()
        {
            return UpdatePath(Export());
        }
        public virtual string Export()
        {
            return Export(DialogService.SaveFile("", ExportFilter));
        }
        public virtual string Export(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            return ExportationHandle(this, path);
        }
        public virtual bool Exit(string message ="Your project has some changes, Are you sure to close it?")
        {
            if (DialogService.ShowMessage(MessageMode.Warning, message) == DialogResult.Yes)
            {
                Application.Exit();
                return true;
            }
            return false;
        }
        public virtual string UpdatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return null;
            UnStoredChanges = false;
            return UpdatingPath(this,Path = path);
        }


        public void LoadConfiguration()
        {
            Configurations = File.Exists(ConfigurationsPath) ? new SmartDictionary<string, string>( IOService.ReadDictionary(ConfigurationsPath, Separator)) : new SmartDictionary<string, string>();
        }
        public void StoreConfiguration()
        {
            IOService.WriteDictionary(ConfigurationsPath, CollectionService.Sort(Configurations), Separator);
        }

        public void OpenConfigurations(Control mainControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (MainControl == null) MainControl = mainControl;
            LoadConfiguration();
            DialogService.InitialDirectory = GetConfiguration("DialogService.InitialDirectory");
            if (mainControl is Form && ((Form)mainControl).WindowState == FormWindowState.Normal)
            {
                mainControl.Location = ConvertService.ToPoint(GetConfiguration(mainControl.Name + ".Location"), mainControl.Location);
                mainControl.Size = ConvertService.ToSize(GetConfiguration(mainControl.Name + ".Size"), mainControl.Size);
            }
            _OpenConfigurations(mainControl, nest, toolstrip, exceptControls);
        }
        private void _OpenConfigurations(Control mainControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == mainControl))
            {
                if (mainControl.Controls.Count > 0 && (mainControl is Panel || mainControl is TableLayoutPanel || mainControl is GroupBox || mainControl is UserControl || mainControl is Form))
                {
                    if (nest > 0)
                        foreach (Control item in mainControl.Controls)
                            _OpenConfigurations(item, nest - 1, toolstrip, exceptControls);
                    else;
                }
                else //Is Control
                    SetTo(mainControl);
                if (toolstrip)
                    foreach (var item in Service.ControlService.GetAllToolStrips(mainControl, nest))
                        SetTo(item);
            }
        }
        public void SaveConfigurations(Control mainControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (MainControl == null) MainControl = mainControl;
            LoadConfiguration();
            SetConfiguration("DialogService.InitialDirectory", DialogService.InitialDirectory);
            if (mainControl is Form && ((Form)mainControl).WindowState == FormWindowState.Normal)
            {
                SetConfiguration(mainControl.Name + ".Location", ConvertService.ToString(mainControl.Location));
                SetConfiguration(mainControl.Name + ".Size", ConvertService.ToString(mainControl.Size));
            }
            _SaveConfigurations(mainControl, nest , toolstrip, exceptControls);
            StoreConfiguration();
        }
        private void _SaveConfigurations(Control mainControl, int nest = 10, bool toolstrip = true, params object[] exceptControls)
        {
            if (!exceptControls.Any(v => v == mainControl))
            {
                if (mainControl.Controls.Count > 0 &&(mainControl is Panel || mainControl is TableLayoutPanel || mainControl is GroupBox || mainControl is UserControl || mainControl is Form))
                {
                    if (nest > 0)
                        foreach (Control item in mainControl.Controls)
                            _SaveConfigurations(item, nest - 1, toolstrip, exceptControls);
                }
                else //Is Control
                    SetFrom(mainControl);
                if (toolstrip)
                    foreach (var item in Service.ControlService.GetAllToolStrips(mainControl, nest))
                        SetFrom(item);
            }
        }


        public bool HasConfiguration(string key) => Configurations.ContainsKey(key);
        public bool HasConfiguration(Control control) => HasConfiguration(GetName(control));
        public bool HasConfiguration(ToolStripItem control) => HasConfiguration(GetName(control));

        public string GetStringConfiguration(string key, string defaultVal = default(string)) => GetConfiguration(key, s=>s, defaultVal);
        public bool GetBoolConfiguration(string key, bool defaultVal = default(bool)) => GetConfiguration(key, s=>ConvertService.TryToBoolean(s, defaultVal), defaultVal);
        public int GetIntConfiguration(string key, int defaultVal = default(int)) => GetConfiguration(key, s=>ConvertService.TryToInt(s, defaultVal), defaultVal);
        public long GetLongConfiguration(string key, long defaultVal = default(long)) => GetConfiguration(key, s=>ConvertService.TryToLong(s, defaultVal), defaultVal);
        public float GetSingleConfiguration(string key, float defaultVal = default(float)) => GetConfiguration(key, s=>ConvertService.TryToSingle(s, defaultVal), defaultVal);
        public double GetDoubleConfiguration(string key, double defaultVal = default(double)) => GetConfiguration(key, s=>ConvertService.TryToDouble(s, defaultVal), defaultVal);
        public double GetNumberConfiguration(string key, double defaultVal = default(double)) => GetConfiguration(key, s => ConvertService.TryToNumber(s, defaultVal), defaultVal);

        public string GetStringConfiguration(Control control, string defaultVal = default(string)) => GetStringConfiguration(GetName(control), defaultVal);
        public bool GetBoolConfiguration(Control control, bool defaultVal = default(bool)) => GetBoolConfiguration(GetName(control), defaultVal);
        public int GetIntConfiguration(Control control, int defaultVal = default(int)) => GetIntConfiguration(GetName(control), defaultVal);
        public long GetLongConfiguration(Control control, long defaultVal = default(long)) => GetLongConfiguration(GetName(control), defaultVal);
        public float GetSingleConfiguration(Control control, float defaultVal = default(float)) => GetSingleConfiguration(GetName(control), defaultVal);
        public double GetDoubleConfiguration(Control control, double defaultVal = default(double)) => GetDoubleConfiguration(GetName(control), defaultVal);
        public double GetNumberConfiguration(Control control, double defaultVal = default(double)) => GetNumberConfiguration(GetName(control), defaultVal);

        public string GetStringConfiguration(ToolStripItem control, string defaultVal = default(string)) => GetStringConfiguration(GetName(control), defaultVal);
        public bool GetBoolConfiguration(ToolStripItem control, bool defaultVal = default(bool)) => GetBoolConfiguration(GetName(control), defaultVal);
        public int GetIntConfiguration(ToolStripItem control, int defaultVal = default(int)) => GetIntConfiguration(GetName(control), defaultVal);
        public long GetLongConfiguration(ToolStripItem control, long defaultVal = default(long)) => GetLongConfiguration(GetName(control), defaultVal);
        public float GetSingleConfiguration(ToolStripItem control, float defaultVal = default(float)) => GetSingleConfiguration(GetName(control), defaultVal);
        public double GetDoubleConfiguration(ToolStripItem control, double defaultVal = default(double)) => GetDoubleConfiguration(GetName(control), defaultVal);
        public double GetNumberConfiguration(ToolStripItem control, double defaultVal = default(double)) => GetNumberConfiguration(GetName(control), defaultVal);

        public T GetConfiguration<T>(string key, Func<string, T> convertor, T defaultVal = default(T))
        {
            string value = Configurations.GetOrDefault(key, null);
            return value == null ? defaultVal : convertor(value);
        }
        public T GetConfiguration<T>(Control control, Func<string, T> convertor, T defaultVal = default(T)) => GetConfiguration(GetName(control), convertor, defaultVal);
        public T GetConfiguration<T>(ToolStripItem control, Func<string, T> convertor, T defaultVal = default(T)) => GetConfiguration(GetName(control), convertor, defaultVal);
        public string GetConfiguration(string key, string defaultVal =null) => Configurations.GetOrDefault(key, defaultVal);
        public string GetConfiguration(Control control, string defaultVal =null) => GetConfiguration(GetName(control), defaultVal);
        public string GetConfiguration(ToolStripItem control, string defaultVal =null) => GetConfiguration(GetName(control), defaultVal);
        
        public string SetConfiguration<T>(string key, Func<string> convertor) => SetConfiguration(key, convertor());
        public string SetConfiguration<T>(string key, Func<string, string> convertor) => SetConfiguration(key, convertor(key));
        public string SetConfiguration<T>(Control control, Func<string> convertor) => SetConfiguration(GetName(control), convertor());
        public string SetConfiguration<T>(Control control, Func<Control, string> convertor) => SetConfiguration(GetName(control), convertor(control));
        public string SetConfiguration<T>(ToolStripItem control, Func<string> convertor) => SetConfiguration(GetName(control), convertor());
        public string SetConfiguration<T>(ToolStripItem control, Func<ToolStripItem, string> convertor) => SetConfiguration(GetName(control), convertor(control));
        public string SetConfiguration(string key, string value)
        {
            Configurations.AddOrSet(key, value);
            return value;
        }
        public string SetConfiguration(Control control, string value) => SetConfiguration(GetName(control), value);
        public string SetConfiguration(ToolStripItem control, string value) => SetConfiguration(GetName(control), value);
        public string SetConfiguration(string key, object value) => SetConfiguration(key, value+"");
        public string SetConfiguration(Control control, object value) => SetConfiguration(GetName(control), value+"");
        public string SetConfiguration(ToolStripItem control, object value) => SetConfiguration(GetName(control), value+"");

        public virtual string GetName(Control control) => control.Parent == null ? control.Name : GetName(control.Parent) + "." + control.Name;
        public virtual string GetName(ToolStripItem control) => control.Owner == null ? control.Name : GetName(control.Owner) + "." + control.Name;

        public virtual void SetTo(Control control)
        {
            if (control.Controls.Count == 0)
            {
                if (control is ComboBox) SetTo((ComboBox)control);
                else if (control is RadioButton) SetTo((RadioButton)control);
                else if (control is CheckBox) SetTo((CheckBox)control);
                else if (control is ListBox) SetTo((ListBox)control);
                else if (control is TextBoxBase) SetTo((TextBoxBase)control);
                else if (control is ToolStrip) SetTo((ToolStrip)control);
                else if (!(control is Label) && !(control is Button))
                    try
                    {
                        control.Text = GetConfiguration(control, control.Text);
                    }
                    catch { }
            }
            else foreach (Control item in control.Controls)
                    SetTo(item);
        }
        public virtual void SetTo(RadioButton forControl)
        {
            bool b;
            forControl.Checked = GetConfiguration(forControl, v => bool.TryParse(v, out b) ? b : forControl.Checked, forControl.Checked);
        }
        public virtual void SetTo(CheckBox forControl)
        {
            bool b;
            forControl.Checked = GetConfiguration(forControl, v => bool.TryParse(v, out b) ? b : forControl.Checked, forControl.Checked);
        }
        public virtual void SetTo(TextBoxBase forControl)
        {
            forControl.Text = GetConfiguration(forControl, forControl.Text);
        }
        public virtual void SetTo(ComboBox forControl)
        {
            if (forControl.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                int value;
                forControl.SelectedIndex = GetConfiguration(forControl, v => int.TryParse(v, out value) && forControl.Items.Count > value ? value : forControl.SelectedIndex, forControl.SelectedIndex);
            }
            else
                forControl.Text = GetConfiguration(forControl, v => v, forControl.Text);
        }
        public virtual void SetTo(ListBox forControl)
        {
            var val = GetConfiguration(forControl+"_Items", v => v==null?null: v.Split(InlineSeparator), null);
            if (val != null)
            {
                forControl.Items.Clear();
                foreach (var item in val)
                    forControl.Items.Add(item);
            }
            int value;
            forControl.SelectedIndex = GetConfiguration(forControl, v => int.TryParse(v, out value) && forControl.Items.Count > value ? value : forControl.SelectedIndex, forControl.SelectedIndex);
        }
        public virtual void SetTo(DataGridView forControl)
        {
            foreach (var item in GetConfiguration(forControl, val =>
                {
                    return (from v in val.Split(InlineSeparator)
                     let inds = v.Split(InItemSeparator)
                     where inds.Length > 1
                     let ci = MiMFa.Service.ConvertService.TryToInt(inds[0], -1)
                     let ri = MiMFa.Service.ConvertService.TryToInt(inds[1], -1)
                     where ci > -1 && ri > -1
                     select new int[] { ci, ri }
                  ).ToList();
                }, new List<int[]>()))
                if (item[0] < forControl.Columns.Count && item[1] < forControl.Rows.Count)
                    forControl.Rows[item[1]].Cells[item[0]].Selected = true;
        }
      
        public virtual void SetTo(ContextMenuStrip forControl)
        {
            SetTo(forControl.Items);
        }
        public virtual void SetTo(ToolStrip forControl)
        {
            SetTo(forControl.Items);
        }
        public virtual void SetTo(ToolStripItemCollection forControl)
        {
            foreach (ToolStripItem item in forControl)
                SetTo(item);
        }
        public virtual void SetTo(ToolStripItem forControl)
        {
            if (forControl is ToolStripDropDownItem) SetTo((ToolStripDropDownItem)forControl);
            if (forControl is ToolStripMenuItem) SetTo((ToolStripMenuItem)forControl);
            else if (forControl is ToolStripButton) SetTo((ToolStripButton)forControl);
            else if (forControl is ToolStripTextBox) SetTo((ToolStripTextBox)forControl);
            else if (forControl is ToolStripComboBox) SetTo((ToolStripComboBox)forControl);
        }
        public virtual void SetTo(ToolStripDropDownItem forControl)
        {
            SetTo(forControl.DropDownItems);
        }
        public virtual void SetTo(ToolStripMenuItem forControl)
        {
            bool b;
            forControl.Checked = GetConfiguration(forControl, v => bool.TryParse(v, out b) ? b : forControl.Checked, forControl.Checked);
            SetTo(forControl.DropDownItems);
        }
        public virtual void SetTo(ToolStripButton forControl)
        {
            bool b;
            forControl.Checked = GetConfiguration(forControl, v => bool.TryParse(v, out b) ? b : forControl.Checked, forControl.Checked);
        }
        public virtual void SetTo(ToolStripTextBox forControl)
        {
            forControl.Text = GetConfiguration(forControl, v => v, forControl.Text);
        }
        public virtual void SetTo(ToolStripComboBox forControl)
        {
            if (forControl.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                int value;
                forControl.SelectedIndex = GetConfiguration(forControl, v => int.TryParse(v, out value) && forControl.Items.Count > value ? value : forControl.SelectedIndex, forControl.SelectedIndex);
            }
            else
                forControl.Text = GetConfiguration(forControl, v => v, forControl.Text);
        }

        public virtual void SetFrom(Control control)
        {
            if (control.Controls.Count == 0)
            {
                if (control is ComboBox) SetFrom((ComboBox)control);
                else if (control is RadioButton) SetFrom((RadioButton)control);
                else if (control is CheckBox) SetFrom((CheckBox)control);
                else if (control is ListBox) SetFrom((ListBox)control);
                else if (control is TextBoxBase) SetFrom((TextBoxBase)control);
                else if (control is ToolStrip) SetFrom((ToolStrip)control);
                else if (!(control is Label)&& !(control is Button)) SetConfiguration(control, control.Text);
            }
            else foreach (Control item in control.Controls)
                    SetFrom(item);
        }
        public virtual void SetFrom(RadioButton fromControl)
        {
            SetConfiguration(fromControl, fromControl.Checked);
        }
        public virtual void SetFrom(CheckBox fromControl)
        {
            SetConfiguration(fromControl, fromControl.Checked);
        }
        public virtual void SetFrom(TextBoxBase fromControl)
        {
            SetConfiguration(fromControl, fromControl.Text);
        }
        public virtual void SetFrom(ComboBox fromControl)
        {
            SetConfiguration(fromControl, fromControl.DropDownStyle == ComboBoxStyle.DropDownList ? fromControl.SelectedIndex + "" : fromControl.Text);
        }
        public virtual void SetFrom(ListBox fromControl)
        {
            List<object> ls = new List<object>();
            foreach (var v in fromControl.Items) ls.Add( v);
            SetConfiguration(fromControl, string.Join(InlineSeparator + "", ls));
            SetConfiguration(fromControl, fromControl.SelectedIndex);
        }
        public virtual void SetFrom(DataGridView fromControl)
        {
            List<object> ls = new List<object>();
            foreach (DataGridViewCell item in fromControl.SelectedCells)
                ls.Add(item.ColumnIndex + InItemSeparator + item.RowIndex);
            SetConfiguration(fromControl, string.Join(InlineSeparator + "", ls));
        }

        public virtual void SetFrom(ToolStrip fromControl)
        {
            SetFrom(fromControl.Items);
        }
        public virtual void SetFrom(ToolStripItemCollection fromControl)
        {
            foreach (ToolStripItem item in fromControl)
                SetFrom(item);
        }
        public virtual void SetFrom(ToolStripItem fromControl)
        {
            if (fromControl is ToolStripDropDownItem) SetFrom((ToolStripDropDownItem)fromControl);
            if (fromControl is ToolStripMenuItem) SetFrom((ToolStripMenuItem)fromControl);
            else if (fromControl is ToolStripButton) SetFrom((ToolStripButton)fromControl);
            else if (fromControl is ToolStripTextBox) SetFrom((ToolStripTextBox)fromControl);
            else if (fromControl is ToolStripComboBox) SetFrom((ToolStripComboBox)fromControl);
        }
        public virtual void SetFrom(ToolStripDropDownItem fromControl)
        {
            SetFrom(fromControl.DropDownItems);
        }
        public virtual void SetFrom(ToolStripMenuItem fromControl)
        {
            SetConfiguration(fromControl, fromControl.Checked);
            SetFrom(fromControl.DropDownItems);
        }
        public virtual void SetFrom(ToolStripButton fromControl)
        {
            if (fromControl.CheckOnClick)
                SetConfiguration(fromControl, fromControl.Checked);
        }
        public virtual void SetFrom(ToolStripTextBox fromControl)
        {
            SetConfiguration(fromControl, fromControl.Text);
        }
        public virtual void SetFrom(ToolStripComboBox fromControl)
        {
            SetConfiguration(fromControl, fromControl.DropDownStyle == ComboBoxStyle.DropDownList ? fromControl.SelectedIndex + "" : fromControl.Text);
        }


        public virtual void DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText) &&
                (InfoService.IsAddress(e.Data.GetData(DataFormats.UnicodeText) + "")
                || InfoService.IsAbsoluteURL(e.Data.GetData(DataFormats.UnicodeText) + "")))
                e.Effect = DragDropEffects.Link;
            else if (e.Data.GetDataPresent(DataFormats.StringFormat))
                e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }
        public virtual void DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                Import((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText))
                Import((e.Data.GetData(DataFormats.UnicodeText) + ""));
            else if (e.Data.GetDataPresent(DataFormats.StringFormat))
                Import((e.Data.GetData(DataFormats.StringFormat) + ""));
        }
    }
}
