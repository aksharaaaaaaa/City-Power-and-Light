using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using CogCaseOne.Services;

[Collection("AccountTests")]
public class AccountApiServiceTests
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetAccountById_ShouldReturnAccountWithValidId()
    {
        // Arrange
        var accountId = "test-account-id"; // Define test account ID
        var expectedResponse = "{\"name\":\"Test Account\",\"accountid\":\"test-account-id\"}"; // Define expected JSON response

        var handlerMock = new Mock<HttpMessageHandler>(); // Create mock HttpMessageHandler to simulate HTTP response
        handlerMock
            .Protected() // Access protected members of HttpMessageHandler
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", // Set up SendAsync method - covers all HTTP requests sent by HttpClient
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>() 
            )
            .ReturnsAsync(new HttpResponseMessage // Return simulated HTTP response
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedResponse),
            });

        var httpClient = new HttpClient(handlerMock.Object); // Create HttpClient using mock handler

        // Act
        var result = await AccountApiService.GetAccountById(httpClient, accountId);

        // Assert
        Assert.Contains("Test Account", result);
        Assert.Contains("test-account-id", result);
    }
}
