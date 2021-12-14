using StandardLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private System.Timers.Timer _timer;

        SocketClient _socketClient;
        public Form1()
        {
            InitializeComponent();
            _socketClient = new SocketClient();
        }

        private void buttonConnection_Click(object sender, EventArgs e)
        {
            if(this.buttonConnection.Text == "Connect")
            {
                _socketClient.Start();
                SetTimer();
                this.buttonConnection.Text = "Disconnect";
            }
            else
            {
                _socketClient.Disconnect();
                this.buttonConnection.Text = "Connect";
                _timer?.Dispose();
            }            
        }
        private void SendMessage(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.textBoxMessage.Text))
            {
                _socketClient.SendMessage(this.textBoxMessage.Text);
                ReceibeMessage();
                this.textBoxMessage.Text = String.Empty;
                _timer.Interval = 5000;
            }            
        }
        private async void ReceibeMessage()
        {
            Response response = await _socketClient.ReceiveMessage<Response>();
            this.listBoxLog.Items.Add(response.ToString());
            if (this.listBoxLog.Items.Count >= (Int32.TryParse(ConfigurationManager.AppSettings["LimitMessages"], out int outLimitMessage) ? outLimitMessage : 100))
                this.listBoxLog.Items.RemoveAt(0);
        }

        private void SetTimer()
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = 5000;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Elapsed += (sender, e) => OnTimedEvent(sender, e, _socketClient);
        }
        private static async void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e, SocketClient client)
        {
            //form.textBoxMessage.Text = "Keep alive";
            //form.SendMessage(source, e);
            client.SendMessage("Keep alive");
            Response response = await client.ReceiveMessage<Response>();
        }
    }
}
