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
                this.buttonConnection.Text = "Disconnect";
            }
            else
            {
                _socketClient.Disconnect();
                this.buttonConnection.Text = "Connect";
            }            
        }
        private void SendMessage(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.textBoxMessage.Text))
            {
                _socketClient.SendMessage(this.textBoxMessage.Text);
                ReceibeMessage();
                this.textBoxMessage.Text = String.Empty;
            }            
        }
        private async void ReceibeMessage()
        {
            Response response = await _socketClient.ReceiveMessage<Response>();
            this.listBoxLog.Items.Add(response.ToString());
            if (this.listBoxLog.Items.Count >= (Int32.TryParse(ConfigurationManager.AppSettings["LimitMessages"], out int outLimitMessage) ? outLimitMessage : 100))
                this.listBoxLog.Items.RemoveAt(0);
        }
    }
}
