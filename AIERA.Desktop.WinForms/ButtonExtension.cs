namespace AIERA.Desktop.WinForms;


public static class ButtonExtension
{
    /// <summary>
    /// Performs a shallow property clone from the provided template button.
    /// </summary>
    /// <remarks>
    /// Not all the properties are possible to copy without causing problems, but most are. 
    /// These properties are: 'BackColor' and ImageKey.
    /// </remarks>
    /// <param name="newBtn"></param>
    /// <param name="templateButton">Button that should be used as a template. Almost all of the properties will be copied</param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0051:Method is too long", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
    public static Button ApplyAccountButtonTemplateProperties(this Button newBtn, Button templateButton)
    {
        #region From properties in designer tab
        newBtn.AccessibleDescription = templateButton.AccessibleDescription;
        newBtn.AccessibleName = templateButton.AccessibleName;
        newBtn.AccessibleRole = templateButton.AccessibleRole;
        //BtnAccount.BackColor = btnMicrosoftAccount_Template.BackColor; // Causes problems
        newBtn.BackgroundImage = templateButton.BackgroundImage;
        newBtn.BackgroundImageLayout = templateButton.BackgroundImageLayout;
        newBtn.Cursor = templateButton.Cursor;
        newBtn.FlatAppearance.BorderColor = templateButton.FlatAppearance.BorderColor;
        newBtn.FlatAppearance.BorderSize = templateButton.FlatAppearance.BorderSize;
        newBtn.FlatAppearance.MouseDownBackColor = templateButton.FlatAppearance.MouseDownBackColor;
        newBtn.FlatAppearance.MouseOverBackColor = templateButton.FlatAppearance.MouseOverBackColor;
        newBtn.FlatStyle = templateButton.FlatStyle;
        newBtn.Font = templateButton.Font;
        newBtn.ForeColor = templateButton.ForeColor;
        newBtn.Image = templateButton.Image;
        if (newBtn.Image is not null)
            newBtn.Image.Tag = templateButton.Image?.Tag;
        newBtn.ImageAlign = templateButton.ImageAlign;
        newBtn.ImageIndex = templateButton.ImageIndex;
        //BtnAccount.ImageKey = btnAccountTemplate.ImageKey; // Causes problems.
        newBtn.ImageList = templateButton.ImageList;
        newBtn.RightToLeft = templateButton.RightToLeft;
        newBtn.Text = templateButton.Text;
        newBtn.TextAlign = templateButton.TextAlign;
        newBtn.TextImageRelation = templateButton.TextImageRelation;
        newBtn.UseMnemonic = templateButton.UseMnemonic;
        newBtn.UseVisualStyleBackColor = templateButton.UseVisualStyleBackColor;
        newBtn.UseWaitCursor = templateButton.UseWaitCursor;
        newBtn.AllowDrop = templateButton.UseWaitCursor;
        newBtn.AutoEllipsis = templateButton.UseWaitCursor;
        newBtn.ContextMenuStrip = templateButton.ContextMenuStrip;
        newBtn.DialogResult = templateButton.DialogResult;
        newBtn.Enabled = templateButton.Enabled;
        newBtn.TabStop = templateButton.TabStop;
        newBtn.UseCompatibleTextRendering = templateButton.UseCompatibleTextRendering;
        newBtn.Visible = templateButton.Visible;
        newBtn.Tag = templateButton.Tag;
        newBtn.Name = templateButton.Name;
        newBtn.CausesValidation = templateButton.CausesValidation;
        newBtn.Anchor = templateButton.Anchor;
        newBtn.AutoSize = templateButton.AutoSize;
        newBtn.AutoSizeMode = templateButton.AutoSizeMode;
        newBtn.Dock = templateButton.Dock;
        newBtn.Location = templateButton.Location;
        newBtn.Margin = templateButton.Margin;
        newBtn.MaximumSize = templateButton.MaximumSize;
        newBtn.MinimumSize = templateButton.MinimumSize;
        newBtn.Padding = templateButton.Padding;
        newBtn.Size = templateButton.Size;
        #endregion
        #region Others
        newBtn.AccessibleDefaultActionDescription = templateButton.AccessibleDefaultActionDescription;
        newBtn.AutoScrollOffset = templateButton.AutoScrollOffset;
        newBtn.BindingContext = templateButton.BindingContext;
        newBtn.Bounds = templateButton.Bounds;
        newBtn.Capture = templateButton.Capture;
        newBtn.ClientSize = templateButton.ClientSize;
        newBtn.Command = templateButton.Command;
        newBtn.CommandParameter = templateButton.CommandParameter;
        newBtn.DataContext = templateButton.DataContext;
        newBtn.IsAccessible = templateButton.IsAccessible;
        newBtn.Left = templateButton.Left;
        newBtn.Parent = templateButton.Parent;
        newBtn.Region = templateButton.Region;
        newBtn.Site = templateButton.Site;
        newBtn.Top = templateButton.Top;
        newBtn.Width = templateButton.Width;
        #endregion
        return newBtn;
    }
}