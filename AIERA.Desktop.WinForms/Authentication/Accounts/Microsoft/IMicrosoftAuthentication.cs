﻿using AIERA.Desktop.WinForms.Authentication.Models.Result_Pattern;
using AIERA.Desktop.WinForms.Models.ViewModels;
using Microsoft.Identity.Client;

namespace AIERA.Desktop.WinForms.Authentication.Accounts.Microsoft;

public interface IMicrosoftAuthentication
{
    Task<Result<MicrosoftAccountViewModel>> SignInAsync(nint? hWnd, IAccount? account, string? claims, CancellationToken cancellationoken = default);
    Task<Result<MicrosoftAccountViewModel>> SignInAsync(nint? hWnd, string? loginHint, string? identifier, string? claims, CancellationToken cancellationoken = default);
    Task SignOutAsync(AuthenticationResult authResult);
    Task SignOutAsync(IAccount account);
}