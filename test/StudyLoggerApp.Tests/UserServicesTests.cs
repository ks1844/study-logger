using StudyLoggerApp.Services;
using Xunit;

public class UserServiceTests
{
    [Fact]
    public void Hello_ReturnsCorrectMessage()
    {
        // Arrange（準備）
        var service = new UserService();

        // Act（実行）
        var result = service.Hello("ken");

        // Assert（検証）
        Assert.Equal("Hello, ken", result);
    }
}