using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using D424_TL.Components.Pages;
using Microsoft.EntityFrameworkCore;
using D424_TL.Database;

namespace D424_TL.Tests
{
  public class RedirectUnitTests : TestContext
  {
    public RedirectUnitTests()
    {
      var mockFactory = new Mock<IDbContextFactory<D424DataContext>>();
      Services.AddSingleton(mockFactory.Object);
    }

    [Fact]
    public void TestRedirect_CurrUserNull_ToSignIn()
    {
      // Arrange
      Global.CurrUser = null;
      var nav = Services.GetRequiredService<NavigationManager>();

      // Act
      RenderComponent<Redirect>();

      // Assert
      Assert.Equal("http://localhost/", nav.Uri);
    }

    [Fact]
    public void TestRedirect_CurrUserAdmin_ToAdminDashboard()
    {
      // Arrange
      Global.CurrUser = new User 
      {
        FirstName = "Test",
        LastName = "Admin",
        Role = "Admin"
      };
      var nav = Services.GetRequiredService<NavigationManager>();

      // Act
      RenderComponent<Redirect>();

      // Assert
      Assert.Equal("http://localhost/admindashboard", nav.Uri);
    }

    [Fact]
    public void TestRedirect_CurrUserTeacher_ToTeacherDashboard()
    {
      // Arrange
      Global.CurrUser = new User
      {
        FirstName = "Test",
        LastName = "Teacher",
        Role = "Teacher"
      };
      var nav = Services.GetRequiredService<NavigationManager>();

      // Act
      RenderComponent<Redirect>();

      // Assert
      Assert.Equal("http://localhost/teacherdashboard", nav.Uri);
    }

    [Fact]
    public void TestRedirect_CurrUserStudent_ToStudentDashboard()
    {
      // Arrange
      Global.CurrUser = new User
      {
        FirstName = "Test",
        LastName = "Student",
        Role = "Student"
      };
      var nav = Services.GetRequiredService<NavigationManager>();

      // Act
      RenderComponent<Redirect>();

      // Assert
      Assert.Equal("http://localhost/studentdashboard", nav.Uri);
    }
  }
}