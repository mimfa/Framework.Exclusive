using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using MiMFa.General;
using MiMFa.Service;

namespace MiMFa.Exclusive.PluginAndComponent
{
    public class Plugins
    {
        //Static
        public static void ControlCreator(ref Control thisControl, string Name, string Text = "", bool Enabled = true, bool Visible = true, Image BackgroundImage = null, RightToLeft RightToLeft = RightToLeft.No, DockStyle Dock = DockStyle.None, int PaddingAll = 0, AnchorStyles Anchor = AnchorStyles.None, ContextMenu ContextMenu = null)
        {
            try
            {
                thisControl.Name = Name;
                thisControl.Text = Text;
                thisControl.Enabled = Enabled;
                thisControl.Visible = Visible;
                thisControl.BackgroundImage = BackgroundImage;
                thisControl.RightToLeft = RightToLeft;
                thisControl.Dock = Dock;
                thisControl.Padding = new Padding(PaddingAll);
                thisControl.Anchor = Anchor;
                thisControl.ContextMenu = ContextMenu;
            }
            catch { }
        }
        public static void ToolStripMenuItemCreator(ref ToolStripMenuItem thisMenustrip, string Name, string Text = "", RightToLeft RightToLeft = RightToLeft.No, bool CheckOnClick = false, bool Checked = false, byte[] Image = null, bool Enabled = true, bool Visible = true, bool RightToLeftAutoMirrorImage = false, int PaddingAll = 0, int MarginAll = 0, DockStyle Dock = DockStyle.None, byte[] BackgroundImage = null)
        {
            try
            {
                thisMenustrip.Name = Name;
                thisMenustrip.Text = Text;
                thisMenustrip.Enabled = Enabled;
                thisMenustrip.Visible = Visible;
                thisMenustrip.Image = ConvertService.ToImage(Image);
                thisMenustrip.CheckOnClick = CheckOnClick;
                thisMenustrip.Checked = Checked;
                thisMenustrip.RightToLeft = RightToLeft;
                thisMenustrip.RightToLeftAutoMirrorImage = RightToLeftAutoMirrorImage;
                thisMenustrip.Padding = new Padding(PaddingAll);
                thisMenustrip.Margin = new Padding(MarginAll);
                thisMenustrip.Dock = Dock;
                thisMenustrip.BackgroundImage = ConvertService.ToImage(BackgroundImage);
            }
            catch { }
        }

        //PACFileCreator
        public static void ToolStripMenuItemPACFileCreator(ToolStripMenuItem thisMenustrip, string PACAddress)
        {
            try
            {
                Dictionary<string, object> dso = new Dictionary<string, object>();
                dso.Add("Name", thisMenustrip.Name);
                dso.Add("Text", thisMenustrip.Text);
                dso.Add("CheckOnClick", thisMenustrip.CheckOnClick);
                dso.Add("Checked", thisMenustrip.Checked);
                dso.Add("Image", thisMenustrip.Image);
                dso.Add("Enabled", thisMenustrip.Enabled);
                dso.Add("Visible", thisMenustrip.Visible);
                dso.Add("RightToLeft", thisMenustrip.RightToLeft);
                dso.Add("RightToLeftAutoMirrorImage", thisMenustrip.RightToLeftAutoMirrorImage);
                dso.Add("Padding", thisMenustrip.Padding);
                dso.Add("Margin", thisMenustrip.Margin);
                dso.Add("Dock", thisMenustrip.Dock);
                dso.Add("BackgroundImage", thisMenustrip.BackgroundImage);

                IOService.SaveSerializeFile(PACAddress, dso);
            }
            catch { }
        }

        //Template
        public List<string> AddExternalClass(EventHandler target, params string[] PAC_Addresses)
        {
            List<string> ls = new List<string>();
            try
            {
            }
            catch { }
            return ls;
        }
        public void AddLanguagesInToolStripMenuItem(ref ToolStripMenuItem thisMenustrip, string[] PAC_Addresses, EventHandler target)
        {
            try
            {
                foreach (var item in PAC_Addresses)
                    CreateToolStripChild(ref thisMenustrip, item, target);
            }
            catch { }
        }

        //Create Object
        public void CreateToolStripChild(ref ToolStripMenuItem thisMenustrip, string PAC_Address, EventHandler target)
        {
            try
            {
                ToolStripMenuItem t = new ToolStripMenuItem();
                Dictionary<string, object> dso = new Dictionary<string, object>();
                IOService.OpenDeserializeFile(PAC_Address, ref dso);
                try
                {

                    ToolStripMenuItemCreator(ref t,
                           dso["Name"].ToString(),
                           dso["Text"].ToString(),
                           (RightToLeft)dso["RightToLeft"],
                           (bool)dso["CheckOnClick"],
                           (bool)dso["Checked"]);
                }
                catch { }
                t.Click += target;
                thisMenustrip.DropDownItems.AddRange(new ToolStripItem[] { t });

            }
            catch { }
        }
    }
}
