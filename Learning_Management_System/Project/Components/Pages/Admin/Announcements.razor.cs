namespace D424_TL.Components.Pages.Admin
{
  public partial class Announcements
  {
    Announcement? selectedAnnouncement;
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
          await RefreshAllAnnouncements();
        }
      }
    }
    async void OnAddAnnouncementClicked()
    {
      var dialog = DialogService.Show<NewAnnouncementDialog>("Add Announcement");
      var result = await dialog.Result;
      await RefreshAllAnnouncements();
    }
    async Task UpdateAnnouncement(Announcement announcement)
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        announcement.LastUpdate = DateTime.Now;
        context.Announcement.Update(announcement);
        await context.SaveChangesAsync();

        selectedAnnouncement = null;
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"UpdateAnnouncement: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
      }
    }
    async Task DeleteAnnouncement(Announcement announcement)
    {
      try
      {
        bool? result = await DialogService.ShowMessageBox("Warning", "Are you sure you want to delete this announcement?", yesText: "Yes", cancelText: "No");
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();

        if (result != null && (bool)result)
        {
          context.Announcement.Remove(announcement);
          await context.SaveChangesAsync();

          Global.AllAnnouncements.Remove(announcement);
          StateHasChanged();
        }
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"DeleteAnnouncement: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
      }
    }
    void StartedEditingItem(Announcement announcement) => selectedAnnouncement = announcement;
    void CanceledEditingItem() => selectedAnnouncement = null;
    async Task RefreshAllAnnouncements()
    {
      await Global.GetAllUsers(D424DataContextFactory);
      await Global.GetFlaggedMessages(D424DataContextFactory);
      await Global.GetAllAnnouncements(D424DataContextFactory);
      StateHasChanged();
    }
    Func<Announcement, bool> Search => x =>
    {
      if (string.IsNullOrWhiteSpace(searchInput))
      {
        return true;
      }
      if (x.Message.Contains(searchInput.Trim(), StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }
      return false;
    };
  }
}
