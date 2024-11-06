namespace Base.UI
{
    partial class MenuCalPanel
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuCalPanel));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ucBtnExt2 = new HZH_Controls.Controls.UCBtnExt();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ucBtnExt1 = new HZH_Controls.Controls.UCBtnExt();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.ucCombox5 = new HZH_Controls.Controls.UCCombox();
            this.ucCombox4 = new HZH_Controls.Controls.UCCombox();
            this.ucCombox3 = new HZH_Controls.Controls.UCCombox();
            this.ucCombox2 = new HZH_Controls.Controls.UCCombox();
            this.ucCombox1 = new HZH_Controls.Controls.UCCombox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.textBox2);
            this.splitContainer2.Panel1.Controls.Add(this.label8);
            this.splitContainer2.Panel1.Controls.Add(this.textBox1);
            this.splitContainer2.Panel1.Controls.Add(this.label4);
            resources.ApplyResources(this.splitContainer2.Panel1, "splitContainer2.Panel1");
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.ucBtnExt2);
            this.splitContainer2.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer2.Panel2.Controls.Add(this.ucBtnExt1);
            // 
            // textBox2
            // 
            resources.ApplyResources(this.textBox2, "textBox2");
            this.textBox2.ForeColor = System.Drawing.Color.Black;
            this.textBox2.Name = "textBox2";
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Name = "label8";
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.ForeColor = System.Drawing.Color.Black;
            this.textBox1.Name = "textBox1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Name = "label4";
            // 
            // ucBtnExt2
            // 
            this.ucBtnExt2.BackColor = System.Drawing.Color.White;
            this.ucBtnExt2.BtnBackColor = System.Drawing.Color.White;
            this.ucBtnExt2.BtnFont = new System.Drawing.Font("华文中宋", 15.75F);
            this.ucBtnExt2.BtnForeColor = System.Drawing.Color.White;
            this.ucBtnExt2.BtnText = "清除设备缓存";
            this.ucBtnExt2.ConerRadius = 5;
            this.ucBtnExt2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ucBtnExt2.EnabledMouseEffect = false;
            this.ucBtnExt2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(161)))), ((int)(((byte)(103)))));
            resources.ApplyResources(this.ucBtnExt2, "ucBtnExt2");
            this.ucBtnExt2.IsRadius = true;
            this.ucBtnExt2.IsShowRect = true;
            this.ucBtnExt2.IsShowTips = false;
            this.ucBtnExt2.Name = "ucBtnExt2";
            this.ucBtnExt2.RectColor = System.Drawing.Color.Black;
            this.ucBtnExt2.RectWidth = 1;
            this.ucBtnExt2.TabStop = false;
            this.ucBtnExt2.TipsColor = System.Drawing.Color.MediumTurquoise;
            this.ucBtnExt2.TipsText = "";
            this.ucBtnExt2.BtnClick += new System.EventHandler(this.ucBtnExt2_BtnClick);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column9,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column8,
            this.Column5,
            this.Column6,
            this.Column7});
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView1_EditingControlShowing);
            this.dataGridView1.Leave += new System.EventHandler(this.dataGridView1_Leave);
            // 
            // Column1
            // 
            this.Column1.FillWeight = 57.67691F;
            resources.ApplyResources(this.Column1, "Column1");
            this.Column1.Name = "Column1";
            // 
            // Column9
            // 
            this.Column9.FillWeight = 93.51634F;
            resources.ApplyResources(this.Column9, "Column9");
            this.Column9.Name = "Column9";
            // 
            // Column2
            // 
            this.Column2.FillWeight = 107.9678F;
            resources.ApplyResources(this.Column2, "Column2");
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.FillWeight = 107.9768F;
            resources.ApplyResources(this.Column3, "Column3");
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.FillWeight = 107.7253F;
            resources.ApplyResources(this.Column4, "Column4");
            this.Column4.Name = "Column4";
            // 
            // Column8
            // 
            this.Column8.FillWeight = 107.2756F;
            resources.ApplyResources(this.Column8, "Column8");
            this.Column8.Name = "Column8";
            // 
            // Column5
            // 
            this.Column5.FillWeight = 106.6791F;
            resources.ApplyResources(this.Column5, "Column5");
            this.Column5.Name = "Column5";
            // 
            // Column6
            // 
            this.Column6.FillWeight = 105.9775F;
            resources.ApplyResources(this.Column6, "Column6");
            this.Column6.Name = "Column6";
            // 
            // Column7
            // 
            this.Column7.FillWeight = 105.2047F;
            resources.ApplyResources(this.Column7, "Column7");
            this.Column7.Name = "Column7";
            // 
            // ucBtnExt1
            // 
            this.ucBtnExt1.BackColor = System.Drawing.Color.White;
            this.ucBtnExt1.BtnBackColor = System.Drawing.Color.White;
            this.ucBtnExt1.BtnFont = new System.Drawing.Font("华文中宋", 15.75F);
            this.ucBtnExt1.BtnForeColor = System.Drawing.Color.White;
            this.ucBtnExt1.BtnText = "确认";
            this.ucBtnExt1.ConerRadius = 5;
            this.ucBtnExt1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ucBtnExt1.EnabledMouseEffect = false;
            this.ucBtnExt1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(161)))), ((int)(((byte)(103)))));
            resources.ApplyResources(this.ucBtnExt1, "ucBtnExt1");
            this.ucBtnExt1.IsRadius = true;
            this.ucBtnExt1.IsShowRect = true;
            this.ucBtnExt1.IsShowTips = false;
            this.ucBtnExt1.Name = "ucBtnExt1";
            this.ucBtnExt1.RectColor = System.Drawing.Color.Black;
            this.ucBtnExt1.RectWidth = 1;
            this.ucBtnExt1.TabStop = false;
            this.ucBtnExt1.TipsColor = System.Drawing.Color.MediumTurquoise;
            this.ucBtnExt1.TipsText = "";
            this.ucBtnExt1.BtnClick += new System.EventHandler(this.ucBtnExt1_BtnClick);
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBox4);
            this.splitContainer1.Panel1.Controls.Add(this.ucCombox5);
            this.splitContainer1.Panel1.Controls.Add(this.ucCombox4);
            this.splitContainer1.Panel1.Controls.Add(this.ucCombox3);
            this.splitContainer1.Panel1.Controls.Add(this.ucCombox2);
            this.splitContainer1.Panel1.Controls.Add(this.ucCombox1);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label10);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            // 
            // textBox4
            // 
            resources.ApplyResources(this.textBox4, "textBox4");
            this.textBox4.ForeColor = System.Drawing.Color.Black;
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            // 
            // ucCombox5
            // 
            this.ucCombox5.BackColor = System.Drawing.Color.Transparent;
            this.ucCombox5.BackColorExt = System.Drawing.SystemColors.Control;
            this.ucCombox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucCombox5.BoxStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ucCombox5.ConerRadius = 5;
            this.ucCombox5.DropPanelHeight = -1;
            this.ucCombox5.FillColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.ucCombox5, "ucCombox5");
            this.ucCombox5.IsRadius = true;
            this.ucCombox5.IsShowRect = true;
            this.ucCombox5.ItemWidth = 70;
            this.ucCombox5.Name = "ucCombox5";
            this.ucCombox5.RectColor = System.Drawing.SystemColors.Control;
            this.ucCombox5.RectWidth = 1;
            this.ucCombox5.SelectedIndex = -1;
            this.ucCombox5.SelectedValue = "";
            this.ucCombox5.Source = null;
            this.ucCombox5.TextValue = null;
            this.ucCombox5.TriangleColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(77)))), ((int)(((byte)(59)))));
            // 
            // ucCombox4
            // 
            this.ucCombox4.BackColor = System.Drawing.Color.Transparent;
            this.ucCombox4.BackColorExt = System.Drawing.SystemColors.Control;
            this.ucCombox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucCombox4.BoxStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ucCombox4.ConerRadius = 5;
            this.ucCombox4.DropPanelHeight = -1;
            this.ucCombox4.FillColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.ucCombox4, "ucCombox4");
            this.ucCombox4.IsRadius = true;
            this.ucCombox4.IsShowRect = true;
            this.ucCombox4.ItemWidth = 70;
            this.ucCombox4.Name = "ucCombox4";
            this.ucCombox4.RectColor = System.Drawing.SystemColors.Control;
            this.ucCombox4.RectWidth = 1;
            this.ucCombox4.SelectedIndex = -1;
            this.ucCombox4.SelectedValue = "";
            this.ucCombox4.Source = null;
            this.ucCombox4.TextValue = null;
            this.ucCombox4.TriangleColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(77)))), ((int)(((byte)(59)))));
            // 
            // ucCombox3
            // 
            this.ucCombox3.BackColor = System.Drawing.Color.Transparent;
            this.ucCombox3.BackColorExt = System.Drawing.SystemColors.Control;
            this.ucCombox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucCombox3.BoxStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ucCombox3.ConerRadius = 5;
            this.ucCombox3.DropPanelHeight = -1;
            this.ucCombox3.FillColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.ucCombox3, "ucCombox3");
            this.ucCombox3.IsRadius = true;
            this.ucCombox3.IsShowRect = true;
            this.ucCombox3.ItemWidth = 70;
            this.ucCombox3.Name = "ucCombox3";
            this.ucCombox3.RectColor = System.Drawing.SystemColors.Control;
            this.ucCombox3.RectWidth = 1;
            this.ucCombox3.SelectedIndex = -1;
            this.ucCombox3.SelectedValue = "";
            this.ucCombox3.Source = null;
            this.ucCombox3.TextValue = null;
            this.ucCombox3.TriangleColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(77)))), ((int)(((byte)(59)))));
            // 
            // ucCombox2
            // 
            this.ucCombox2.BackColor = System.Drawing.Color.Transparent;
            this.ucCombox2.BackColorExt = System.Drawing.SystemColors.Control;
            this.ucCombox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucCombox2.BoxStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ucCombox2.ConerRadius = 5;
            this.ucCombox2.DropPanelHeight = -1;
            this.ucCombox2.FillColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.ucCombox2, "ucCombox2");
            this.ucCombox2.IsRadius = true;
            this.ucCombox2.IsShowRect = true;
            this.ucCombox2.ItemWidth = 70;
            this.ucCombox2.Name = "ucCombox2";
            this.ucCombox2.RectColor = System.Drawing.SystemColors.Control;
            this.ucCombox2.RectWidth = 1;
            this.ucCombox2.SelectedIndex = -1;
            this.ucCombox2.SelectedValue = "";
            this.ucCombox2.Source = null;
            this.ucCombox2.TextValue = null;
            this.ucCombox2.TriangleColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(77)))), ((int)(((byte)(59)))));
            // 
            // ucCombox1
            // 
            this.ucCombox1.BackColor = System.Drawing.Color.Transparent;
            this.ucCombox1.BackColorExt = System.Drawing.SystemColors.Control;
            this.ucCombox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucCombox1.BoxStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ucCombox1.ConerRadius = 5;
            this.ucCombox1.DropPanelHeight = -1;
            this.ucCombox1.FillColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.ucCombox1, "ucCombox1");
            this.ucCombox1.IsRadius = true;
            this.ucCombox1.IsShowRect = true;
            this.ucCombox1.ItemWidth = 70;
            this.ucCombox1.Name = "ucCombox1";
            this.ucCombox1.RectColor = System.Drawing.SystemColors.Control;
            this.ucCombox1.RectWidth = 1;
            this.ucCombox1.SelectedIndex = -1;
            this.ucCombox1.SelectedValue = "";
            this.ucCombox1.Source = null;
            this.ucCombox1.TextValue = "";
            this.ucCombox1.TriangleColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(77)))), ((int)(((byte)(59)))));
            this.ucCombox1.TextChangedEvent += new System.EventHandler(this.ucCombox1_TextChangedEvent);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Name = "label1";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Name = "label6";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Name = "label3";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Name = "label10";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // MenuCalPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Honeydew;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label9);
            this.Name = "MenuCalPanel";
            resources.ApplyResources(this, "$this");
            this.VisibleChanged += new System.EventHandler(this.MenuCalPanel_VisibleChanged);
            this.Resize += new System.EventHandler(this.MenuCalPanel_Resize);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private HZH_Controls.Controls.UCCombox ucCombox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label3;
        private HZH_Controls.Controls.UCCombox ucCombox3;
        private System.Windows.Forms.Label label1;
        private HZH_Controls.Controls.UCCombox ucCombox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private HZH_Controls.Controls.UCCombox ucCombox4;
        private System.Windows.Forms.Label label6;
        private HZH_Controls.Controls.UCCombox ucCombox5;
        private System.Windows.Forms.TextBox textBox1;
        private HZH_Controls.Controls.UCBtnExt ucBtnExt1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label10;
        private HZH_Controls.Controls.UCBtnExt ucBtnExt2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
    }
}
