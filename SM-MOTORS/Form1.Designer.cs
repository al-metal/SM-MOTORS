﻿namespace SM_MOTORS
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
            this.gbBike18.SuspendLayout();
            this.gbSMMOTORS.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(695, 3);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(208, 55);
            this.btnUpdate.TabIndex = 0;
            this.btnUpdate.Text = "Обработать SM-MOTORS";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSaveTempale
            // 
            this.btnSaveTempale.Location = new System.Drawing.Point(704, 301);
            this.btnSaveTempale.Name = "btnSaveTempale";
            this.btnSaveTempale.Size = new System.Drawing.Size(164, 23);
            this.btnSaveTempale.TabIndex = 2;
            this.btnSaveTempale.Text = "Сохранить текст";
            this.btnSaveTempale.UseVisualStyleBackColor = true;
            this.btnSaveTempale.Click += new System.EventHandler(this.button2_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(2, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(687, 111);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(2, 135);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(687, 111);
            this.richTextBox2.TabIndex = 4;
            this.richTextBox2.Text = "";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(2, 252);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(687, 20);
            this.textBox1.TabIndex = 5;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(2, 278);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(687, 20);
            this.textBox2.TabIndex = 6;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(2, 304);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(687, 20);
            this.textBox3.TabIndex = 7;
            // 
            // btnUpdateImages
            // 
            this.btnUpdateImages.Location = new System.Drawing.Point(695, 64);
            this.btnUpdateImages.Name = "btnUpdateImages";
            this.btnUpdateImages.Size = new System.Drawing.Size(208, 35);
            this.btnUpdateImages.TabIndex = 8;
            this.btnUpdateImages.Text = "Обновить картинки";
            this.btnUpdateImages.UseVisualStyleBackColor = true;
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
            this.gbBike18.Location = new System.Drawing.Point(696, 105);
            this.gbBike18.Name = "gbBike18";
            this.gbBike18.Size = new System.Drawing.Size(207, 47);
            this.gbBike18.TabIndex = 11;
            this.gbBike18.TabStop = false;
            this.gbBike18.Text = "Bike18.ru";
            // 
            // gbSMMOTORS
            // 
            this.gbSMMOTORS.Controls.Add(this.tbLoginSM);
            this.gbSMMOTORS.Controls.Add(this.tbPasswordSM);
            this.gbSMMOTORS.Location = new System.Drawing.Point(696, 158);
            this.gbSMMOTORS.Name = "gbSMMOTORS";
            this.gbSMMOTORS.Size = new System.Drawing.Size(207, 47);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 344);
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
            this.Text = "Form1";
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
    }
}

