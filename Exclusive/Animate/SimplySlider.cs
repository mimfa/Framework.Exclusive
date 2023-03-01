using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MiMFa.Exclusive.Animate
{
    public class SimplySlider
    {
        public SimplySlider(int speed = 10)
        {
            try
            {
                Active = false;
                TimerOn.Tick += (object sender, EventArgs e) => { TimerOnAct(speed);};
                TimerOn.Enabled = false;
                TimerOn.Interval = 1;
                TimerOff.Tick += (object sender, EventArgs e) => { TimerOffAct(speed); };
                TimerOff.Enabled = false;
                TimerOff.Interval = 1;
            }
            catch { }
        }

        public void AddControl<T, F>(T container, F actor) where T : Control where F : Control => AddControl(container, actor, container.Dock);
        public void AddControl<T, F>(T container, F actor, DockStyle sideEffect) where T : Control where F : Control
        {
            actor.Click += (o, e) =>
           {
               if (container.Visible)
                   Off(container, sideEffect);
               else
                   On(container, sideEffect);
           };
        }
        public void On<T>(T container) where T : Control => On(container, container.Dock);
        public void On<T>(T container, DockStyle sideEffect) where T : Control
        {
            try
            {
                OnContainer = container;
                OnContainerDock = container.Dock;
                if (!container.MaximumSize.IsEmpty)
                    OnContainerSize = OnContainer.MaximumSize;
                else OnContainerSize = OnContainer.Size;
                DS =  sideEffect;
                container.Dock = sideEffect;
                switch (DS)
                {
                    case DockStyle.Left:
                        OnContainer.Width = 0;
                        break;
                    case DockStyle.Right:
                        OnContainer.Width = 0;
                        break;
                    case DockStyle.Top:
                        OnContainer.Height = 0;
                        break;
                    case DockStyle.Bottom:
                        OnContainer.Height = 0;
                        break;
                }
                TimerOn.Enabled = true;
            }
            catch { }
        }//Called In Panel Action
        public void Off<T>(T container) where T : Control => Off(container, container.Dock);
        public void Off<T>(T container, DockStyle sideEffect) where T : Control
        {
            try
            {
                OffContainerDock = container.Dock;
                OffContainer = container;
                if (!container.MaximumSize.IsEmpty) OffContainerSize = OffContainer.MaximumSize;
                else OffContainerSize = OffContainer.Size;
                DS = sideEffect;
                container.Dock = sideEffect;
                TimerOff.Enabled = true;
            }
            catch { }
        }//Called In Panel Action

        #region Private Region

        //  Slider
        private Timer TimerOn = new Timer();
        private Timer TimerOff = new Timer();
        private bool Active = false;
        private int FinalStretch = 0;
        private DockStyle DS = DockStyle.None;
        private Control OnContainer = null;
        private Control OffContainer = null;
        private DockStyle OnContainerDock;
        private DockStyle OffContainerDock;
        private Size OnContainerSize;
        private Size OffContainerSize;

        private void TimerOnAct(int speed = 1)
        {
            try
            {
                switch (DS)
                {
                    case DockStyle.Left:
                        HorizentalOn(speed);
                        break;
                    case DockStyle.Right:
                        HorizentalOn(speed);
                        break;
                    case DockStyle.Top:
                        VerticalOn(speed);
                        break;
                    case DockStyle.Bottom:
                        VerticalOn(speed);
                        break;
                }
            }
            catch { }
        }//Called In Timer
        private void TimerOffAct(int speed = 1)
        {
            try
            {
                switch (DS)
                {
                    case DockStyle.Left:
                        HorizentalOff(speed);
                        break;
                    case DockStyle.Right:
                        HorizentalOff(speed);
                        break;
                    case DockStyle.Top:
                        VerticalOff(speed);
                        break;
                    case DockStyle.Bottom:
                        VerticalOff(speed);
                        break;
                }
            }
            catch { }
        }//Called In Timer

        private void HorizentalOn(int speed = 1)
        {
            try
            {
                if (OnContainer != null)
                {
                    if (!Active && !OnContainer.Visible)
                    {
                        FinalStretch = OnContainerSize.Width;
                        OnContainer.Width = 0;
                        Active = OnContainer.Visible = true;
                    }

                    if (Active && OnContainer.Width <= FinalStretch)
                        OnContainer.Width += speed;

                    if (Active && OnContainer.Width >= FinalStretch)
                    {
                        OnContainer.Width = OnContainerSize.Width;
                        Active = TimerOn.Enabled = false;
                        OnContainer = null;
                    }
                }
            }
            catch { }
        }
        private void HorizentalOff(int speed = 1)
        {
            try
            {
                if (OffContainer != null)
                {
                    if (!Active && OffContainer.Visible)
                    {
                        FinalStretch = OffContainerSize.Width;
                        Active = true;
                    }

                    if (Active && OffContainer.Width - speed >= 0)
                        OffContainer.Width -= speed;
                    else if (OffContainer.Width - speed < 0)
                        OffContainer.Width = 0;


                    if (Active && OffContainer.Width <= OffContainer.MinimumSize.Width)
                    {
                        OffContainer.Visible = false;
                        Active = TimerOff.Enabled = false;
                        OffContainer.Width = OffContainerSize.Width;
                        OffContainer = null;
                    }
                }
            }
            catch { }
        }
        private void VerticalOn(int speed = 1)
        {
            try
            {
                if (OnContainer != null)
                {
                    if (!Active && !OnContainer.Visible)
                    {
                        FinalStretch = OnContainerSize.Height;
                        OnContainer.Height = 0;
                        Active = OnContainer.Visible = true;
                    }

                    if (Active && OnContainer.Height <= FinalStretch)
                        OnContainer.Height += speed;

                    if (Active && OnContainer.Height >= FinalStretch)
                    {
                        OnContainer.Height = OnContainerSize.Height;
                        Active = TimerOn.Enabled = false;
                        OnContainer = null;
                        OnContainer.Dock = OnContainerDock;
                    }
                }
            }
            catch { }
        }
        private void VerticalOff(int speed = 1)
        {
            try
            {
                if (OffContainer != null)
                {
                    if (!Active && OffContainer.Visible)
                    {
                        FinalStretch = OffContainerSize.Height;
                        Active = true;
                    }

                    if (Active && OffContainer.Height - speed >= 0)
                        OffContainer.Height -= speed;
                    else if (OffContainer.Height - speed < 0)
                        OffContainer.Height = 0;


                    if (Active && OffContainer.Height <= OffContainer.MinimumSize.Height)
                    {
                        OffContainer.Visible = false;
                        Active = TimerOff.Enabled = false;
                        OffContainer.Height = OffContainerSize.Height;
                        OffContainer = null;
                        OffContainer.Dock = OffContainerDock;
                    }
                }
            }
            catch { }
        }

        #endregion
    }
}
