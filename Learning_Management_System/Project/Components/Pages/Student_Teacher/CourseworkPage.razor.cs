namespace D424_TL.Components.Pages.Student_Teacher
{
  public partial class CourseworkPage
  {
    ObservableCollection<Coursework> ClassCoursework { get; set; } = new ObservableCollection<Coursework>();

    Coursework? selectedCoursework;
    bool readOnly = true;
    string? className = "";
    string? searchInput;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      try
      {
        if (firstRender)
        {
          if (Global.CurrUser == null || Global.CurrUser.Role == "Admin") 
          {
            Global.MainContentClass = "signInBackground";
            Global.DrawerOpen = false;
            Global.DrawerClass = "invisible";
            Global.AppBarClass = "invisible";
            Global.MainContentWidth = MaxWidth.Small;
            nav.NavigateTo("", true);
          }
          else if (Global.SelectedClassId_Coursework == 0) { nav.NavigateTo("classes"); }
          else
          {
            D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
            className = (await context.Class.Where(c => c.Id == Global.SelectedClassId_Coursework).FirstAsync()).Title;
            await RefreshData();
            if (Global.CurrUser.Role == "Teacher")
            {
              Snackbar.Add("Click on an item's 'Coursework Name' to view grades.", Severity.Info);
              readOnly = false;
            }
          }
        }
      }
      catch (Exception ex)
      {
        // Do nothing 
      }
    }
    async Task RefreshData()
    {
      await GetClassCoursework();
      await Global.GetUnreadMessages(D424DataContextFactory);
      StateHasChanged();
    }
    async Task OnAddCourseworkClicked()
    {
      var dialog = DialogService.Show<NewCourseworkDialog>("Add Coursework");
      var result = await dialog.Result;
      await GetClassCoursework();
    }
    async Task UpdateCoursework(Coursework coursework)
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        coursework.LastUpdate = DateTime.Now;
        context.Coursework.Update(coursework);
        await context.SaveChangesAsync();

        selectedCoursework = null;
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"UpdateCoursework: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
      }
    }
    async Task DeleteCoursework(Coursework coursework)
    {
      try
      {
        bool? result = await DialogService.ShowMessageBox("Warning", "Are you sure you want to delete this coursework and all associated data?", yesText: "Yes", cancelText: "No");
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();

        if (result != null && (bool)result)
        {

          context ??= await D424DataContextFactory.CreateDbContextAsync();
          var grades = await context.Grade.Where(g => g.CourseworkId == coursework.Id).ToListAsync();
          foreach (var grade in grades)
          {
            context.Grade.Remove(grade);
          }

          context.Coursework.Remove(coursework);
          await context.SaveChangesAsync();
        }
        StateHasChanged();
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"DeleteCoursework: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
      }
    }
    void ViewGrades(int courseworkId)
    {
      try
      {
        Global.SelectedCourseworkId_Grades = courseworkId;
        nav.NavigateTo("grades");
      }
      catch (Exception ex)
      {
        // Do nothing
      }
    }
    async Task GetClassCoursework()
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();

        ClassCoursework.Clear();
        var courseworkList = await context.Coursework.Where(c => c.ClassId == Global.SelectedClassId_Coursework && c.GradingPeriod == Global.SelectedClassGradingPeriod && c.Year == Global.SelectedClassYear).ToListAsync();
        foreach (var courseworkEntry in courseworkList) { ClassCoursework.Add(courseworkEntry); }
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"GetClassCoursework: {ex.Message}");
      }
    }
    async Task<int> GetScore(int courseworkId)
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        int score = (await context.Grade.Where(g => g.StudentId == Global.CurrUser.Id && g.CourseworkId == courseworkId).FirstAsync()).Score;

        if (score < 0) { return 0; }

        return score;
      }
      catch (Exception ex) 
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"GetScore: {ex.Message}");
        Snackbar.Add("There was an error obtaining enrollment information. Please contact your site admin.", Severity.Error);
        return 0;
      }
    }
    async Task<string> GetGrade(Coursework coursework)
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        float score = (await context.Grade.Where(g => g.StudentId == Global.CurrUser.Id && g.CourseworkId == coursework.Id).FirstAsync()).Score;

        if (score < 0) { return "N/A"; }

        float gradePercent = (score / coursework.MaxScore) * 100;

        if (gradePercent >= 98) { return "A+"; }
        else if (gradePercent >= 94) { return "A"; }
        else if (gradePercent >= 90) { return "A-"; }
        else if (gradePercent >= 88) { return "B+"; }
        else if (gradePercent >= 84) { return "B"; }
        else if (gradePercent >= 80) { return "B-"; }
        else if (gradePercent >= 78) { return "C+"; }
        else if (gradePercent >= 74) { return "C"; }
        else if (gradePercent >= 70) { return "C-"; }
        else if (gradePercent >= 68) { return "D+"; }
        else if (gradePercent >= 64) { return "D"; }
        else if (gradePercent >= 60) { return "D-"; }
        else if (gradePercent < 60) { return "F"; }
        else { return "N/A"; }
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"GetGrade: {ex.Message}");
        Snackbar.Add("There was an error obtaining enrollment information. Please contact your site admin.", Severity.Error);
        return "Error";
      }
    }
    void StartedEditingItem(Coursework _coursework) => selectedCoursework = _coursework;
    void CanceledEditingItem() => selectedCoursework = null;
    Func<Coursework, bool> Search => x =>
    {
      if (string.IsNullOrWhiteSpace(searchInput))
      {
        return true;
      }
      if (x.Description.Contains(searchInput.Trim(), StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }
      return false;
    };
  }
}
