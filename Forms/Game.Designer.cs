namespace Match_3
{
    partial class Game
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
            scoreTB = new TextBox();
            timeLeftTB = new TextBox();
            SuspendLayout();
            // 
            // scoreTB
            // 
            scoreTB.BackColor = SystemColors.Menu;
            scoreTB.BorderStyle = BorderStyle.None;
            scoreTB.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            scoreTB.Location = new Point(12, 803);
            scoreTB.Name = "scoreTB";
            scoreTB.ReadOnly = true;
            scoreTB.Size = new Size(175, 50);
            scoreTB.TabIndex = 0;
            scoreTB.Text = "Score: 0";
            // 
            // timeLeftTB
            // 
            timeLeftTB.BackColor = SystemColors.Menu;
            timeLeftTB.BorderStyle = BorderStyle.None;
            timeLeftTB.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            timeLeftTB.Location = new Point(555, 818);
            timeLeftTB.Name = "timeLeftTB";
            timeLeftTB.ReadOnly = true;
            timeLeftTB.Size = new Size(233, 35);
            timeLeftTB.TabIndex = 0;
            timeLeftTB.Text = "Time left: ";
            // 
            // Game
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 865);
            Controls.Add(timeLeftTB);
            Controls.Add(scoreTB);
            Name = "Game";
            Text = "Game";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox scoreTB;
        private TextBox timeLeftTB;
    }
}