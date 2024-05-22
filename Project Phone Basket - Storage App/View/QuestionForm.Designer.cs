namespace Project_Phone_Basket___Storage_App.View
{
    partial class QuestionForm
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
            this.yes = new System.Windows.Forms.Button();
            this.message = new System.Windows.Forms.Label();
            this.no = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // yes
            // 
            this.yes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yes.Location = new System.Drawing.Point(163, 46);
            this.yes.Margin = new System.Windows.Forms.Padding(5, 10, 5, 5);
            this.yes.Name = "yes";
            this.yes.Size = new System.Drawing.Size(100, 30);
            this.yes.TabIndex = 3;
            this.yes.Text = "Si";
            this.yes.UseVisualStyleBackColor = true;
            // 
            // message
            // 
            this.message.AutoEllipsis = true;
            this.message.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.message.Location = new System.Drawing.Point(12, 13);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(512, 23);
            this.message.TabIndex = 2;
            this.message.Text = "{Insertar pregunta}";
            this.message.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // no
            // 
            this.no.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.no.Location = new System.Drawing.Point(273, 46);
            this.no.Margin = new System.Windows.Forms.Padding(5, 10, 5, 5);
            this.no.Name = "no";
            this.no.Size = new System.Drawing.Size(100, 30);
            this.no.TabIndex = 4;
            this.no.Text = "No";
            this.no.UseVisualStyleBackColor = true;
            // 
            // QuestionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 86);
            this.Controls.Add(this.no);
            this.Controls.Add(this.yes);
            this.Controls.Add(this.message);
            this.Name = "QuestionForm";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button yes;
        public System.Windows.Forms.Label message;
        public System.Windows.Forms.Button no;
    }
}