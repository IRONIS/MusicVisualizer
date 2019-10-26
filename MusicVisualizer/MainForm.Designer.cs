namespace MusicVisualizer
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblArduinoStatus = new System.Windows.Forms.Label();
            this.btnStopGrab = new System.Windows.Forms.Button();
            this.btnStartGrab = new System.Windows.Forms.Button();
            this.lblAudioStatus = new System.Windows.Forms.Label();
            this.waveChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cbSerialPort = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.waveChart)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(269, 4);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnect.TabIndex = 0;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(188, 4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblArduinoStatus
            // 
            this.lblArduinoStatus.AutoSize = true;
            this.lblArduinoStatus.Location = new System.Drawing.Point(12, 9);
            this.lblArduinoStatus.Name = "lblArduinoStatus";
            this.lblArduinoStatus.Size = new System.Drawing.Size(43, 13);
            this.lblArduinoStatus.TabIndex = 2;
            this.lblArduinoStatus.Text = "Arduino";
            // 
            // btnStopGrab
            // 
            this.btnStopGrab.Enabled = false;
            this.btnStopGrab.Location = new System.Drawing.Point(269, 31);
            this.btnStopGrab.Name = "btnStopGrab";
            this.btnStopGrab.Size = new System.Drawing.Size(75, 23);
            this.btnStopGrab.TabIndex = 4;
            this.btnStopGrab.Text = "Stop";
            this.btnStopGrab.UseVisualStyleBackColor = true;
            this.btnStopGrab.Click += new System.EventHandler(this.btnStopGrab_Click);
            // 
            // btnStartGrab
            // 
            this.btnStartGrab.Location = new System.Drawing.Point(188, 31);
            this.btnStartGrab.Name = "btnStartGrab";
            this.btnStartGrab.Size = new System.Drawing.Size(75, 23);
            this.btnStartGrab.TabIndex = 5;
            this.btnStartGrab.Text = "Start";
            this.btnStartGrab.UseVisualStyleBackColor = true;
            this.btnStartGrab.Click += new System.EventHandler(this.btnStartGrab_Click);
            // 
            // lblAudioStatus
            // 
            this.lblAudioStatus.AutoSize = true;
            this.lblAudioStatus.Location = new System.Drawing.Point(12, 37);
            this.lblAudioStatus.Name = "lblAudioStatus";
            this.lblAudioStatus.Size = new System.Drawing.Size(59, 13);
            this.lblAudioStatus.TabIndex = 6;
            this.lblAudioStatus.Text = "Grab audio";
            // 
            // waveChart
            // 
            chartArea1.Name = "LeftChannelArea";
            chartArea2.Name = "RightChannelArea";
            this.waveChart.ChartAreas.Add(chartArea1);
            this.waveChart.ChartAreas.Add(chartArea2);
            this.waveChart.Location = new System.Drawing.Point(15, 60);
            this.waveChart.Name = "waveChart";
            this.waveChart.Size = new System.Drawing.Size(329, 396);
            this.waveChart.TabIndex = 8;
            this.waveChart.Text = "WaveChart";
            // 
            // cbSerialPort
            // 
            this.cbSerialPort.FormattingEnabled = true;
            this.cbSerialPort.Location = new System.Drawing.Point(61, 6);
            this.cbSerialPort.Name = "cbSerialPort";
            this.cbSerialPort.Size = new System.Drawing.Size(121, 21);
            this.cbSerialPort.TabIndex = 9;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 468);
            this.Controls.Add(this.cbSerialPort);
            this.Controls.Add(this.waveChart);
            this.Controls.Add(this.lblAudioStatus);
            this.Controls.Add(this.btnStartGrab);
            this.Controls.Add(this.btnStopGrab);
            this.Controls.Add(this.lblArduinoStatus);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnDisconnect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "MusicVisualizer";
            ((System.ComponentModel.ISupportInitialize)(this.waveChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblArduinoStatus;
        private System.Windows.Forms.Button btnStopGrab;
        private System.Windows.Forms.Button btnStartGrab;
        private System.Windows.Forms.Label lblAudioStatus;
        private System.Windows.Forms.DataVisualization.Charting.Chart waveChart;
        private System.Windows.Forms.ComboBox cbSerialPort;
    }
}

