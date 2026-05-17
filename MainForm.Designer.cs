namespace TicTacToe
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // ── Form itself ──────────────────────────────────
            this.Name = "MainForm";
            this.Text = "Tic‑Tac‑Toe  ✦  MinMax AI";
            this.BackColor = System.Drawing.Color.FromArgb(15, 15, 26);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font = new System.Drawing.Font("Courier New", 9.5f,
                                       System.Drawing.FontStyle.Regular,
                                       System.Drawing.GraphicsUnit.Point);

            // ClientSize is set in MainForm.cs after all controls are measured
            this.ResumeLayout(false);
        }
    }
}