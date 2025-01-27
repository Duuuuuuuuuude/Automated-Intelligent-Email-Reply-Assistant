using System.ComponentModel;
using AIERA.Desktop.WinForms.Authentication.Models.Result_Pattern;
using AIERA.Desktop.WinForms.Models.ViewModels;
using Common.Models;

namespace AIERA.Desktop.WinForms;

partial class SettingsForm
{
    private const string _lblStatusDefaultText = "Status: Not running"; // If updated here, also update the LblStatus.Text label with the same text.

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    //private static System.ComponentModel.ComponentResourceManager resources;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        _settingsFormClosingCancellationTokenSource.Dispose();
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
        panelStatus = new Panel();
        lblStatus = new Label();
        txtSentFolder = new TextBox();
        lblSentFolder = new Label();
        txtReplyFolder = new TextBox();
        btnRemoveAccount = new Button();
        checkBoxAutoReply = new CheckBox();
        lblAutoReply = new Label();
        groupBoxAutomationSettings = new GroupBox();
        groupBoxAccountSettings = new GroupBox();
        groupBoxAuthenticationSettings = new GroupBox();
        btnSignIn = new Button();
        lblAccountErrorMessage = new Label();
        gbFolderSettings = new GroupBox();
        lblReplyFolder = new Label();
        lblAccountsLoadStatus = new Label();
        BtnAddNewAccount = new Button();
        groupBoxAccounts = new GroupBox();
        tableLayoutPanelAccountButtonsAndStatusLabel = new TableLayoutPanel();
        flowLayoutPanelAccountButtons = new FlowLayoutPanel();
        btnMicrosoftAccount_Template = new Button();
        tlpAccountSettings = new TableLayoutPanel();
        splitContainerSettings = new SplitContainer();
        panelStatus.SuspendLayout();
        groupBoxAutomationSettings.SuspendLayout();
        groupBoxAccountSettings.SuspendLayout();
        groupBoxAuthenticationSettings.SuspendLayout();
        gbFolderSettings.SuspendLayout();
        groupBoxAccounts.SuspendLayout();
        tableLayoutPanelAccountButtonsAndStatusLabel.SuspendLayout();
        flowLayoutPanelAccountButtons.SuspendLayout();
        tlpAccountSettings.SuspendLayout();
        ((ISupportInitialize)splitContainerSettings).BeginInit();
        splitContainerSettings.Panel1.SuspendLayout();
        splitContainerSettings.Panel2.SuspendLayout();
        splitContainerSettings.SuspendLayout();
        SuspendLayout();
        // 
        // panelStatus
        // 
        panelStatus.AutoSize = true;
        panelStatus.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        panelStatus.Controls.Add(lblStatus);
        panelStatus.Dock = DockStyle.Bottom;
        panelStatus.Location = new Point(0, 424);
        panelStatus.MaximumSize = new Size(800, 46);
        panelStatus.MinimumSize = new Size(800, 26);
        panelStatus.Name = "panelStatus";
        panelStatus.Size = new Size(800, 26);
        panelStatus.TabIndex = 0;
        // 
        // lblStatus
        // 
        lblStatus.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        lblStatus.AutoSize = true;
        lblStatus.Location = new Point(0, 0);
        lblStatus.Margin = new Padding(3);
        lblStatus.MaximumSize = new Size(800, 40);
        lblStatus.MinimumSize = new Size(800, 20);
        lblStatus.Name = "lblStatus";
        lblStatus.Padding = new Padding(5, 0, 5, 0);
        lblStatus.Size = new Size(800, 20);
        lblStatus.TabIndex = 0;
        lblStatus.Text = "Status: Not running";
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtSentFolder
        // 
        txtSentFolder.Location = new Point(149, 77);
        txtSentFolder.Margin = new Padding(5, 10, 10, 10);
        txtSentFolder.Name = "txtSentFolder";
        txtSentFolder.PlaceholderText = "e.g. AIERA/Replied";
        txtSentFolder.Size = new Size(125, 27);
        txtSentFolder.TabIndex = 3;
        txtSentFolder.Text = "AIERA/Replied";
        // 
        // lblSentFolder
        // 
        lblSentFolder.AutoSize = true;
        lblSentFolder.Location = new Point(13, 80);
        lblSentFolder.Margin = new Padding(10, 10, 5, 10);
        lblSentFolder.Name = "lblSentFolder";
        lblSentFolder.Size = new Size(85, 20);
        lblSentFolder.TabIndex = 2;
        lblSentFolder.Text = "Sent folder:";
        // 
        // txtReplyFolder
        // 
        txtReplyFolder.Location = new Point(149, 30);
        txtReplyFolder.Margin = new Padding(5, 10, 10, 10);
        txtReplyFolder.Name = "txtReplyFolder";
        txtReplyFolder.PlaceholderText = "e.g. AIERA/Reply";
        txtReplyFolder.Size = new Size(125, 27);
        txtReplyFolder.TabIndex = 1;
        txtReplyFolder.Text = "AIERA/Reply";
        // 
        // btnRemoveAccount
        // 
        btnRemoveAccount.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnRemoveAccount.AutoSize = true;
        btnRemoveAccount.BackColor = Color.FromArgb(208, 55, 57);
        btnRemoveAccount.FlatAppearance.BorderSize = 0;
        btnRemoveAccount.FlatAppearance.MouseOverBackColor = Color.FromArgb(201, 75, 77);
        btnRemoveAccount.ForeColor = SystemColors.ButtonHighlight;
        btnRemoveAccount.Location = new Point(283, 134);
        btnRemoveAccount.Name = "btnRemoveAccount";
        btnRemoveAccount.Size = new Size(169, 32);
        btnRemoveAccount.TabIndex = 1;
        btnRemoveAccount.Text = "&Remove this account...";
        btnRemoveAccount.UseVisualStyleBackColor = false;
        btnRemoveAccount.Click += BtnRemoveAccount_Click;
        // 
        // checkBoxAutoReply
        // 
        checkBoxAutoReply.AutoSize = true;
        checkBoxAutoReply.Location = new Point(151, 36);
        checkBoxAutoReply.Margin = new Padding(5, 10, 10, 10);
        checkBoxAutoReply.Name = "checkBoxAutoReply";
        checkBoxAutoReply.Size = new Size(18, 17);
        checkBoxAutoReply.TabIndex = 5;
        checkBoxAutoReply.UseVisualStyleBackColor = true;
        // 
        // lblAutoReply
        // 
        lblAutoReply.AutoSize = true;
        lblAutoReply.Location = new Point(13, 33);
        lblAutoReply.Margin = new Padding(10, 10, 5, 10);
        lblAutoReply.Name = "lblAutoReply";
        lblAutoReply.Size = new Size(128, 20);
        lblAutoReply.TabIndex = 4;
        lblAutoReply.Text = "Auto reply emails:";
        // 
        // groupBoxAutomationSettings
        // 
        groupBoxAutomationSettings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        groupBoxAutomationSettings.Controls.Add(checkBoxAutoReply);
        groupBoxAutomationSettings.Controls.Add(lblAutoReply);
        groupBoxAutomationSettings.Location = new Point(6, 161);
        groupBoxAutomationSettings.Name = "groupBoxAutomationSettings";
        groupBoxAutomationSettings.Size = new Size(464, 85);
        groupBoxAutomationSettings.TabIndex = 2;
        groupBoxAutomationSettings.TabStop = false;
        groupBoxAutomationSettings.Text = "Automation settings";
        // 
        // groupBoxAccountSettings
        // 
        groupBoxAccountSettings.Controls.Add(groupBoxAuthenticationSettings);
        groupBoxAccountSettings.Controls.Add(groupBoxAutomationSettings);
        groupBoxAccountSettings.Controls.Add(gbFolderSettings);
        groupBoxAccountSettings.Dock = DockStyle.Fill;
        groupBoxAccountSettings.Location = new Point(0, 0);
        groupBoxAccountSettings.Name = "groupBoxAccountSettings";
        groupBoxAccountSettings.Size = new Size(476, 424);
        groupBoxAccountSettings.TabIndex = 0;
        groupBoxAccountSettings.TabStop = false;
        groupBoxAccountSettings.Text = "Settings - Example@outlook.dk";
        // 
        // groupBoxAuthenticationSettings
        // 
        groupBoxAuthenticationSettings.Controls.Add(btnSignIn);
        groupBoxAuthenticationSettings.Controls.Add(lblAccountErrorMessage);
        groupBoxAuthenticationSettings.Controls.Add(btnRemoveAccount);
        groupBoxAuthenticationSettings.Location = new Point(6, 252);
        groupBoxAuthenticationSettings.Name = "groupBoxAuthenticationSettings";
        groupBoxAuthenticationSettings.Size = new Size(458, 172);
        groupBoxAuthenticationSettings.TabIndex = 3;
        groupBoxAuthenticationSettings.TabStop = false;
        groupBoxAuthenticationSettings.Text = "Authentication settings";
        // 
        // btnSignIn
        // 
        btnSignIn.Location = new Point(13, 137);
        btnSignIn.Name = "btnSignIn";
        btnSignIn.Size = new Size(94, 29);
        btnSignIn.TabIndex = 3;
        btnSignIn.Text = "&Sign In";
        btnSignIn.UseVisualStyleBackColor = true;
        btnSignIn.Click += BtnSignIn_Click;
        // 
        // lblAccountErrorMessage
        // 
        lblAccountErrorMessage.AutoSize = true;
        lblAccountErrorMessage.Location = new Point(13, 33);
        lblAccountErrorMessage.Margin = new Padding(10, 10, 5, 10);
        lblAccountErrorMessage.Name = "lblAccountErrorMessage";
        lblAccountErrorMessage.Size = new Size(181, 20);
        lblAccountErrorMessage.TabIndex = 2;
        lblAccountErrorMessage.Text = "Errors will be written here.";
        lblAccountErrorMessage.Visible = false;
        // 
        // gbFolderSettings
        // 
        gbFolderSettings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        gbFolderSettings.Controls.Add(txtSentFolder);
        gbFolderSettings.Controls.Add(lblSentFolder);
        gbFolderSettings.Controls.Add(txtReplyFolder);
        gbFolderSettings.Controls.Add(lblReplyFolder);
        gbFolderSettings.Location = new Point(6, 29);
        gbFolderSettings.Name = "gbFolderSettings";
        gbFolderSettings.Size = new Size(464, 126);
        gbFolderSettings.TabIndex = 0;
        gbFolderSettings.TabStop = false;
        gbFolderSettings.Text = "Folder settings";
        // 
        // lblReplyFolder
        // 
        lblReplyFolder.AutoSize = true;
        lblReplyFolder.Location = new Point(13, 33);
        lblReplyFolder.Margin = new Padding(10, 10, 5, 10);
        lblReplyFolder.Name = "lblReplyFolder";
        lblReplyFolder.Size = new Size(126, 20);
        lblReplyFolder.TabIndex = 0;
        lblReplyFolder.Text = "Read from folder:";
        // 
        // lblAccountsLoadStatus
        // 
        lblAccountsLoadStatus.AutoSize = true;
        lblAccountsLoadStatus.Dock = DockStyle.Fill;
        lblAccountsLoadStatus.Location = new Point(3, 163);
        lblAccountsLoadStatus.Name = "lblAccountsLoadStatus";
        lblAccountsLoadStatus.Size = new Size(308, 164);
        lblAccountsLoadStatus.TabIndex = 2;
        lblAccountsLoadStatus.Text = "Accounts load status";
        lblAccountsLoadStatus.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // BtnAddNewAccount
        // 
        BtnAddNewAccount.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        BtnAddNewAccount.Location = new Point(3, 362);
        BtnAddNewAccount.Name = "BtnAddNewAccount";
        BtnAddNewAccount.Size = new Size(314, 59);
        BtnAddNewAccount.TabIndex = 1;
        BtnAddNewAccount.Text = "&Add new account...";
        BtnAddNewAccount.UseVisualStyleBackColor = true;
        BtnAddNewAccount.Click += BtnAddAccount_Click;
        // 
        // groupBoxAccounts
        // 
        groupBoxAccounts.Controls.Add(tableLayoutPanelAccountButtonsAndStatusLabel);
        groupBoxAccounts.Dock = DockStyle.Fill;
        groupBoxAccounts.Location = new Point(3, 3);
        groupBoxAccounts.Name = "groupBoxAccounts";
        groupBoxAccounts.Size = new Size(314, 353);
        groupBoxAccounts.TabIndex = 2;
        groupBoxAccounts.TabStop = false;
        groupBoxAccounts.Text = "Accounts";
        // 
        // tableLayoutPanelAccountButtonsAndStatusLabel
        // 
        tableLayoutPanelAccountButtonsAndStatusLabel.ColumnCount = 1;
        tableLayoutPanelAccountButtonsAndStatusLabel.ColumnStyles.Add(new ColumnStyle());
        tableLayoutPanelAccountButtonsAndStatusLabel.Controls.Add(lblAccountsLoadStatus, 0, 1);
        tableLayoutPanelAccountButtonsAndStatusLabel.Controls.Add(flowLayoutPanelAccountButtons, 0, 0);
        tableLayoutPanelAccountButtonsAndStatusLabel.Dock = DockStyle.Fill;
        tableLayoutPanelAccountButtonsAndStatusLabel.Location = new Point(3, 23);
        tableLayoutPanelAccountButtonsAndStatusLabel.Name = "tableLayoutPanelAccountButtonsAndStatusLabel";
        tableLayoutPanelAccountButtonsAndStatusLabel.RowCount = 2;
        tableLayoutPanelAccountButtonsAndStatusLabel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tableLayoutPanelAccountButtonsAndStatusLabel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tableLayoutPanelAccountButtonsAndStatusLabel.Size = new Size(308, 327);
        tableLayoutPanelAccountButtonsAndStatusLabel.TabIndex = 0;
        // 
        // flowLayoutPanelAccountButtons
        // 
        flowLayoutPanelAccountButtons.AutoSize = true;
        flowLayoutPanelAccountButtons.Controls.Add(btnMicrosoftAccount_Template);
        flowLayoutPanelAccountButtons.Dock = DockStyle.Fill;
        flowLayoutPanelAccountButtons.FlowDirection = FlowDirection.TopDown;
        flowLayoutPanelAccountButtons.Location = new Point(3, 3);
        flowLayoutPanelAccountButtons.Name = "flowLayoutPanelAccountButtons";
        flowLayoutPanelAccountButtons.Size = new Size(308, 157);
        flowLayoutPanelAccountButtons.TabIndex = 1;
        // 
        // btnMicrosoftAccount_Template
        // 
        btnMicrosoftAccount_Template.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        btnMicrosoftAccount_Template.AutoSize = true;
        btnMicrosoftAccount_Template.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        btnMicrosoftAccount_Template.Image = AIERA.Desktop.WinForms.Properties.Resources.ms_symbollockup_mssymbol_19;
        btnMicrosoftAccount_Template.ImageAlign = ContentAlignment.MiddleLeft;
        btnMicrosoftAccount_Template.Location = new Point(3, 3);
        btnMicrosoftAccount_Template.MaximumSize = new Size(302, 80);
        btnMicrosoftAccount_Template.MinimumSize = new Size(302, 60);
        btnMicrosoftAccount_Template.Name = "btnMicrosoftAccount_Template";
        btnMicrosoftAccount_Template.Padding = new Padding(12, 3, 0, 0);
        btnMicrosoftAccount_Template.Size = new Size(302, 60);
        btnMicrosoftAccount_Template.TabIndex = 0;
        btnMicrosoftAccount_Template.Text = "template@example.hotmail.com";
        btnMicrosoftAccount_Template.TextImageRelation = TextImageRelation.ImageBeforeText;
        btnMicrosoftAccount_Template.UseVisualStyleBackColor = true;
        btnMicrosoftAccount_Template.Visible = false;
        // 
        // tlpAccountSettings
        // 
        tlpAccountSettings.ColumnCount = 1;
        tlpAccountSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tlpAccountSettings.Controls.Add(BtnAddNewAccount, 0, 1);
        tlpAccountSettings.Controls.Add(groupBoxAccounts, 0, 0);
        tlpAccountSettings.Dock = DockStyle.Fill;
        tlpAccountSettings.Location = new Point(0, 0);
        tlpAccountSettings.Name = "tlpAccountSettings";
        tlpAccountSettings.RowCount = 1;
        tlpAccountSettings.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tlpAccountSettings.RowStyles.Add(new RowStyle(SizeType.Absolute, 65F));
        tlpAccountSettings.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        tlpAccountSettings.Size = new Size(320, 424);
        tlpAccountSettings.TabIndex = 0;
        // 
        // splitContainerSettings
        // 
        splitContainerSettings.Dock = DockStyle.Fill;
        splitContainerSettings.Location = new Point(0, 0);
        splitContainerSettings.Name = "splitContainerSettings";
        // 
        // splitContainerSettings.Panel1
        // 
        splitContainerSettings.Panel1.Controls.Add(tlpAccountSettings);
        // 
        // splitContainerSettings.Panel2
        // 
        splitContainerSettings.Panel2.Controls.Add(groupBoxAccountSettings);
        splitContainerSettings.Size = new Size(800, 424);
        splitContainerSettings.SplitterDistance = 320;
        splitContainerSettings.TabIndex = 7;
        // 
        // settingsForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(splitContainerSettings);
        Controls.Add(panelStatus);
        MinimumSize = new Size(818, 497);
        Name = "SettingsForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Settings - ";
        FormClosing += SettingsForm_FormClosing;
        Load += SettingsForm_Load;
        panelStatus.ResumeLayout(false);
        panelStatus.PerformLayout();
        groupBoxAutomationSettings.ResumeLayout(false);
        groupBoxAutomationSettings.PerformLayout();
        groupBoxAccountSettings.ResumeLayout(false);
        groupBoxAuthenticationSettings.ResumeLayout(false);
        groupBoxAuthenticationSettings.PerformLayout();
        gbFolderSettings.ResumeLayout(false);
        gbFolderSettings.PerformLayout();
        groupBoxAccounts.ResumeLayout(false);
        tableLayoutPanelAccountButtonsAndStatusLabel.ResumeLayout(false);
        tableLayoutPanelAccountButtonsAndStatusLabel.PerformLayout();
        flowLayoutPanelAccountButtons.ResumeLayout(false);
        flowLayoutPanelAccountButtons.PerformLayout();
        tlpAccountSettings.ResumeLayout(false);
        splitContainerSettings.Panel1.ResumeLayout(false);
        splitContainerSettings.Panel2.ResumeLayout(false);
        ((ISupportInitialize)splitContainerSettings).EndInit();
        splitContainerSettings.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }
    #endregion

