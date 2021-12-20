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
                try
                {
                    _socketClient.Start();
                    SetTimer();
                    this.buttonConnection.Text = "Disconnect";
                    this.buttonSend.Enabled = true;
                }
                catch (Exception)
                {
                    MessageBox.Show("Error to connect to the server", "Communication error");
                    this.buttonConnection.Text = "Connect";
                    this.buttonSend.Enabled = false;
                }
            }
            else
            {
                try
                {
                    _socketClient.Disconnect();
                    this.buttonConnection.Text = "Connect";
                    this.buttonSend.Enabled = false;
                }catch (Exception)
                {
                    this.buttonConnection.Text = "Disconnect";
                    this.buttonSend.Enabled = true;
                }
                _timer?.Dispose();
            }            
        }
        private void SendMessage(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.textBoxMessage.Text))
            {
                _timer?.Stop();
                try
                {
                    _socketClient.SendMessage(this.textBoxMessage.Text);
                }catch (Exception)
                {
                    _timer?.Start();
                    MessageBox.Show("Error to send message", "Communication error");
                    return;
                }
                ReceibeMessage();
                this.textBoxMessage.Text = String.Empty;
                _timer.Interval = 5000;
                _timer?.Start();
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
            try
            {
                _timer = new System.Timers.Timer();
                _timer.Interval = 5000;
                _timer.AutoReset = true;
                _timer.Enabled = true;
                _timer.Elapsed += (sender, e) => OnTimedEvent(sender, e, _socketClient);
            }
            catch (Exception)
            {
                _timer = null;
            }
        }
        private static async void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e, SocketClient client)
        {
            client.SendMessage("Keep alive");
            Response response = await client.ReceiveMessage<Response>();
        }
    }
}
