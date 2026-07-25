namespace PacmanGame.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private PacmanGame.Controls.BufferedPanel GamePanel;
        private System.Windows.Forms.Panel PnlStatus;
        private System.Windows.Forms.Label LblScore;
        private System.Windows.Forms.Label LblLevel;
        private System.Windows.Forms.Label LblLives;
        private System.Windows.Forms.Timer PlayerTimer;
        private System.Windows.Forms.Timer GhostTimer;
        private System.Windows.Forms.Timer VulnerableTimer;
        private System.Windows.Forms.Timer LevelTransitionTimer;

        protected override void Dispose(bool Disposing)
        {
            if (Disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(Disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.PnlStatus = new System.Windows.Forms.Panel();
            this.LblScore = new System.Windows.Forms.Label();
            this.LblLevel = new System.Windows.Forms.Label();
            this.LblLives = new System.Windows.Forms.Label();
            this.PlayerTimer = new System.Windows.Forms.Timer(this.components);
            this.GhostTimer = new System.Windows.Forms.Timer(this.components);
            this.VulnerableTimer = new System.Windows.Forms.Timer(this.components);
            this.LevelTransitionTimer = new System.Windows.Forms.Timer(this.components);
            this.GamePanel = new PacmanGame.Controls.BufferedPanel();
            this.PnlStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // PnlStatus
            // 
            this.PnlStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.PnlStatus.Controls.Add(this.LblScore);
            this.PnlStatus.Controls.Add(this.LblLevel);
            this.PnlStatus.Controls.Add(this.LblLives);
            this.PnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PnlStatus.Location = new System.Drawing.Point(0, 600);
            this.PnlStatus.Name = "PnlStatus";
            this.PnlStatus.Size = new System.Drawing.Size(780, 60);
            this.PnlStatus.TabIndex = 1;
            // 
            // LblScore
            // 
            this.LblScore.AutoSize = true;
            this.LblScore.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.LblScore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(120)))));
            this.LblScore.Location = new System.Drawing.Point(80, 11);
            this.LblScore.Name = "LblScore";
            this.LblScore.Size = new System.Drawing.Size(121, 38);
            this.LblScore.TabIndex = 0;
            this.LblScore.Text = "Score: 0";
            // 
            // LblLevel
            // 
            this.LblLevel.AutoSize = true;
            this.LblLevel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.LblLevel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(120)))));
            this.LblLevel.Location = new System.Drawing.Point(328, 11);
            this.LblLevel.Name = "LblLevel";
            this.LblLevel.Size = new System.Drawing.Size(116, 38);
            this.LblLevel.TabIndex = 1;
            this.LblLevel.Text = "Level: 1";
            // 
            // LblLives
            // 
            this.LblLives.AutoSize = true;
            this.LblLives.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.LblLives.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(120)))));
            this.LblLives.Location = new System.Drawing.Point(588, 11);
            this.LblLives.Name = "LblLives";
            this.LblLives.Size = new System.Drawing.Size(113, 38);
            this.LblLives.TabIndex = 2;
            this.LblLives.Text = "Lives: 3";
            // 
            // PlayerTimer
            // 
            this.PlayerTimer.Interval = 110;
            this.PlayerTimer.Tick += new System.EventHandler(this.PlayerTimer_Tick);
            // 
            // GhostTimer
            // 
            this.GhostTimer.Interval = 190;
            this.GhostTimer.Tick += new System.EventHandler(this.GhostTimer_Tick);
            // 
            // VulnerableTimer
            // 
            this.VulnerableTimer.Interval = 6000;
            this.VulnerableTimer.Tick += new System.EventHandler(this.VulnerableTimer_Tick);
            // 
            // LevelTransitionTimer
            // 
            this.LevelTransitionTimer.Interval = 1600;
            this.LevelTransitionTimer.Tick += new System.EventHandler(this.LevelTransitionTimer_Tick);
            // 
            // GamePanel
            // 
            this.GamePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(220)))));
            this.GamePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GamePanel.Location = new System.Drawing.Point(0, 0);
            this.GamePanel.Name = "GamePanel";
            this.GamePanel.Size = new System.Drawing.Size(780, 660);
            this.GamePanel.TabIndex = 0;
            this.GamePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.GamePanel_Paint);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(780, 660);
            this.Controls.Add(this.PnlStatus);
            this.Controls.Add(this.GamePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pacman Game";
            this.PnlStatus.ResumeLayout(false);
            this.PnlStatus.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
