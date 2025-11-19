namespace D424_TL.Components.Pages.Student_Teacher
{
  public partial class MessagesPage
  {
    ObservableCollection<KeyValuePair<User, Message>> Chats { get; set; } = new ObservableCollection<KeyValuePair<User, Message>>();
    ObservableCollection<Message> ChatHistory { get; set; } = new ObservableCollection<Message>();

    int recipient;
    string? messageContent;
    bool sendButtonDisabled = true;
    bool messageFieldDisabled = true;

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
          sendButtonDisabled = true;
          messageFieldDisabled = true;
          ChatHistory.Clear();
          StateHasChanged();
        }
      }
    }
    async void OnNewMessageClicked()
    {
      var dialog = DialogService.Show<NewMessageDialog>("New chat");
      var result = await dialog.Result;
      await RefreshData();
    }
    async Task RefreshData()
    {
      if (Global.CurrUser.Role == "Teacher") { await Global.GetRecipientsTeacher(D424DataContextFactory); } else { await Global.GetRecipientsStudent(D424DataContextFactory); }
      await GetChats();
      await Global.GetUnreadMessages(D424DataContextFactory);
      StateHasChanged();
    }
    async Task ShowHistory(int recipientId)
    {
      try
      {
        await GetChatHistory(recipientId);
        var unreadMessages = ChatHistory.Where(m => m.Read == "No" && m.RecipientId == Global.CurrUser.Id);

        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        foreach (var message in unreadMessages)
        {
          message.Read = "Yes";
          context.Update(message);
          await context.SaveChangesAsync();
          context.Message.Entry(message).State = EntityState.Detached;
        }

        sendButtonDisabled = false;
        messageFieldDisabled = false;
        recipient = recipientId;
        await GetChats();
        StateHasChanged();
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"ShowHistory: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
      }
    }
    async Task GetChats()
    {
      try
      {
        Chats.Clear();
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
        foreach (var recipient in Global.Recipients)
        {
          var latestMessage = await context.Message.Where(m => (m.SenderId == Global.CurrUser.Id && m.RecipientId == recipient.Id) || (m.SenderId == recipient.Id && m.RecipientId == Global.CurrUser.Id)).OrderByDescending(m => m.Sent).FirstOrDefaultAsync();
          if (latestMessage is null) { continue; }
          var keyValuePair = new KeyValuePair<User, Message>(recipient, latestMessage);
          if (Chats.Contains(keyValuePair)) { continue; }
          Chats.Add(keyValuePair);
        }
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"GetChats: {ex.Message}");
      }
    }
    async Task GetChatHistory(int recipientId)
    {
      try
      {
        D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();

        ChatHistory.Clear();
        var history = await context.Message.Where(m => (m.SenderId == Global.CurrUser.Id && m.RecipientId == recipientId) || (m.SenderId == recipientId && m.RecipientId == Global.CurrUser.Id)).OrderBy(m => m.Sent).ToListAsync();
        foreach (var message in history) { ChatHistory.Add(message); }
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"GetChatHistory: {ex.Message}");
      }
    }
    async Task SendMessage()
    {
      try
      {
        if (messageContent != null && recipient != 0)
        {
          D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
          var newMessage = new Message
          {
            Id = await context.Message.MaxAsync(m => m.Id) + 1,
            SenderId = Global.CurrUser.Id,
            RecipientId = recipient,
            Content = messageContent.Trim(),
            Flagged = "No",
            Sent = DateTime.Now,
            Read = "No"
          };
          context.Message.Add(newMessage);
          await context.SaveChangesAsync();
          await RefreshData();
          ChatHistory.Add(newMessage);
          messageContent = null;
        }
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"SendMessage: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
      }
    }
    async Task FlagMessage(Message message)
    {
      try
      {
        bool? result = await DialogService.ShowMessageBox("Confirm", "Do you want to flag this message for Admin review?", yesText: "Yes", cancelText: "No");
        if (result != null && (bool)result)
        {
          D424DataContext context = await D424DataContextFactory.CreateDbContextAsync();
          message.Flagged = "Yes";
          context.Update(message);
          await context.SaveChangesAsync();
          Snackbar.Add("The message will be reviewed by an admin. Please refrain from submitting multiple requests for the same message.", Severity.Info);
        }
      }
      catch (Exception ex)
      {
        await Global.Log(D424DataContextFactory, "ERROR", $"FlagMessage: {ex.Message}");
        Snackbar.Add("There was a problem fulfilling your request. Please contact your site admin", Severity.Error);
      }
    }
  }
}
