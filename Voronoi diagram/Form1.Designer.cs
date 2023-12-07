
namespace Voronoi_diagram
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.modeButton = new System.Windows.Forms.Button();
			this.clearButton = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.mousePosLabel = new System.Windows.Forms.Label();
			this.fileButton = new System.Windows.Forms.Button();
			this.stepButton = new System.Windows.Forms.Button();
			this.runButton = new System.Windows.Forms.Button();
			this.dataLabel = new System.Windows.Forms.Label();
			this.paintArea = new System.Windows.Forms.Panel();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.modeButton);
			this.splitContainer1.Panel1.Controls.Add(this.clearButton);
			this.splitContainer1.Panel1.Controls.Add(this.textBox1);
			this.splitContainer1.Panel1.Controls.Add(this.mousePosLabel);
			this.splitContainer1.Panel1.Controls.Add(this.fileButton);
			this.splitContainer1.Panel1.Controls.Add(this.stepButton);
			this.splitContainer1.Panel1.Controls.Add(this.runButton);
			this.splitContainer1.Panel1.Controls.Add(this.dataLabel);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.paintArea);
			this.splitContainer1.Size = new System.Drawing.Size(982, 653);
			this.splitContainer1.SplitterDistance = 344;
			this.splitContainer1.TabIndex = 0;
			// 
			// modeButton
			// 
			this.modeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(249)))), ((int)(((byte)(243)))));
			this.modeButton.Location = new System.Drawing.Point(26, 266);
			this.modeButton.Name = "modeButton";
			this.modeButton.Size = new System.Drawing.Size(105, 29);
			this.modeButton.TabIndex = 7;
			this.modeButton.Text = "input mode";
			this.modeButton.UseVisualStyleBackColor = false;
			this.modeButton.Click += new System.EventHandler(this.modeButton_Click);
			// 
			// clearButton
			// 
			this.clearButton.Location = new System.Drawing.Point(26, 194);
			this.clearButton.Name = "clearButton";
			this.clearButton.Size = new System.Drawing.Size(94, 29);
			this.clearButton.TabIndex = 6;
			this.clearButton.Text = "clear";
			this.clearButton.UseVisualStyleBackColor = true;
			this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(12, 351);
			this.textBox1.MaximumSize = new System.Drawing.Size(322, 290);
			this.textBox1.MinimumSize = new System.Drawing.Size(322, 290);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(322, 290);
			this.textBox1.TabIndex = 5;
			this.textBox1.Text = "6\r\n40 120\r\n100 50\r\n150 270\r\n340 230\r\n400 500\r\n520 300";
			// 
			// mousePosLabel
			// 
			this.mousePosLabel.AutoSize = true;
			this.mousePosLabel.Location = new System.Drawing.Point(200, 45);
			this.mousePosLabel.Name = "mousePosLabel";
			this.mousePosLabel.Size = new System.Drawing.Size(81, 19);
			this.mousePosLabel.TabIndex = 4;
			this.mousePosLabel.Text = "mousePos";
			// 
			// fileButton
			// 
			this.fileButton.Location = new System.Drawing.Point(26, 40);
			this.fileButton.Name = "fileButton";
			this.fileButton.Size = new System.Drawing.Size(94, 29);
			this.fileButton.TabIndex = 3;
			this.fileButton.Text = "choose file";
			this.fileButton.UseVisualStyleBackColor = true;
			this.fileButton.Click += new System.EventHandler(this.fileButton_Click);
			// 
			// stepButton
			// 
			this.stepButton.Location = new System.Drawing.Point(189, 116);
			this.stepButton.Name = "stepButton";
			this.stepButton.Size = new System.Drawing.Size(111, 29);
			this.stepButton.TabIndex = 2;
			this.stepButton.Text = "step by step";
			this.stepButton.UseVisualStyleBackColor = true;
			this.stepButton.Click += new System.EventHandler(this.stepButton_Click);
			// 
			// runButton
			// 
			this.runButton.Location = new System.Drawing.Point(26, 116);
			this.runButton.Name = "runButton";
			this.runButton.Size = new System.Drawing.Size(94, 29);
			this.runButton.TabIndex = 1;
			this.runButton.Text = "Run";
			this.runButton.UseVisualStyleBackColor = true;
			this.runButton.Click += new System.EventHandler(this.runButton_Click);
			// 
			// dataLabel
			// 
			this.dataLabel.AutoSize = true;
			this.dataLabel.Location = new System.Drawing.Point(12, 329);
			this.dataLabel.Name = "dataLabel";
			this.dataLabel.Size = new System.Drawing.Size(40, 19);
			this.dataLabel.TabIndex = 0;
			this.dataLabel.Text = "data";
			// 
			// paintArea
			// 
			this.paintArea.BackColor = System.Drawing.SystemColors.Window;
			this.paintArea.Cursor = System.Windows.Forms.Cursors.Cross;
			this.paintArea.Location = new System.Drawing.Point(16, 27);
			this.paintArea.MaximumSize = new System.Drawing.Size(600, 600);
			this.paintArea.MinimumSize = new System.Drawing.Size(600, 600);
			this.paintArea.Name = "paintArea";
			this.paintArea.Size = new System.Drawing.Size(600, 600);
			this.paintArea.TabIndex = 0;
			this.paintArea.MouseClick += new System.Windows.Forms.MouseEventHandler(this.paintArea_MouseClick);
			this.paintArea.MouseMove += new System.Windows.Forms.MouseEventHandler(this.paintArea_MouseMove);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(982, 653);
			this.Controls.Add(this.splitContainer1);
			this.MaximumSize = new System.Drawing.Size(1000, 700);
			this.MinimumSize = new System.Drawing.Size(1000, 700);
			this.Name = "Form1";
			this.Text = "Form1";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Label dataLabel;
		private System.Windows.Forms.Button runButton;
		private System.Windows.Forms.Button fileButton;
		private System.Windows.Forms.Button stepButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Label mousePosLabel;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Panel paintArea;
		private System.Windows.Forms.Button clearButton;
		private System.Windows.Forms.Button modeButton;
	}
}

