using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        SocketServer _socketServer;
        public Form1()
        {
            InitializeComponent();
            _socketServer = new SocketServer();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if(this.buttonStart.Text == "Start")
            {
                try
                {
                    Task.Run(() => _socketServer.Start());
                    this.buttonStart.Text = "Stop";
                }catch (Exception)
                {
                    this.buttonStart.Text = "Start";
                    MessageBox.Show("Error to start service", "Service error");
                }
            }
            else
            {
                _socketServer.Stop();
                this.buttonStart.Text = "Start";
            }            
        }
    }
}
