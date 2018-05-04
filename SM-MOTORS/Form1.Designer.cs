namespace SM_MOTORS
{
    partial class Form1
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
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnSaveTempale = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.btnUpdateImages = new System.Windows.Forms.Button();
            this.tbLoginBike = new System.Windows.Forms.TextBox();
            this.tbPasswordBike = new System.Windows.Forms.TextBox();
            this.gbBike18 = new System.Windows.Forms.GroupBox();
            this.gbSMMOTORS = new System.Windows.Forms.GroupBox();
            this.tbLoginSM = new System.Windows.Forms.TextBox();
            this.tbPasswordSM = new System.Windows.Forms.TextBox();
            this.cbFullText = new System.Windows.Forms.CheckBox();
            this.cbSEO = new System.Windows.Forms.CheckBox();
            this.cbMiniText = new System.Windows.Forms.CheckBox();
            this.btnRazdels = new System.Windows.Forms.Button();
            this.gbBike18.SuspendLayout();
            this.gbSMMOTORS.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(454, 12);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(208, 23);
            this.btnUpdate.TabIndex = 0;
            this.btnUpdate.Text = "Обработать запчасти";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSaveTempale
            // 
            this.btnSaveTempale.Location = new System.Drawing.Point(454, 70);
            this.btnSaveTempale.Name = "btnSaveTempale";
            this.btnSaveTempale.Size = new System.Drawing.Size(208, 31);
            this.btnSaveTempale.TabIndex = 2;
            this.btnSaveTempale.Text = "Сохранить текст";
            this.btnSaveTempale.UseVisualStyleBackColor = true;
            this.btnSaveTempale.Click += new System.EventHandler(this.button2_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(3, 9);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(445, 111);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(2, 135);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(445, 111);
            this.richTextBox2.TabIndex = 4;
            this.richTextBox2.Text = "";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(2, 252);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(445, 20);
            this.textBox1.TabIndex = 5;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(2, 278);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(445, 20);
            this.textBox2.TabIndex = 6;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(2, 304);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(445, 20);
            this.textBox3.TabIndex = 7;
            // 
            // btnUpdateImages
            // 
            this.btnUpdateImages.Location = new System.Drawing.Point(454, 293);
            this.btnUpdateImages.Name = "btnUpdateImages";
            this.btnUpdateImages.Size = new System.Drawing.Size(208, 31);
            this.btnUpdateImages.TabIndex = 8;
            this.btnUpdateImages.Text = "Обновить картинки";
            this.btnUpdateImages.UseVisualStyleBackColor = true;
            this.btnUpdateImages.Visible = false;
            this.btnUpdateImages.Click += new System.EventHandler(this.button3_Click);
            // 
            // tbLoginBike
            // 
            this.tbLoginBike.Location = new System.Drawing.Point(0, 19);
            this.tbLoginBike.Name = "tbLoginBike";
            this.tbLoginBike.Size = new System.Drawing.Size(100, 20);
            this.tbLoginBike.TabIndex = 9;
            // 
            // tbPasswordBike
            // 
            this.tbPasswordBike.Location = new System.Drawing.Point(106, 19);
            this.tbPasswordBike.Name = "tbPasswordBike";
            this.tbPasswordBike.Size = new System.Drawing.Size(100, 20);
            this.tbPasswordBike.TabIndex = 10;
            this.tbPasswordBike.UseSystemPasswordChar = true;
            // 
            // gbBike18
            // 
            this.gbBike18.Controls.Add(this.tbLoginBike);
            this.gbBike18.Controls.Add(this.tbPasswordBike);
            this.gbBike18.Location = new System.Drawing.Point(454, 107);
            this.gbBike18.Name = "gbBike18";
            this.gbBike18.Size = new System.Drawing.Size(208, 47);
            this.gbBike18.TabIndex = 11;
            this.gbBike18.TabStop = false;
            this.gbBike18.Text = "Bike18.ru";
            // 
            // gbSMMOTORS
            // 
            this.gbSMMOTORS.Controls.Add(this.tbLoginSM);
            this.gbSMMOTORS.Controls.Add(this.tbPasswordSM);
            this.gbSMMOTORS.Location = new System.Drawing.Point(454, 160);
            this.gbSMMOTORS.Name = "gbSMMOTORS";
            this.gbSMMOTORS.Size = new System.Drawing.Size(208, 47);
            this.gbSMMOTORS.TabIndex = 12;
            this.gbSMMOTORS.TabStop = false;
            this.gbSMMOTORS.Text = "SM-MOTORS";
            // 
            // tbLoginSM
            // 
            this.tbLoginSM.Location = new System.Drawing.Point(0, 19);
            this.tbLoginSM.Name = "tbLoginSM";
            this.tbLoginSM.Size = new System.Drawing.Size(100, 20);
            this.tbLoginSM.TabIndex = 9;
            // 
            // tbPasswordSM
            // 
            this.tbPasswordSM.Location = new System.Drawing.Point(106, 19);
            this.tbPasswordSM.Name = "tbPasswordSM";
            this.tbPasswordSM.Size = new System.Drawing.Size(100, 20);
            this.tbPasswordSM.TabIndex = 10;
            this.tbPasswordSM.UseSystemPasswordChar = true;
            // 
            // cbFullText
            // 
            this.cbFullText.AutoSize = true;
            this.cbFullText.Location = new System.Drawing.Point(453, 236);
            this.cbFullText.Name = "cbFullText";
            this.cbFullText.Size = new System.Drawing.Size(160, 17);
            this.cbFullText.TabIndex = 13;
            this.cbFullText.Text = "Замена полного описания";
            this.cbFullText.UseVisualStyleBackColor = true;
            // 
            // cbSEO
            // 
            this.cbSEO.AutoSize = true;
            this.cbSEO.Checked = true;
            this.cbSEO.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSEO.Location = new System.Drawing.Point(454, 259);
            this.cbSEO.Name = "cbSEO";
            this.cbSEO.Size = new System.Drawing.Size(90, 17);
            this.cbSEO.TabIndex = 14;
            this.cbSEO.Text = "Замена СЕО";
            this.cbSEO.UseVisualStyleBackColor = true;
            // 
            // cbMiniText
            // 
            this.cbMiniText.AutoSize = true;
            this.cbMiniText.Checked = true;
            this.cbMiniText.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMiniText.Location = new System.Drawing.Point(453, 213);
            this.cbMiniText.Name = "cbMiniText";
            this.cbMiniText.Size = new System.Drawing.Size(165, 17);
            this.cbMiniText.TabIndex = 15;
            this.cbMiniText.Text = "Замена краткого описания";
            this.cbMiniText.UseVisualStyleBackColor = true;
            // 
            // btnRazdels
            // 
            this.btnRazdels.Location = new System.Drawing.Point(454, 41);
            this.btnRazdels.Name = "btnRazdels";
            this.btnRazdels.Size = new System.Drawing.Size(206, 23);
            this.btnRazdels.TabIndex = 17;
            this.btnRazdels.Text = "Отдельные разделы";
            this.btnRazdels.UseVisualStyleBackColor = true;
            this.btnRazdels.Click += new System.EventHandler(this.btnRazdels_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 344);
            this.Controls.Add(this.btnRazdels);
            this.Controls.Add(this.cbMiniText);
            this.Controls.Add(this.cbSEO);
            this.Controls.Add(this.cbFullText);
            this.Controls.Add(this.gbSMMOTORS);
            this.Controls.Add(this.gbBike18);
            this.Controls.Add(this.btnUpdateImages);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnSaveTempale);
            this.Controls.Add(this.btnUpdate);
            this.Name = "Form1";
            this.Text = "Запчасти SM-MOTORS";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gbBike18.ResumeLayout(false);
            this.gbBike18.PerformLayout();
            this.gbSMMOTORS.ResumeLayout(false);
            this.gbSMMOTORS.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnSaveTempale;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button btnUpdateImages;
        private System.Windows.Forms.TextBox tbLoginBike;
        private System.Windows.Forms.TextBox tbPasswordBike;
        private System.Windows.Forms.GroupBox gbBike18;
        private System.Windows.Forms.GroupBox gbSMMOTORS;
        private System.Windows.Forms.TextBox tbLoginSM;
        private System.Windows.Forms.TextBox tbPasswordSM;
        private System.Windows.Forms.CheckBox cbFullText;
        private System.Windows.Forms.CheckBox cbSEO;
        private System.Windows.Forms.CheckBox cbMiniText;
        private System.Windows.Forms.Button btnRazdels;
    }
}

