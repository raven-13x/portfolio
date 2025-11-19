namespace D424_TL.Components.Pages.Admin
{
  public partial class ClassManagement
  {
    Class? selectedClass;
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
          await RefreshAllClasses();
        }
      }
    }
    async void OnAddClassClicked()
    {
      var dialog = DialogService.Show<NewClassDialog>("Add Class");
      var result = await dialog.Result;
      await RefreshAllClasses();
    }
    async Task UpdateClass(Class _class)
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        _class.LastUpdate = DateTime.Now;
        context.Class.Update(_class);
        await context.SaveChangesAsync();

        selectedClass = null;
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"UpdateCourse: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
      }
    }
    async Task DeleteClass(Class _class)
    {
      try
      {
        bool? result = await DialogService.ShowMessageBox("Warning", "Are you sure you want to delete this course and all associated data?", yesText: "Yes", cancelText: "No");
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();

        if (result != null && (bool)result)
        {
          var courseworkEntries = await context.Coursework.Where(c => c.ClassId == _class.Id).ToListAsync();
          foreach (var entry in courseworkEntries)
          {
            var grades = await context.Grade.Where(g => g.CourseworkId == entry.Id).ToListAsync();
            foreach(var grade in grades) 
            {
              context.Grade.Remove(grade);
            }

            context.Coursework.Remove(entry);
          }
          
          context.Class.Remove(_class);
          await context.SaveChangesAsync();

          Global.AllClasses.Remove(_class);
          StateHasChanged();
        }
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"DeleteCourse: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
      }
    }
    void StartedEditingItem(Class _class) => selectedClass = _class;
    void CanceledEditingItem() => selectedClass = null;
    async Task RefreshAllClasses()
    {
      await Global.GetAllUsers(D424DataContextFactory);
      await Global.GetFlaggedMessages(D424DataContextFactory);
      await Global.GetAllTeachers(D424DataContextFactory);
      await Global.GetAllClasses(D424DataContextFactory);
      StateHasChanged();
    }
    Func<Class, bool> Search => x =>
    {
      if (string.IsNullOrWhiteSpace(searchInput))
      {
        return true;
      }
      if (x.TeacherId.ToString().Contains(searchInput.Trim(), StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }
      if (x.Title.Contains(searchInput.Trim(), StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }
      return false;
    };
  }
}
