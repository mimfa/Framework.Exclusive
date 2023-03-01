using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MiMFa.Service;
using MiMFa.General;

namespace MiMFa.Exclusive.Animate
{
    public class ControlSorter
    {
        // Sorter ByClick
        public Dictionary<Point, Size> StateOrders = new Dictionary<Point, Size>();
        public Control ControlsParent = null;
        public string SorterHistoryFileAddress = "";
        public ControlSorter(string sorterHistoryFileAddress, Control parent)
        {
            try
            {
                SorterHistoryFileAddress = sorterHistoryFileAddress;
                ControlsParent = parent;
                Control.ControlCollection ccc = parent.Controls;
                for (int i = 0; i < ccc.Count; i++)
                    ccc[i].MouseClick += new MouseEventHandler(Increment);
                OnControlClick(parent);
            }
            catch { }
        }
        public void Increment(object sender, EventArgs e)
        {
            OnControlClick((Control)sender);
        }
        protected void OnControlClick(Control clickedControl = null)
        {
            try
            {
                Dictionary<string, int> diccon = GetDicClick(ControlsParent);
                if (diccon.Count > 0 && clickedControl != null)
                    try { diccon[clickedControl.Name] = diccon[clickedControl.Name] + 1; }
                    catch { }
                IOService.SaveSerializeFile(SorterHistoryFileAddress, diccon);
            }
            catch { }
        }
        public void ControlsSort(params Control[] childsByDecOrder)
        {
            try
            {
                Dictionary<string, int> diccon = GetDicClick(childsByDecOrder);
                List<int> lin = new List<int>();
                List<Control> lcon = new List<Control>();
                List<int> lout = new List<int>();
                List<int> lindex = new List<int>();
                Control con_m = null;
                StateOrders.Clear();
                foreach (var item in childsByDecOrder)
                    StateOrders.Add(item.Location, item.Size);
                foreach (var item in diccon)
                {
                    if ((con_m = ControlService.GetControlsByName(ControlsParent, item.Key)) != null)
                    {
                        lcon.Add(con_m);
                        lin.Add(item.Value);
                    }
                }
                CollectionService.Sort(lin, out lout, out lindex);
                lindex.Reverse();
                int index = 0;
                foreach (var item in StateOrders)
                {
                    try
                    {
                        lcon[lindex[index]].Size = item.Value;
                        lcon[lindex[index]].Location = item.Key;
                    }
                    catch { }
                    index++;
                }
            }
            catch { }
        }
        public void ControlsSort(Control parent)
        {
            parent.SuspendLayout();
            ControlsSort(parent.Controls.OfType<Control>().ToArray());
            parent.ResumeLayout(true);
        }

        #region Private Region

        private Dictionary<string, int> GetDicClick(Control parent)
        {
            Control.ControlCollection ccc = parent.Controls;
            Dictionary<string, int> diccon = new Dictionary<string, int>();
            try { IOService.OpenDeserializeFile(SorterHistoryFileAddress, ref diccon); }
            catch
            {
                diccon.Clear();
                for (int i = 0; i < ccc.Count; i++)
                    diccon.Add(ccc[i].Name, i);
            }
            if (diccon.Count < parent.Controls.Count)
            {
                diccon.Clear();
                for (int i = 0; i < ccc.Count; i++)
                    diccon.Add(ccc[i].Name, i);
            }
            return diccon;
        }
        private Dictionary<string, int> GetDicClick(params Control[] childsByDecOrder)
        {
            Dictionary<string, int> diccon = new Dictionary<string, int>();
            try { IOService.OpenDeserializeFile(SorterHistoryFileAddress, ref diccon); }
            catch
            {
                diccon.Clear();
                for (int i = 0; i < childsByDecOrder.Length; i++)
                    diccon.Add(childsByDecOrder[i].Name, i);
            }
            if (diccon.Count < childsByDecOrder.Length)
            {
                diccon.Clear();
                for (int i = 0; i < childsByDecOrder.Length; i++)
                    diccon.Add(childsByDecOrder[i].Name, i);
            }
            return diccon;
        }

        #endregion
    }
}
