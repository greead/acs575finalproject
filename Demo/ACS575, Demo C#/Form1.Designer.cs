namespace ACS575__Demo_C_ {
    partial class Form1 {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            label1 = new Label();
            txtEmail = new TextBox();
            lstCharacters = new ListBox();
            lstInventory = new ListBox();
            lstItems = new ListBox();
            btnAddInventory = new Button();
            btnRemInventory = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            btnSearch = new Button();
            btnSaveInv = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(36, 9);
            label1.Name = "label1";
            label1.Size = new Size(36, 15);
            label1.TabIndex = 0;
            label1.Text = "Email";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(78, 6);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(100, 23);
            txtEmail.TabIndex = 1;
            // 
            // lstCharacters
            // 
            lstCharacters.FormattingEnabled = true;
            lstCharacters.ItemHeight = 15;
            lstCharacters.Location = new Point(30, 86);
            lstCharacters.Name = "lstCharacters";
            lstCharacters.Size = new Size(151, 259);
            lstCharacters.TabIndex = 2;
            lstCharacters.SelectedIndexChanged += lstCharacters_SelectedIndexChanged;
            // 
            // lstInventory
            // 
            lstInventory.FormattingEnabled = true;
            lstInventory.ItemHeight = 15;
            lstInventory.Location = new Point(281, 86);
            lstInventory.Name = "lstInventory";
            lstInventory.SelectionMode = SelectionMode.MultiExtended;
            lstInventory.Size = new Size(151, 259);
            lstInventory.TabIndex = 3;
            // 
            // lstItems
            // 
            lstItems.FormattingEnabled = true;
            lstItems.ItemHeight = 15;
            lstItems.Location = new Point(497, 86);
            lstItems.Name = "lstItems";
            lstItems.SelectionMode = SelectionMode.MultiExtended;
            lstItems.Size = new Size(151, 259);
            lstItems.TabIndex = 4;
            // 
            // btnAddInventory
            // 
            btnAddInventory.Location = new Point(440, 208);
            btnAddInventory.Name = "btnAddInventory";
            btnAddInventory.Size = new Size(51, 23);
            btnAddInventory.TabIndex = 5;
            btnAddInventory.Text = "< ADD";
            btnAddInventory.UseVisualStyleBackColor = true;
            btnAddInventory.Click += btnAddInventory_Click;
            // 
            // btnRemInventory
            // 
            btnRemInventory.Location = new Point(440, 237);
            btnRemInventory.Name = "btnRemInventory";
            btnRemInventory.Size = new Size(51, 23);
            btnRemInventory.TabIndex = 6;
            btnRemInventory.Text = "REM >";
            btnRemInventory.UseVisualStyleBackColor = true;
            btnRemInventory.Click += btnRemInventory_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(325, 68);
            label2.Name = "label2";
            label2.Size = new Size(57, 15);
            label2.TabIndex = 7;
            label2.Text = "Inventory";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(544, 68);
            label3.Name = "label3";
            label3.Size = new Size(57, 15);
            label3.TabIndex = 8;
            label3.Text = "Items List";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(69, 68);
            label4.Name = "label4";
            label4.Size = new Size(63, 15);
            label4.TabIndex = 9;
            label4.Text = "Characters";
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(184, 6);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(75, 23);
            btnSearch.TabIndex = 10;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // btnSaveInv
            // 
            btnSaveInv.Location = new Point(440, 292);
            btnSaveInv.Name = "btnSaveInv";
            btnSaveInv.Size = new Size(51, 23);
            btnSaveInv.TabIndex = 11;
            btnSaveInv.Text = "Save";
            btnSaveInv.UseVisualStyleBackColor = true;
            btnSaveInv.Click += btnSaveInv_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(661, 357);
            Controls.Add(btnSaveInv);
            Controls.Add(btnSearch);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(btnRemInventory);
            Controls.Add(btnAddInventory);
            Controls.Add(lstItems);
            Controls.Add(lstInventory);
            Controls.Add(lstCharacters);
            Controls.Add(txtEmail);
            Controls.Add(label1);
            Name = "Form1";
            Text = "ACS575 Demo";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtEmail;
        private ListBox lstCharacters;
        private ListBox lstInventory;
        private ListBox lstItems;
        private Button btnAddInventory;
        private Button btnRemInventory;
        private Label label2;
        private Label label3;
        private Label label4;
        private Button btnSearch;
        private Button btnSaveInv;
    }
}
