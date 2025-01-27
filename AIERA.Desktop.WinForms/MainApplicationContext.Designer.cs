using AIERA.Desktop.WinForms;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Windows.Forms;

namespace AIERA.Desktop.WinForms;
public class CustomToolStripRenderer : ToolStripProfessionalRenderer
{
    protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
    {
        // Fill the image margin area with a custom color
        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(32, 32, 32)), e.AffectedBounds);
    }
}


partial class MainApplicationContext : ApplicationContext
{
    private static readonly string _notifyIconDefaultText = Program.ApplicationNameFull;
    private const string _MenuItemReplyAllDefaultText = "Have AI reply to all";

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
        _appExitCancellationTokenSource.Dispose();

        if (_replyAllCancellationTokenSource is not null)
          _replyAllCancellationTokenSource.Dispose(); 
        
        if (disposing && (components is not null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Initialize Components
    private  void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainApplicationContext));
        notifyIcon = new NotifyIcon(components);
        contextMenuStripSystemTray = new ContextMenuStrip(components);
        menuItemSettings = new ToolStripMenuItem();
        menuItemReplyAll = new ToolStripMenuItem();
        menuItemCancelReplyAll = new ToolStripMenuItem();
        menuItemAddAccount = new ToolStripMenuItem();
        toolStripSeparator1 = new ToolStripSeparator();
        toolStripSeparator2 = new ToolStripSeparator();
        menuItemExit = new ToolStripMenuItem();
        contextMenuStripSystemTray.SuspendLayout();
        // 
        // notifyIcon
        // 
        notifyIcon.ContextMenuStrip = contextMenuStripSystemTray;
        notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        notifyIcon.Text = _notifyIconDefaultText;
        notifyIcon.Visible = true;
        notifyIcon.MouseClick += NotifyIcon_MouseClick;
        // 
        // cMSSystemTray
        // 
        contextMenuStripSystemTray.BackColor = Color.FromArgb(32, 32, 32);
        contextMenuStripSystemTray.ImageScalingSize = new Size(20, 20);
        contextMenuStripSystemTray.Items.AddRange(new ToolStripItem[] { menuItemReplyAll, menuItemCancelReplyAll, toolStripSeparator1, menuItemAddAccount, menuItemSettings, toolStripSeparator2, menuItemExit });
        contextMenuStripSystemTray.Name = "CMSSystemTray";
        contextMenuStripSystemTray.Size = new Size(180, 117);
        contextMenuStripSystemTray.Renderer = new CustomToolStripRenderer();
        // 
        // menuItemSettings
        // 
        menuItemSettings.BackColor = Color.FromArgb(32, 32, 32);
        menuItemSettings.ForeColor = Color.White;
        menuItemSettings.Image = AIERA.Desktop.WinForms.Properties.Resources._961913_gear_cog_settings_cog_gear_settings;
        menuItemSettings.Name = "MenuItemSettings";
        menuItemSettings.Size = new Size(179, 26);
        menuItemSettings.Text = "Settings";
        menuItemSettings.ToolTipText = "Open Settings";
        menuItemSettings.Click += SettingsItem_MouseClick;
        // 
        // menuItemReplyAll
        // 
        menuItemReplyAll.BackColor = Color.FromArgb(32, 32, 32);
        menuItemReplyAll.ForeColor = Color.White;
        menuItemReplyAll.Image = AIERA.Desktop.WinForms.Properties.Resources._8672759_ic_fluent_arrow_reply_all_icon;
        menuItemReplyAll.Name = _MenuItemReplyAllDefaultText;
        menuItemReplyAll.Size = new Size(179, 26);
        menuItemReplyAll.Text = "Have AI reply to all";
        menuItemReplyAll.Click += ReplyToAllItem_Mouse;

        // 
        // cancelReplyAllItem
        // 
        menuItemCancelReplyAll.BackColor = Color.FromArgb(32, 32, 32);
        menuItemCancelReplyAll.ForeColor = Color.White;
        menuItemCancelReplyAll.Image = AIERA.Desktop.WinForms.Properties.Resources._675491_cancel_delete_remove_close_stop_icon;
        menuItemCancelReplyAll.Name = _MenuItemReplyAllDefaultText;
        menuItemCancelReplyAll.Size = new Size(179, 26);
        menuItemCancelReplyAll.Text = "Cancel";
        menuItemCancelReplyAll.Visible = false;
        menuItemCancelReplyAll.ToolTipText = "Cancel automatic AI reply";
        menuItemCancelReplyAll.Click += CancelReplyAllItem_Mouse;
        // 
        // menuItemAddAccount
        // 
        menuItemAddAccount.ForeColor = Color.White;
        menuItemAddAccount.Image = AIERA.Desktop.WinForms.Properties.Resources._4115234_login_sign_in_icon;
        menuItemAddAccount.Name = "MenuItemAddAccount";
        menuItemAddAccount.Size = new Size(179, 26);
        menuItemAddAccount.Text = "Add Accocunt";
        menuItemAddAccount.ToolTipText = "Add Account";
        menuItemAddAccount.Click += AddAccountItem_Mouse;
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(176, 6);
        // 
        // toolStripSeparator2
        // 
        toolStripSeparator2.Name = "toolStripSeparator2";
        toolStripSeparator2.Size = new Size(176, 6);
        // 
        // menuItemExit
        // 
        menuItemExit.ForeColor = Color.White;
        menuItemExit.Name = "MenuItemExit";
        menuItemExit.Size = new Size(179, 26);
        menuItemExit.Text = "Exit";
        menuItemExit.ToolTipText = "Exit " + Program.ApplicationNameAbbreviation;
        menuItemExit.Click += ExitMenuItem_MouseClick;

        //
        //
        //
        contextMenuStripSystemTray.ResumeLayout(false);
        contextMenuStripSystemTray.PerformLayout();
    }
    #endregion

    private static NotifyIcon notifyIcon;
    private ContextMenuStrip contextMenuStripSystemTray;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripMenuItem menuItemSettings;
    private ToolStripMenuItem menuItemExit;
    private static ToolStripMenuItem menuItemReplyAll;
    private ToolStripMenuItem menuItemCancelReplyAll;
    private ToolStripMenuItem menuItemAddAccount;
}