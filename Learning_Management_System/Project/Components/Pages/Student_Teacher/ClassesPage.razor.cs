namespace D424_TL.Components.Pages.Student_Teacher
{
  public partial class ClassesPage
  {
    ObservableCollection<ScheduleEntry>? ClassesQ1 { get; set; } = new ObservableCollection<ScheduleEntry>();
    ObservableCollection<ScheduleEntry>? ClassesQ2 { get; set; } = new ObservableCollection<ScheduleEntry>();
    ObservableCollection<ScheduleEntry>? ClassesQ3 { get; set; } = new ObservableCollection<ScheduleEntry>();
    ObservableCollection<ScheduleEntry>? ClassesQ4 { get; set; } = new ObservableCollection<ScheduleEntry>();

    List<ScheduleEntry> studentScheduleEntries = new List<ScheduleEntry>();
    int selectedYear = DateTime.Today.Year;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      await base.OnAfterRenderAsync(firstRender);
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
        else 
        {
          await RefreshData();
          Global.SelectedClassId_Coursework = 0;
          Snackbar.Add("Click on a class to view coursework details.", Severity.Info);
        }
      }
    }
    async Task RefreshData()
    {
      await Global.GetAllUsers(D424DataContextFactory);
      await Global.GetAllClasses(D424DataContextFactory);
      await GetFullSchedule(selectedYear);
      await Global.GetUnreadMessages(D424DataContextFactory);
      StateHasChanged();
    }
    string GetTeacher(int classId)
    {
      var _class = Global.AllClasses.Where(c => c.Id == classId).FirstOrDefault();
      var teacher = Global.AllUsers.Where(u => u.Id == _class.TeacherId).FirstOrDefault();
      return $"{teacher.FirstName} {teacher.LastName}";
    }
    async Task GetFullSchedule(int year)
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();

        ClassesQ1?.Clear();
        ClassesQ2?.Clear();
        ClassesQ3?.Clear();
        ClassesQ4?.Clear();
        var schedule = await context.ScheduleEntry.Where(s => s.UserId == Global.CurrUser.Id).OrderBy(s => s.GradingPeriod).OrderBy(s => s.Period).ToListAsync();
        foreach (var scheduleEntry in schedule)
        {
          if (scheduleEntry.GradingPeriod == 1) { ClassesQ1.Add(scheduleEntry); }
          else if (scheduleEntry.GradingPeriod == 2) { ClassesQ2.Add(scheduleEntry); }
          else if (scheduleEntry.GradingPeriod == 3) { ClassesQ3.Add(scheduleEntry); }
          else if (scheduleEntry.GradingPeriod == 4) { ClassesQ4.Add(scheduleEntry); }
        }
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"GetFullSchedule: {ex.Message}");
      }
    }
    async Task<int> GetEnrollmentByClass(int classId, int gradingPeriod)
    {
      try
      {
        int result = 0;

        studentScheduleEntries?.Clear();
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        var scheduleEntries = await context.ScheduleEntry.Where(s => s.ClassId == classId && s.GradingPeriod == gradingPeriod).ToListAsync();

        foreach (var scheduleEntry in scheduleEntries)
        {
          if (Global.AllUsers.Where(u => u.Id == scheduleEntry.UserId && u.Role == "Student").FirstOrDefault() != null)
          {
            studentScheduleEntries?.Add(scheduleEntry);
            result++;
          }
        }
        return result;
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"GetAllMessages: {ex.Message}");
        Snackbar.Add("There was an error obtaining enrollment information. Please contact your site admin.", Severity.Error);
        return 0;
      }
    }
    async Task<string> GetGrade(ScheduleEntry scheduleEntry)
    {
      try
      {
        float totalScore = 0;
        float totalPossible = 0;
        float gradePercent;

        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        var courseworkList = await context.Coursework.Where(c => c.ClassId == scheduleEntry.ClassId && c.GradingPeriod == scheduleEntry.GradingPeriod && c.Year == scheduleEntry.Year).ToListAsync();
        if (courseworkList.Count == 0) { return "N/A"; }

        foreach (var coursework in courseworkList)
        {
          var grade = await context.Grade.Where(g => g.CourseworkId == coursework.Id && g.StudentId == Global.CurrUser.Id).FirstOrDefaultAsync();
          if (grade == null || grade.Score < 0) { continue; }

          totalPossible += coursework.MaxScore;
          totalScore += grade.Score;
        }

        if (totalPossible == 0) { gradePercent = -1; } else { gradePercent = (totalScore / totalPossible) * 100; }

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
        else if (gradePercent < 60 && gradePercent >= 0) { return "F"; }
        else { return "N/A"; }
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"GetGrade: {ex.Message}");
        Snackbar.Add("There was an error obtaining enrollment information. Please contact your site admin.", Severity.Error);
        return "Error";
      }
    }
    void ViewCoursework(int classId, int gradingPeriod, int year)
    {
      try
      {
        Global.SelectedClassId_Coursework = classId;
        Global.SelectedClassGradingPeriod = gradingPeriod;
        Global.SelectedClassYear = year;
        nav.NavigateTo("coursework");
      }
      catch (Exception ex)
      {
        // Do nothing
      }
    }
  }
}
