using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using MiMFa.General;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.Message
{
    public class MessageViewer
    {
        public Control Container = null;
        public Control Label = null;
        public Color ForeColor = Color.Transparent;
        public Color BackColor = Color.Transparent;
        public Image Image = null;

        public string Message = "";
        public List<string> Logs = new List<string>();
        public MessageMode MessageType = MessageMode.Message;
        public int LenghtTime = 10;
        public Timer Timer = new Timer();

        public event EventHandler ShowMessage = (o, a) => { };
        public event EventHandler HideMessage = (o, a) => { };

        public MessageViewer()
        {
            Timer.Tick += Timer_Tick;
            Timer.Interval = 1000;
            Timer.Enabled = true;
        }

        public string Get(MessageMode messageType,int lenghtTime,params string[] keys)
        {
            string message = Default.Translate(keys);
            switch (messageType)
            {
                case MessageMode.Success:
                    return Success(message, lenghtTime);
                case MessageMode.Warning:
                    return Warning(message, lenghtTime);
                case MessageMode.Error:
                    return Error(message, lenghtTime);
                case MessageMode.Message:
                    return Text(message, lenghtTime);
                default:
                    return message;
            }
        }
        public string Get(int lenghtTime,params string[] keys)
        {
            return Get(MessageType, lenghtTime, keys);
        }
        public string Get(params string[] keys)
        {
            return Get(MessageType, -1, keys);
        }
        public string Get(MessageMode messageType,params string[] keys)
        {
            return Get(messageType,-1, keys);
        }
        public string Success(string message = "", int lenghtTime = -1)
        {
            ForeColor = SuccessForeColor;
            BackColor = SuccessBackColor;
            Image = SuccessImage;
            MessageType = MessageMode.Success;
            Start(message,lenghtTime);
            return Message;
        }
        public string Warning(string message = "", int lenghtTime = -1)
        {
            ForeColor = WarningForeColor;
            BackColor = WarningBackColor;
            Image = WarningImage;
            MessageType = MessageMode.Warning;
            Start(message,lenghtTime);
            return Message;
        }
        public string Error(string message = "", int lenghtTime = -1)
        {
            ForeColor = ErrorForeColor;
            BackColor = ErrorBackColor;
            Image = ErrorImage;
            MessageType = MessageMode.Error;
            Start(message,lenghtTime);
            return Message;
        }
        public string Text(string message = "", int lenghtTime = -1)
        {
            ForeColor = TextForeColor;
            BackColor = TextBackColor;
            Image = TextImage;
            MessageType = MessageMode.Message;
            Start(message,lenghtTime);
            return Message;
        }
        public string Success(params string[] keys)
        {
            string message = Default.Translate(keys);
            ForeColor = SuccessForeColor;
            BackColor = SuccessBackColor;
            Image = SuccessImage;
            MessageType = MessageMode.Success;
            Start(message,-1);
            return Message;
        }
        public string Warning(params string[] keys)
        {
            string message = Default.Translate(keys);
            ForeColor = WarningForeColor;
            BackColor = WarningBackColor;
            Image = WarningImage;
            MessageType = MessageMode.Warning;
            Start(message, -1);
            return Message;
        }
        public DialogResult DialogWarning(params string[] message)
        {
            return Service.DialogService.ShowMessage(MessageMode.Warning,true, message);
        }
        public DialogResult DialogError(params string[] message)
        {
            return Service.DialogService.ShowMessage(MessageMode.Error, true, message);
        }
        public DialogResult DialogSuccess(params string[] message)
        {
            return Service.DialogService.ShowMessage(MessageMode.Success, true, message);
        }
        public DialogResult DialogText(params string[] message)
        {
            return Service.DialogService.ShowMessage(MessageMode.Message, true, message);
        }
        public DialogResult DialogQuestion(params string[] message)
        {
            return Service.DialogService.ShowMessage(MessageMode.Question, true, message);
        }
        public string Error(params string[] keys)
        {
            string message = Default.Translate(keys);
            ForeColor = ErrorForeColor;
            BackColor = ErrorBackColor;
            Image = ErrorImage;
            MessageType = MessageMode.Error;
            Start(message, -1);
            return Message;
        }
        public string Text(params string[] keys)
        {
            string message = Default.Translate(keys);
            ForeColor = TextForeColor;
            BackColor = TextBackColor;
            Image = TextImage;
            MessageType = MessageMode.Message;
            Start(message, -1);
            return Message;
        }

        public Image SuccessImage = null;
        public Color SuccessForeColor = Color.White;
        public Color SuccessBackColor = Color.DarkGreen;
        public Image ErrorImage = null;
        public Color ErrorForeColor = Color.White;
        public Color ErrorBackColor = Color.Red;
        public Image WarningImage = null;
        public Color WarningForeColor = Color.White;
        public Color WarningBackColor = Color.OrangeRed;
        public Image TextImage = null;
        public Color TextForeColor = Color.White;
        public Color TextBackColor = Color.DarkMagenta;

        int Time = 0;
        private void Start(string message,int lenghtTime)
        {
            ShowMessage(this, EventArgs.Empty);
            Time = 0;
            Logs.Add(Message = message);
            if(lenghtTime >= 0) LenghtTime = lenghtTime;
            if (Container != null)
                MiMFa.Service.ControlService.SetControlThreadSafe(Container, new Action<object[]>((oa) => {
                    Container.BackColor = BackColor;
                    Container.ForeColor = ForeColor;
                    Container.BackgroundImage = Image;
                }), new object[] { });
            if (Label != null)
                MiMFa.Service.ControlService.SetControlThreadSafe(Label, new Action<object[]>((oa) => {
                    Label.Visible = true;
                    Label.Text = Message;
                }), new object[] { });
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Time++ < LenghtTime) return;
            HideMessage(this, EventArgs.Empty);
            if (Container != null)
                MiMFa.Service.ControlService.SetControlThreadSafe(Container, new Action<object[]>((oa) =>
                {
                    Container.BackColor = Color.Transparent;
                    Container.ForeColor = Color.Transparent;
                    Container.BackgroundImage = null;
                }), new object[] { });
            if (Label != null)
                MiMFa.Service.ControlService.SetControlThreadSafe(Label, new Action<object[]>((oa) =>
                {
                    Label.Visible = false;
                    Label.Text = "";
                }), new object[] { });
        }
    }
}
