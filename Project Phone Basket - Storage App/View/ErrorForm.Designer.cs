namespace Project_Phone_Basket___Storage_App.View
{
    partial class ErrorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.message = new System.Windows.Forms.Label();
            this.close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // message
            // 
            this.message.AutoEllipsis = true;
            this.message.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.message.Location = new System.Drawing.Point(12, 13);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(512, 23);
            this.message.TabIndex = 0;
            this.message.Text = "{Insertar mensaje de error}";
            this.message.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // close
            // 
            this.close.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close.Location = new System.Drawing.Point(218, 46);
            this.close.Margin = new System.Windows.Forms.Padding(5, 10, 5, 10);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(100, 30);
            this.close.TabIndex = 1;
            this.close.Text = "Tancar";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.button1_Click);
            // 
            // ErrorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 86);
            this.Controls.Add(this.close);
            this.Controls.Add(this.message);
            this.Name = "ErrorForm";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label message;
        public System.Windows.Forms.Button close;
    }
}