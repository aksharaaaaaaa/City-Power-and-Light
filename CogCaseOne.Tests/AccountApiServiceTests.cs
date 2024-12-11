using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CogCaseOne.Services;
using Moq;
using Moq.Protected;

[TestClass]
public class AccountApiServiceTests
{
    [TestMethod]
    public async Task GetAccountById_ReturnsAccountData()
    {
        // Arrange
        var accountId = "test-account-id";
        var expectedResponse = "{\"name\":\"Test Account\",\"accountid\":\"test-account-id\"}";

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedResponse),
            });

        var httpClient = new HttpClient(handlerMock.Object);

        // Act
        var result = await AccountApiService.GetAccountById(httpClient, accountId);

        // Assert
        Assert.IsTrue(result.Contains("Test Account"));
        Assert.IsTrue(result.Contains("test-account-id"));
    }
}
