namespace Htggbb.FitnessCourse
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSynthesis = new System.Windows.Forms.Button();
            this.openDialog = new System.Windows.Forms.OpenFileDialog();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnInit = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSynthesis
            // 
            this.btnSynthesis.Location = new System.Drawing.Point(84, 3);
            this.btnSynthesis.Name = "btnSynthesis";
            this.btnSynthesis.Size = new System.Drawing.Size(122, 23);
            this.btnSynthesis.TabIndex = 0;
            this.btnSynthesis.Text = "Text to speech";
            this.btnSynthesis.UseVisualStyleBackColor = true;
            this.btnSynthesis.Click += new System.EventHandler(this.btnSynthesis_Click);
            // 
            // openDialog
            // 
            this.openDialog.Filter = "课程文件 | *.txt";
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(0, 33);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(600, 256);
            this.txtLog.TabIndex = 1;
            this.txtLog.Text = "";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnInit);
            this.flowLayoutPanel1.Controls.Add(this.btnSynthesis);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(600, 33);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(3, 3);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(75, 23);
            this.btnInit.TabIndex = 1;
            this.btnInit.Text = "Init";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 289);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "FormMain";
            this.Text = "Speech Creator";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSynthesis;
        private System.Windows.Forms.OpenFileDialog openDialog;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnInit;
    }
}

