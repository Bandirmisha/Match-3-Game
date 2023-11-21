namespace Match_3
{
    partial class MainMenu
    {
        private System.ComponentModel.IContainer components = null;
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 15.8571434F, FontStyle.Regular, GraphicsUnit.Point);
            button1.Location = new Point(165, 108);
            button1.Name = "button1";
            button1.Size = new Size(442, 187);
            button1.TabIndex = 0;
            button1.Text = "Начать игру";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // MainMenu
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Name = "MainMenu";
            Text = "Main Menu";
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
    }
}