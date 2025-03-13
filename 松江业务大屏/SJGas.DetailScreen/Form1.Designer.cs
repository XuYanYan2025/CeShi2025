namespace DetailScreen
{
    partial class Form1
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            DevExpress.XtraCharts.SimpleDiagram simpleDiagram4 = new DevExpress.XtraCharts.SimpleDiagram();
            DevExpress.XtraCharts.Series series4 = new DevExpress.XtraCharts.Series();
            DevExpress.XtraCharts.PieSeriesLabel pieSeriesLabel4 = new DevExpress.XtraCharts.PieSeriesLabel();
            DevExpress.XtraCharts.PiePointOptions piePointOptions4 = new DevExpress.XtraCharts.PiePointOptions();
            DevExpress.XtraCharts.PieSeriesView pieSeriesView4 = new DevExpress.XtraCharts.PieSeriesView();
            this.PL_HuaWu = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.PL_YeWu = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.chartControl3 = new DevExpress.XtraCharts.ChartControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chartControl2 = new DevExpress.XtraCharts.ChartControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chartControl1 = new DevExpress.XtraCharts.ChartControl();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.PL_JieDian = new System.Windows.Forms.Panel();
            this.chartControl4 = new DevExpress.XtraCharts.ChartControl();
            this.PL_HuaWu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.PL_YeWu.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(simpleDiagram4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(pieSeriesLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(pieSeriesView4)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl2)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl1)).BeginInit();
            this.PL_JieDian.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl4)).BeginInit();
            this.SuspendLayout();
            // 
            // PL_HuaWu
            // 
            this.PL_HuaWu.Controls.Add(this.dataGridView1);
            this.PL_HuaWu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PL_HuaWu.Location = new System.Drawing.Point(0, 0);
            this.PL_HuaWu.Name = "PL_HuaWu";
            this.PL_HuaWu.Size = new System.Drawing.Size(1008, 729);
            this.PL_HuaWu.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle13.BackColor = System.Drawing.Color.Linen;
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle13;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle14;
            this.dataGridView1.ColumnHeadersHeight = 40;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.Color.Red;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.Khaki;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle15;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridView1.MinimumSize = new System.Drawing.Size(1228, 458);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("宋体", 9F);
            dataGridViewCellStyle16.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle16;
            this.dataGridView1.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dataGridView1.RowTemplate.Height = 40;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.ShowCellErrors = false;
            this.dataGridView1.ShowCellToolTips = false;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.ShowRowErrors = false;
            this.dataGridView1.Size = new System.Drawing.Size(1228, 729);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.VirtualMode = true;
            this.dataGridView1.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView1_CellValueNeeded);
            // 
            // PL_YeWu
            // 
            this.PL_YeWu.Controls.Add(this.panel3);
            this.PL_YeWu.Controls.Add(this.panel2);
            this.PL_YeWu.Controls.Add(this.panel1);
            this.PL_YeWu.Controls.Add(this.label1);
            this.PL_YeWu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PL_YeWu.Location = new System.Drawing.Point(0, 0);
            this.PL_YeWu.Name = "PL_YeWu";
            this.PL_YeWu.Size = new System.Drawing.Size(1008, 729);
            this.PL_YeWu.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.chartControl3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(678, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(330, 729);
            this.panel3.TabIndex = 3;
            // 
            // chartControl3
            // 
            simpleDiagram4.EqualPieSize = false;
            this.chartControl3.Diagram = simpleDiagram4;
            this.chartControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControl3.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Center;
            this.chartControl3.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.BottomOutside;
            this.chartControl3.Legend.Direction = DevExpress.XtraCharts.LegendDirection.LeftToRight;
            this.chartControl3.Legend.Font = new System.Drawing.Font("Tahoma", 18F);
            this.chartControl3.Location = new System.Drawing.Point(0, 0);
            this.chartControl3.Name = "chartControl3";
            this.chartControl3.Padding.Bottom = 0;
            this.chartControl3.Padding.Left = 0;
            this.chartControl3.Padding.Right = 0;
            this.chartControl3.Padding.Top = 0;
            piePointOptions4.ValueNumericOptions.Format = DevExpress.XtraCharts.NumericFormat.Percent;
            pieSeriesLabel4.PointOptions = piePointOptions4;
            series4.Label = pieSeriesLabel4;
            series4.Name = "Series 1";
            pieSeriesView4.RuntimeExploding = false;
            series4.View = pieSeriesView4;
            this.chartControl3.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series4};
            this.chartControl3.Size = new System.Drawing.Size(330, 729);
            this.chartControl3.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.chartControl2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(342, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(341, 729);
            this.panel2.TabIndex = 2;
            // 
            // chartControl2
            // 
            this.chartControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControl2.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Center;
            this.chartControl2.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.BottomOutside;
            this.chartControl2.Legend.Font = new System.Drawing.Font("Tahoma", 18F);
            this.chartControl2.Location = new System.Drawing.Point(0, 0);
            this.chartControl2.Name = "chartControl2";
            this.chartControl2.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartControl2.Size = new System.Drawing.Size(341, 729);
            this.chartControl2.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chartControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(342, 729);
            this.panel1.TabIndex = 1;
            // 
            // chartControl1
            // 
            this.chartControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControl1.EmptyChartText.Font = new System.Drawing.Font("Tahoma", 20F);
            this.chartControl1.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Center;
            this.chartControl1.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.BottomOutside;
            this.chartControl1.Legend.Font = new System.Drawing.Font("Tahoma", 18F);
            this.chartControl1.Location = new System.Drawing.Point(0, 0);
            this.chartControl1.Name = "chartControl1";
            this.chartControl1.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartControl1.Size = new System.Drawing.Size(342, 729);
            this.chartControl1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("华文中宋", 42F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(396, 299);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(420, 64);
            this.label1.TabIndex = 0;
            this.label1.Text = "業務工單統計表";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // PL_JieDian
            // 
            this.PL_JieDian.Controls.Add(this.chartControl4);
            this.PL_JieDian.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PL_JieDian.Location = new System.Drawing.Point(0, 0);
            this.PL_JieDian.Name = "PL_JieDian";
            this.PL_JieDian.Size = new System.Drawing.Size(1008, 729);
            this.PL_JieDian.TabIndex = 5;
            // 
            // chartControl4
            // 
            this.chartControl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControl4.Legend.Font = new System.Drawing.Font("Tahoma", 24F);
            this.chartControl4.Location = new System.Drawing.Point(0, 0);
            this.chartControl4.Name = "chartControl4";
            this.chartControl4.Padding.Bottom = 30;
            this.chartControl4.Padding.Left = 30;
            this.chartControl4.Padding.Right = 30;
            this.chartControl4.Padding.Top = 30;
            this.chartControl4.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.chartControl4.Size = new System.Drawing.Size(1008, 729);
            this.chartControl4.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.PL_JieDian);
            this.Controls.Add(this.PL_HuaWu);
            this.Controls.Add(this.PL_YeWu);
            this.MaximumSize = new System.Drawing.Size(1024, 768);
            this.MinimumSize = new System.Drawing.Size(1024, 726);
            this.Name = "Form1";
            this.Text = "話務分時統計";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.PL_HuaWu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.PL_YeWu.ResumeLayout(false);
            this.PL_YeWu.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(simpleDiagram4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(pieSeriesLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(pieSeriesView4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl3)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartControl2)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartControl1)).EndInit();
            this.PL_JieDian.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartControl4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PL_HuaWu;
        private System.Windows.Forms.Panel PL_YeWu;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraCharts.ChartControl chartControl1;
        private DevExpress.XtraCharts.ChartControl chartControl3;
        private DevExpress.XtraCharts.ChartControl chartControl2;
        private System.Windows.Forms.Panel PL_JieDian;
        private DevExpress.XtraCharts.ChartControl chartControl4;
    }
}

