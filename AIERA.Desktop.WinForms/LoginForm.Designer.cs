namespace AIERA.Desktop.WinForms;

partial class LoginForm
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
        _loginFormClosingCancellationTokenSource.Dispose();
        if (disposing && (components is not null))
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
        tlpLogin = new TableLayoutPanel();
        btnLoginMicrosoft = new Button();
        tableLayoutPanel1 = new TableLayoutPanel();
        btnCancel = new Button();
        tlpLogin.SuspendLayout();
        tableLayoutPanel1.SuspendLayout();
        SuspendLayout();
        // 
        // tlpLogin
        // 
        tlpLogin.ColumnCount = 3;
        tlpLogin.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 49.9999962F));
        tlpLogin.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 222F));
        tlpLogin.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0000076F));
        tlpLogin.Controls.Add(btnLoginMicrosoft, 1, 1);
        tlpLogin.Controls.Add(tableLayoutPanel1, 1, 3);
        tlpLogin.Dock = DockStyle.Fill;
        tlpLogin.Location = new Point(0, 0);
        tlpLogin.Name = "tlpLogin";
        tlpLogin.RowCount = 4;
        tlpLogin.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
        tlpLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 47F));
        tlpLogin.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
        tlpLogin.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
        tlpLogin.Size = new Size(432, 323);
        tlpLogin.TabIndex = 0;
        // 
        // btnLoginMicrosoft
        // 
        btnLoginMicrosoft.Anchor = AnchorStyles.None;
        btnLoginMicrosoft.BackgroundImageLayout = ImageLayout.Center;
        btnLoginMicrosoft.Image = Properties.Resources.ms_symbollockup_signin_dark;
        btnLoginMicrosoft.Location = new Point(107, 95);
        btnLoginMicrosoft.Name = "btnLoginMicrosoft";
        btnLoginMicrosoft.Size = new Size(215, 41);
        btnLoginMicrosoft.TabIndex = 0;
        btnLoginMicrosoft.UseVisualStyleBackColor = true;
        btnLoginMicrosoft.Click += BtnLoginMicrosoft_Click;
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 3;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tableLayoutPanel1.Controls.Add(btnCancel, 1, 1);
        tableLayoutPanel1.Location = new Point(107, 234);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 3;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 49.9999962F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0000076F));
        tableLayoutPanel1.Size = new Size(216, 80);
        tableLayoutPanel1.TabIndex = 1;
        // 
        // btnCancel
        // 
        btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(71, 25);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(74, 29);
        btnCancel.TabIndex = 0;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        // 
        // LoginForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(432, 323);
        Controls.Add(tlpLogin);
        MaximizeBox = false;
        MaximumSize = new Size(450, 370);
        MinimizeBox = false;
        MinimumSize = new Size(450, 370);
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Login - ";
        FormClosing += LoginForm_FormClosing;
        Load += LoginForm_Load;
        tlpLogin.ResumeLayout(false);
        tableLayoutPanel1.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel tlpLogin;
    private Button btnLoginMicrosoft;
    private TableLayoutPanel tableLayoutPanel1;
    private Button btnCancel;
}