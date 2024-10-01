using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using RefreshToken.API.Filters;
using RefreshToken.API.Interfaces.Settings;
using RefreshToken.API.Settings.NotificationSettings;

namespace UnitTests.FiltersTests;

public sealed class NotificationFilterTests
{
    private readonly Mock<INotificationHandler> _notificationHandlerMock;
    private readonly NotificationFilter _notificationFilter;

    public NotificationFilterTests()
    {
        _notificationHandlerMock = new Mock<INotificationHandler>();
        _notificationFilter = new NotificationFilter(_notificationHandlerMock.Object);
    }

    [Fact]
    public void OnActionExecuted_DoesNotHaveNotifications_ResultIsNull()
    {
        // A
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());
        var context = new ActionExecutedContext(
            actionContext,
            [],
            null!);

        _notificationHandlerMock.Setup(n => n.HasNotifications())
            .Returns(false);

        // A
        _notificationFilter.OnActionExecuted(context);

        // A
        _notificationHandlerMock.Verify(n => n.GetNotifications(), Times.Never());

        Assert.Null(context.Result);
    }

    [Fact]
    public void OnActionExecuted_HasNotifications_ResultIsBadRequest()
    {
        // A
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());
        var context = new ActionExecutedContext(
            actionContext,
            [],
            null!);

        _notificationHandlerMock.Setup(n => n.HasNotifications())
            .Returns(true);

        var notifications = new List<Notification>()
        {
            new("test",
                "random"),
            new("test",
                "random"),
            new("test",
                "random")
        };
        _notificationHandlerMock.Setup(n => n.GetNotifications())
            .Returns(notifications);

        // A
        _notificationFilter.OnActionExecuted(context);

        // A
        _notificationHandlerMock.Verify(n => n.GetNotifications(), Times.Once());

        var contextResult = context.Result;
        Assert.NotNull(contextResult);
        Assert.IsType<BadRequestObjectResult>(contextResult);

        var badRequestObjectResult = contextResult as BadRequestObjectResult;
        Assert.Equal(notifications, badRequestObjectResult!.Value);
    }
}
