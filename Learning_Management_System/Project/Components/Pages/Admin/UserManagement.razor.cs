namespace D424_TL.Components.Pages.Admin
{
  public partial class UserManagement
  {
    User? selectedUser;
    string? passwordHash;
    string? searchInput;
    bool passwordValid;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      await base.OnAfterRenderAsync(firstRender);
      if (firstRender)
      {
        if (Global.CurrUser == null || Global.CurrUser.Role != "Admin") 
        {
          Global.MainContentClass = "signInBackground";
          Global.DrawerOpen = false;
          Global.DrawerClass = "invisible";
          Global.AppBarClass = "invisible";
          Global.MainContentWidth = MaxWidth.Small;
          nav.NavigateTo("", true);
        }
        else
        {
          await RefreshAllUsers();
        }
      }
    }
    async void OnAddUserClicked()
    {
      var dialog = DialogService.Show<NewUserDialog>("Add User");
      var result = await dialog.Result;
      await RefreshAllUsers();
    }
    async Task UpdateUser(User user)
    {
      try
      {
        if (user.Id == 1 || user.Id == 2 || user.Id == 3) 
        {
          Snackbar.Add("You are not permitted to make any changes to the three test users.", Severity.Error);
          return;
        }
        if (passwordValid == false) { return; }

        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();

        if (user.Password != passwordHash)
        {
          user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        }

        user.LastUpdate = DateTime.Now;
        context.User.Update(user);
        await context.SaveChangesAsync();

        selectedUser = null;
        passwordHash = null;
      }
      catch (Exception ex) 
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"UpdateUser: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
      }
    }
    async Task DeleteUser(User user) 
    {
      try
      {
        bool? result = await DialogService.ShowMessageBox("Warning", "Are you sure you want to delete this user and all associated data?", cancelText: "No", yesText: "Yes");
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();

        if (result != null && (bool)result && (user.Id != 1 || user.Id != 2 || user.Id != 3))
        {
          var scheduleEntries = await context.ScheduleEntry.Where(s => s.UserId == user.Id).ToListAsync();
          foreach (var scheduleEntry in scheduleEntries) 
          {
            var attendanceEntries = await context.Attendance.Where(a => a.ScheduleEntryId == scheduleEntry.Id).ToListAsync();
            foreach (var attendanceEntry in attendanceEntries)
            {
              context.Attendance.Remove(attendanceEntry);
            }

            context.ScheduleEntry.Remove(scheduleEntry);
          }

          var grades = await context.Grade.Where(g => g.StudentId == user.Id).ToListAsync();
          foreach (var grade in grades)
          {
            context.Grade.Remove(grade);
          }

          var messages = await context.Message.Where(m => m.SenderId == user.Id || m.RecipientId == user.Id).ToListAsync();
          foreach (var message in messages)
          {
            context.Message.Remove(message);
          }

          var announcements = await context.Announcement.Where(a => a.UserId == user.Id).ToListAsync();
          foreach (var announcement in announcements)
          {
            context.Announcement.Remove(announcement);
          }

          context.User.Remove(user);
          await context.SaveChangesAsync();

          Global.AllUsers.Remove(user);
          StateHasChanged();
        }
        else
        {
          Snackbar.Add("You are not allowed to delete any of the three test users.", Severity.Error);
        }
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"DeleteUser: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
      }
    }
    void StartedEditingItem(User user) 
    {
      selectedUser = user;
      passwordHash = user.Password;
    }
    void CanceledEditingItem()
    {
      selectedUser = null;
      passwordHash = null;
    }
    async Task RefreshAllUsers()
    {
      await Global.GetFlaggedMessages(D424DataContextFactory);
      await Global.GetAllUsers(D424DataContextFactory);
      StateHasChanged();
    }
    Func<User, bool> Search => x =>
    {
      if (string.IsNullOrWhiteSpace(searchInput))
      {
        return true;
      }
      if (x.FirstName.Contains(searchInput.Trim(), StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }
      if (x.LastName.Contains(searchInput.Trim(), StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }
      if (x.Username.Contains(searchInput.Trim(), StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }
      return false;
    };
    IEnumerable<string> PasswordValidation(string password)
    {
      string numberPattern = @"\d";
      string upperCasePattern = @"[A-Z]";
      string symbolsPattern = @"[!@#$%^&*()_\-+=<>,.?]";

      if (string.IsNullOrWhiteSpace(password))
      {
        yield return "Password is required";
        yield break;
      }
      if (password.Length < 8 || password.Length > 25) { yield return "Password must be between 8-25 characters"; }
      if (!Regex.IsMatch(password, numberPattern)) { yield return "Password must contain at least one number"; }
      if (!Regex.IsMatch(password, upperCasePattern)) { yield return "Password must contain at least one uppercase letter"; }
      if (!Regex.IsMatch(password, symbolsPattern)) { yield return "Password must contain at least one symbol"; }
    }
  }
}