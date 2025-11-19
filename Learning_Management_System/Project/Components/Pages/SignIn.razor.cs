namespace D424_TL.Components.Pages
{
  public partial class SignIn
  {
    string usernameInput = "";
    string passwordInput = "";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      await base.OnAfterRenderAsync(firstRender);
      if (firstRender) 
      {
        if (Global.CurrUser != null) { Global.CurrUser = null; }

        Global.MainContentClass = "signInBackground";
        Global.DrawerOpen = false;
        Global.DrawerClass = "invisible";
        Global.AppBarClass = "invisible";
        Global.MainContentWidth = MaxWidth.Small;
        StateHasChanged();
        var result = await DialogService.ShowAsync<WelcomeDialog>("Welcome");
      }
    }
    async Task SignInClicked()
    {
      string usernamePattern = @"^[A-Za-z0-9!@#$*&]*$";

      if (!Regex.IsMatch(usernameInput, usernamePattern) || string.IsNullOrWhiteSpace(passwordInput))
      {
        Snackbar.Add("Please enter valid credentials", Severity.Warning);
        return;
      }

      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();

        if (Authenticate().Result) { nav.NavigateTo("redirect"); }
      }
      catch (Exception ex) 
      {
        Snackbar.Add("Something went wrong. Please contact your site admin.", Severity.Error);
        await Global.Log(D424DataContextFactory, "ERROR", $"SignInClicked: {ex.Message}");
      }
    }
    async Task<bool> Authenticate()
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        User? user = context?.User.Where(u => u.Username == usernameInput).FirstOrDefault();

        if (user == null)
        {
          Snackbar.Add("Please enter valid credentials", Severity.Warning);
          return false;
        }
        else if (!BCrypt.Net.BCrypt.Verify(passwordInput.Trim(), user.Password))
        {
          await HandleFailedAttempt(user);
          return false;
        }
        else if (user.Locked == "Yes")
        {
          Snackbar.Add("Your account is locked. Please contact your site admin", Severity.Error);
          await Global.Log(D424DataContextFactory, "WARNING", "Attempted sign in to locked account.", user);
          return false;
        }
        else
        {
          Global.CurrUser = user;
          await ResetFailedAttempts(user);
          await Global.Log(D424DataContextFactory, "INFO", "Sign in");
          return true;
        }
      }
      catch (Exception ex) 
      {
        Snackbar.Add("Something went wrong. Please contact your site admin.", Severity.Error);
        await Global.Log(D424DataContextFactory, "ERROR", $"Authenticate: {ex.Message}");
        return false;
      }
    }
    async Task HandleFailedAttempt(User user)
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        context?.User.Attach(user);
        user.FailedAttempts++;
        context.Entry(user).Property(u => u.FailedAttempts).IsModified = true;

        if (user.FailedAttempts > 2 && (user.Id != 1 && user.Id != 2 && user.Id != 3))
        {
          user.Locked = "Yes";
          context.Entry(user).Property(user => user.Locked).IsModified = true;
          Snackbar.Add("Your account has been locked", Severity.Error);
          await Global.Log(D424DataContextFactory, "WARNING", "Account has been locked", user);
        }
        else
        {
          if (user.FailedAttempts > 2 && (user.Id == 1 || user.Id == 2 || user.Id == 3))
          {
            Snackbar.Add("This account would normally be locked due to excessive failed sign in attempts. Being a test account, you receive this message, and sign in attempts have been reset. ", Severity.Error);
            user.FailedAttempts = 0;
            context.Update(user);
            await context.SaveChangesAsync();
          }

          if (user.FailedAttempts == 2) 
          {
            Snackbar.Add($"You have 1 attempt left", Severity.Warning);
          }
          else
          {
            Snackbar.Add($"You have {3 - user.FailedAttempts} attempts left", Severity.Warning);
          }
          await Global.Log(D424DataContextFactory, "WARNING", "Failed sign in attempt.", user);
        }

        await context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Snackbar.Add("Something went wrong. Please contact your site admin.", Severity.Error);
        await Global.Log(D424DataContextFactory, "ERROR", $"HandleFailedSignIn: {ex.Message}");
      }
    }
    async Task ResetFailedAttempts(User user)
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        context?.User.Attach(user);
        user.FailedAttempts = 0;
        context.Entry(user).Property(u => u.FailedAttempts).IsModified = true;
        await context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Snackbar.Add("Something went wrong. Please contact your site admin.", Severity.Error);
        await Global.Log(D424DataContextFactory, "ERROR", $"ResetFailedAttempts: {ex.Message}");
      }
    }
  }
}
