namespace plex_playlist_manager_4
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnSelectPlaylist;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnToTop;
        private System.Windows.Forms.Button btnToBottom;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ListBox listBoxPlaylists;
        private System.Windows.Forms.DataGridView dataGridViewItems;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            btnSelectPlaylist = new Button();
            btnMoveUp = new Button();
            btnMoveDown = new Button();
            btnToTop = new Button();
            btnToBottom = new Button();
            btnDelete = new Button();
            listBoxPlaylists = new ListBox();
            dataGridViewItems = new DataGridView();
            SuspendLayout();
            // 
            // btnSelectPlaylist
            // 
            btnSelectPlaylist.Location = new Point(12, 247);
            btnSelectPlaylist.Name = "btnSelectPlaylist";
            btnSelectPlaylist.Size = new Size(197, 23);
            btnSelectPlaylist.TabIndex = 0;
            btnSelectPlaylist.Text = "Select Playlist";
            btnSelectPlaylist.UseVisualStyleBackColor = true;
            btnSelectPlaylist.Click += btnSelectPlaylist_Click_1;
            // 
            // btnMoveUp
            // 
            btnMoveUp.Location = new Point(12, 276);
            btnMoveUp.Name = "btnMoveUp";
            btnMoveUp.Size = new Size(85, 23);
            btnMoveUp.TabIndex = 1;
            btnMoveUp.Text = "Move Up";
            btnMoveUp.UseVisualStyleBackColor = true;
            btnMoveUp.Click += btnMoveUp_Click_1;
            // 
            // btnMoveDown
            // 
            btnMoveDown.Location = new Point(12, 305);
            btnMoveDown.Name = "btnMoveDown";
            btnMoveDown.Size = new Size(85, 23);
            btnMoveDown.TabIndex = 2;
            btnMoveDown.Text = "Move Down";
            btnMoveDown.UseVisualStyleBackColor = true;
            btnMoveDown.Click += btnMoveDown_Click_1;
            // 
            // btnToTop
            // 
            btnToTop.Location = new Point(122, 276);
            btnToTop.Name = "btnToTop";
            btnToTop.Size = new Size(85, 23);
            btnToTop.TabIndex = 3;
            btnToTop.Text = "To Top";
            btnToTop.UseVisualStyleBackColor = true;
            btnToTop.Click += btnToTop_Click_1;
            // 
            // btnToBottom
            // 
            btnToBottom.Location = new Point(122, 305);
            btnToBottom.Name = "btnToBottom";
            btnToBottom.Size = new Size(85, 23);
            btnToBottom.TabIndex = 4;
            btnToBottom.Text = "To Bottom";
            btnToBottom.UseVisualStyleBackColor = true;
            btnToBottom.Click += btnToBottom_Click_1;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(122, 334);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(85, 23);
            btnDelete.TabIndex = 5;
            btnDelete.Text = "Remove";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click_1;
            // 
            // listBoxPlaylists
            // 
            listBoxPlaylists.FormattingEnabled = true;
            listBoxPlaylists.ItemHeight = 15;
            listBoxPlaylists.Location = new Point(12, 12);
            listBoxPlaylists.Name = "listBoxPlaylists";
            listBoxPlaylists.Size = new Size(197, 229);
            listBoxPlaylists.TabIndex = 6;
            // 
            // dataGridViewItems
            // 
            dataGridViewItems.AllowUserToAddRows = false;
            dataGridViewItems.AllowUserToDeleteRows = false;
            dataGridViewItems.AllowUserToResizeRows = false;
            dataGridViewItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewItems.Location = new Point(226, 12);
            dataGridViewItems.Name = "dataGridViewItems";
            dataGridViewItems.ReadOnly = true;
            dataGridViewItems.RowHeadersVisible = false;
            dataGridViewItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewItems.Size = new Size(345, 574);
            dataGridViewItems.TabIndex = 7;
            dataGridViewItems.MultiSelect = false;
            dataGridViewItems.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "Title", HeaderText = "Title", DataPropertyName = "Title", Width = 180 },
                new DataGridViewTextBoxColumn { Name = "Year", HeaderText = "Year", DataPropertyName = "Year", Width = 50 },
                new DataGridViewTextBoxColumn { Name = "Library", HeaderText = "Library", DataPropertyName = "Library", Width = 90 }
            );
            dataGridViewItems.CellDoubleClick += dataGridViewItems_CellDoubleClick;
            dataGridViewItems.SelectionChanged += dataGridViewItems_SelectionChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(583, 592);
            Controls.Add(dataGridViewItems);
            Controls.Add(listBoxPlaylists);
            Controls.Add(btnDelete);
            Controls.Add(btnToBottom);
            Controls.Add(btnToTop);
            Controls.Add(btnMoveDown);
            Controls.Add(btnMoveUp);
            Controls.Add(btnSelectPlaylist);
            Name = "Form1";
            Text = "Plex Playlist Manager";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}
