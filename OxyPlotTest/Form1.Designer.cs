namespace OxyPlotTest
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.plotView1 = new OxyPlot.WindowsForms.PlotView();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTheta = new System.Windows.Forms.TextBox();
            this.txtDeltaX = new System.Windows.Forms.TextBox();
            this.txtDeltaY = new System.Windows.Forms.TextBox();
            this.txtNumOfOutlier = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // plotView1
            // 
            this.plotView1.Location = new System.Drawing.Point(12, 12);
            this.plotView1.Name = "plotView1";
            this.plotView1.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.plotView1.Size = new System.Drawing.Size(1175, 827);
            this.plotView1.TabIndex = 0;
            this.plotView1.Text = "plotView1";
            this.plotView1.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plotView1.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plotView1.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微軟正黑體", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button1.Location = new System.Drawing.Point(1447, 552);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(151, 125);
            this.button1.TabIndex = 1;
            this.button1.Text = "Register";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(1467, 715);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 54);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("微軟正黑體", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button2.Location = new System.Drawing.Point(1447, 398);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(151, 125);
            this.button2.TabIndex = 3;
            this.button2.Text = "Load File";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "ICP",
            "CPD(Rig)",
            "CPD(Affin)",
            "CPD(Deformable)",
            "CPD(Rig)+ICP",
            "CPD(Affin)+ICP"});
            this.comboBox1.Location = new System.Drawing.Point(1735, 1);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 23);
            this.comboBox1.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(1503, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 43);
            this.label2.TabIndex = 5;
            this.label2.Text = "theta:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(1470, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 43);
            this.label3.TabIndex = 6;
            this.label3.Text = "delta X:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微軟正黑體", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(1470, 214);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(171, 54);
            this.label4.TabIndex = 7;
            this.label4.Text = "delta Y:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微軟正黑體", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(1289, 268);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(322, 43);
            this.label5.TabIndex = 8;
            this.label5.Text = "Number of Outlier:";
            // 
            // txtTheta
            // 
            this.txtTheta.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtTheta.Location = new System.Drawing.Point(1609, 102);
            this.txtTheta.Name = "txtTheta";
            this.txtTheta.Size = new System.Drawing.Size(125, 47);
            this.txtTheta.TabIndex = 9;
            this.txtTheta.Text = "18";
            // 
            // txtDeltaX
            // 
            this.txtDeltaX.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtDeltaX.Location = new System.Drawing.Point(1609, 155);
            this.txtDeltaX.Name = "txtDeltaX";
            this.txtDeltaX.Size = new System.Drawing.Size(125, 47);
            this.txtDeltaX.TabIndex = 10;
            this.txtDeltaX.Text = "0";
            // 
            // txtDeltaY
            // 
            this.txtDeltaY.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtDeltaY.Location = new System.Drawing.Point(1609, 208);
            this.txtDeltaY.Name = "txtDeltaY";
            this.txtDeltaY.Size = new System.Drawing.Size(125, 47);
            this.txtDeltaY.TabIndex = 11;
            this.txtDeltaY.Text = "0";
            // 
            // txtNumOfOutlier
            // 
            this.txtNumOfOutlier.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtNumOfOutlier.Location = new System.Drawing.Point(1609, 261);
            this.txtNumOfOutlier.Name = "txtNumOfOutlier";
            this.txtNumOfOutlier.Size = new System.Drawing.Size(125, 47);
            this.txtNumOfOutlier.TabIndex = 12;
            this.txtNumOfOutlier.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1853, 1023);
            this.Controls.Add(this.txtNumOfOutlier);
            this.Controls.Add(this.txtDeltaY);
            this.Controls.Add(this.txtDeltaX);
            this.Controls.Add(this.txtTheta);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.plotView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OxyPlot.WindowsForms.PlotView plotView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTheta;
        private System.Windows.Forms.TextBox txtDeltaX;
        private System.Windows.Forms.TextBox txtDeltaY;
        private System.Windows.Forms.TextBox txtNumOfOutlier;
    }
}

