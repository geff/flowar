namespace AStar
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCalcPath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCalcPath
            // 
            this.btnCalcPath.Location = new System.Drawing.Point(-1, 0);
            this.btnCalcPath.Name = "btnCalcPath";
            this.btnCalcPath.Size = new System.Drawing.Size(38, 23);
            this.btnCalcPath.TabIndex = 0;
            this.btnCalcPath.Text = "Ok";
            this.btnCalcPath.UseVisualStyleBackColor = true;
            this.btnCalcPath.Click += new System.EventHandler(this.btnCalcPath_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.btnCalcPath);
            this.Location = new System.Drawing.Point(550, 600);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.Click += new System.EventHandler(this.Form1_Click);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCalcPath;
    }
}