#nullable enable
    #region Account Buttons

    /// <summary>
    /// Creates a button from account data.
    /// </summary>
    private Button CreateButtonFromAccountData(Result<MicrosoftAccountViewModel, MicrosoftAuthenticationError> authenticationResult)
    {
        Button btnAccount = new Button();
        btnAccount.ApplyAccountButtonTemplateProperties(btnMicrosoftAccount_Template);

        ApplyAccountDataToButton(btnAccount, authenticationResult);

        return btnAccount;
    }


    private void ApplyAccountDataToButton(Button btnAccount, Result<MicrosoftAccountViewModel, MicrosoftAuthenticationError> authenticationResult)
    {
        MicrosoftAccountViewModel microsoftAccountViewModel = authenticationResult.GetMicrosoftAccountViewModel();

        btnAccount.Visible = true;
        btnAccount.Tag = microsoftAccountViewModel;
        btnAccount.Name = microsoftAccountViewModel.Account.HomeAccountId.ToString(); // Since this is used as a key for the button, changing it would cause problems when updating/deleting the button.
        btnAccount.UseVisualStyleBackColor = microsoftAccountViewModel is null ? false : true;

        ToolTip toolTip = new() { ShowAlways = true };

        if (authenticationResult.IsSuccess)
        {
            btnAccount.Text = microsoftAccountViewModel!.LoginHint;
            toolTip.SetToolTip(btnAccount, microsoftAccountViewModel.LoginHint);
        }
        else // Apply error state to account button.
        {
            btnAccount.UseVisualStyleBackColor = false;
            btnAccount.Text = $"{microsoftAccountViewModel!.Account!.Username}\n(Failed to load account.)";
            toolTip.SetToolTip(btnAccount, $"{microsoftAccountViewModel.LoginHint}\nError message: {authenticationResult.Error!.Message}");
        }

        btnRemoveAccount.Tag = microsoftAccountViewModel.Account; // TODO: Button.Tag skal ikke sættes her.
        btnSignIn.Tag = microsoftAccountViewModel.Account; // TODO: Button.Tag skal ikke sættes her.
    }


    /// <summary>
    /// Updates the layout panel with account buttons.
    /// </summary>
    /// <param name="buttons">Buttons to add to the UI.</param>
    private void AddButtonsToUI(params Button[] buttons)
    {
        flowLayoutPanelAccountButtons.SuspendLayout();

        if (!buttons.Any())
        {
            flowLayoutPanelAccountButtons.Controls.Clear(); // In case this is the second time the method is called, because of a refresh.
        }
        else
        {
            // Adjust row styles to give all space to account buttons.
            tableLayoutPanelAccountButtonsAndStatusLabel.RowStyles[0] = new RowStyle(SizeType.Percent, 100); // Full height for buttons
            tableLayoutPanelAccountButtonsAndStatusLabel.RowStyles[1] = new RowStyle(SizeType.Absolute, 0); // Hide status label}
            flowLayoutPanelAccountButtons.Controls.AddRange(buttons);
        }

        flowLayoutPanelAccountButtons.ResumeLayout();
        flowLayoutPanelAccountButtons.PerformLayout();
    }

    private void AddButtonToUI(Button button) => AddButtonsToUI(button);

    /// <summary>
    /// Updates already existing account button or creates a new account button and inserts it in the list/panel of accounts.
    /// </summary>
    /// <param name="authenticationResult"><see cref="Result"/> that contains the account data that will be added to the button.</param>
    public void UpsertAccountButton(Result<MicrosoftAccountViewModel, MicrosoftAuthenticationError> authenticationResult)
    {
        MicrosoftAccountViewModel microsoftAccountViewModel = authenticationResult.GetMicrosoftAccountViewModel();

        string accountId = microsoftAccountViewModel.Account.HomeAccountId.ToString();
        bool buttonExists = flowLayoutPanelAccountButtons.Controls.ContainsKey(accountId);

        if (buttonExists)
        {
            // Update existing button with new data
            Button existingButton = (Button)flowLayoutPanelAccountButtons.Controls[accountId]!;
            ApplyAccountDataToButton(existingButton, microsoftAccountViewModel);
        }
        else
        {
            // Create and add a new button
            Button newButton = CreateButtonFromAccountData(microsoftAccountViewModel);
            AddButtonToUI(newButton);
        }
    }
    #endregion
#nullable disable

    private Panel panelStatus;
    private TextBox txtSentFolder;
    private Label lblSentFolder;
    private TextBox txtReplyFolder;
    private Button btnRemoveAccount;
    private CheckBox checkBoxAutoReply;
    private Label lblAutoReply;
    private GroupBox groupBoxAutomationSettings;
    private GroupBox groupBoxAccountSettings;
    private GroupBox gbFolderSettings;
    private Label lblReplyFolder;
    private Label lblAccountsLoadStatus;
    private Button BtnAddNewAccount;
    private GroupBox groupBoxAccounts;
    private TableLayoutPanel tlpAccountSettings;
    private SplitContainer splitContainerSettings;
    private Label lblStatus;
    private GroupBox groupBoxAuthenticationSettings;
    private Label lblAccountErrorMessage;
    private Button btnSignIn;
    private TableLayoutPanel tableLayoutPanelAccountButtonsAndStatusLabel;
    private FlowLayoutPanel flowLayoutPanelAccountButtons;
    private Button btnMicrosoftAccount_Template;
}

