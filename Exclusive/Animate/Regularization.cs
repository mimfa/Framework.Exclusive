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
using MiMFa.Model;


namespace MiMFa.Exclusive.Animate
{
  public  class Regularization
    {
        public static void ChildControlsHorizentalLayout<T>(T control) where T : Control
        {
            int cend = control.Size.Width;
            for (int i = 0; i < control.Controls.Count; i++)
            {
                if (i == 0) control.Controls[i].Location = new Point(control.Controls[i].Margin.Left, control.Controls[i].Margin.Top);
                else
                {
                    int xend = control.Controls[i - 1].Location.X + control.Controls[i].Margin.Left + control.Controls[i - 1].Size.Width + control.Controls[i - 1].Margin.Right;
                    control.Controls[i].Location = new Point(xend, control.Controls[i - 1].Location.Y);
                    int eend = control.Controls[i].Location.X + control.Controls[i].Size.Width + control.Controls[i].Margin.Right;

                    if (eend <= cend)
                        control.Controls[i].Location = new Point(xend, control.Controls[i - 1].Location.Y - control.Controls[i - 1].Margin.Top + control.Controls[i].Margin.Top);
                    else control.Controls[i].Location = new Point(control.Controls[i].Margin.Left, control.Controls[i - 1].Location.Y + control.Controls[i - 1].Size.Height + control.Controls[i].Margin.Top + control.Controls[i - 1].Margin.Bottom);
                }
            }
        }
        public static void ChildControlsVerticalLayout<T>(T control) where T : Control
        {
            int cend = control.Size.Height;
            for (int i = 0; i < control.Controls.Count; i++)
            {
                if (i == 0) control.Controls[i].Location = new Point(control.Controls[i].Margin.Left, control.Controls[i].Margin.Top);
                else
                {
                    int yend = control.Controls[i - 1].Location.Y + control.Controls[i].Margin.Top + control.Controls[i - 1].Size.Height + control.Controls[i - 1].Margin.Bottom;
                    control.Controls[i].Location = new Point( control.Controls[i - 1].Location.X,yend);
                    int eend = control.Controls[i].Location.Y + control.Controls[i].Size.Height + control.Controls[i].Margin.Bottom;

                    if (eend <= cend)
                        control.Controls[i].Location = new Point(control.Controls[i - 1].Location.X - control.Controls[i - 1].Margin.Left + control.Controls[i].Margin.Left,yend);
                    else control.Controls[i].Location = new Point( control.Controls[i - 1].Location.X + control.Controls[i - 1].Size.Height + control.Controls[i].Margin.Left + control.Controls[i - 1].Margin.Right,control.Controls[i].Margin.Top);
                }
            }
        }
        public static void ChildControlsBothLayout<T>(T control) where T : Control
        {
            {
                ChildControlsHorizentalLayout(control);
                int cend = control.Size.Height;
                int iend = control.Controls.Count - 1;
                int eend = control.Controls[iend].Location.Y + control.Controls[iend].Size.Height + control.Controls[iend].Margin.Bottom;
                if (eend > cend)
                {
                    int highmines = ((eend - cend) / control.Controls.Count)+1;
                    for (int i = 0; i < control.Controls.Count; i++)
                        control.Controls[i].Height -= highmines;
                }
                if (eend < cend)
                {
                    int highplus = ((cend - eend) / control.Controls.Count)-1;
                    for (int i = 0; i < control.Controls.Count; i++)
                        control.Controls[i].Height += highplus;
                }
            }

        }

        public static void ChildControlsLayout<T>(T control, LayoutMode layout) where T : Control
        {
            switch(layout)
            {
                case LayoutMode.Horizental:
                    ChildControlsHorizentalLayout(control);
                    break;
                case LayoutMode.Vertical:
                    ChildControlsVerticalLayout(control);
                    break;
                case LayoutMode.Both:
                    ChildControlsBothLayout(control);
                    break;
                default:
                    break;
            }
        }
    }
}
