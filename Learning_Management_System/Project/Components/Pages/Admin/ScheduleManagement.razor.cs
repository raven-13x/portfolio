namespace D424_TL.Components.Pages.Admin
{
  public partial class ScheduleManagement
  {
    ScheduleEntry? selectedEntry;
    string? searchInput;

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
          await RefreshAllScheduleEntries();
        }
      }
    }
    async void OnAddEntryClicked()
    {
      var dialog = DialogService.Show<NewScheduleEntryDialog>("Add Entry");
      var result = await dialog.Result;
      await RefreshAllScheduleEntries();
    }
    async Task UpdateScheduleEntry(ScheduleEntry scheduleEntry)
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        scheduleEntry.LastUpdate = DateTime.Now;
        context.ScheduleEntry.Update(scheduleEntry);
        await context.SaveChangesAsync();

        selectedEntry = null;
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"UpdateScheduleEntry: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
      }
    }
    async Task DeleteScheduleEntry(ScheduleEntry scheduleEntry)
    {
      try
      {
        bool? result = await DialogService.ShowMessageBox("Warning", "Are you sure you want to delete this entry and all associated data?", yesText: "Yes", cancelText: "No");
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();

        if (result != null && (bool)result)
        {
          var attendanceEntries = await context.Attendance.Where(a => a.ScheduleEntryId == scheduleEntry.Id).ToListAsync();
          foreach (var attendanceEntry in attendanceEntries)
          {
            context.Attendance.Remove(attendanceEntry);
          }

          var courseworkEntries = await context.Coursework.Where(c => c.ClassId == scheduleEntry.ClassId).ToListAsync();
          foreach (var courseworkEntry in courseworkEntries)
          {
            var grade = await context.Grade.Where(g => g.CourseworkId == courseworkEntry.Id).FirstAsync();
            context.Grade.Remove(grade);
          }

          context.ScheduleEntry.Remove(scheduleEntry);
          await context.SaveChangesAsync();

          Global.AllScheduleEntries.Remove(scheduleEntry);
          StateHasChanged();
        }
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"DeleteScheduleEntry: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
      }
    }
    void StartedEditingItem(ScheduleEntry scheduleEntry) => selectedEntry = scheduleEntry;
    void CanceledEditingItem() => selectedEntry = null;
    async Task RefreshAllScheduleEntries()
    {
      await Global.GetFlaggedMessages(D424DataContextFactory);
      await Global.GetAllScheduleEntries(D424DataContextFactory);
      await Global.GetAllUsers(D424DataContextFactory);
      await Global.GetAllClasses(D424DataContextFactory);
      StateHasChanged();
    }
    Func<ScheduleEntry, bool> Search => x =>
    {
      if (string.IsNullOrWhiteSpace(searchInput))
      {
        return true;
      }
      if (x.UserId.ToString().Contains(searchInput.Trim(), StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }
      if (x.ClassId.ToString().Contains(searchInput.Trim(), StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }
      return false;
    };
  }
}
