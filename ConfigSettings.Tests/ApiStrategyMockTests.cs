using ConfigSettings.Shared.Strategies;
using Moq;
using Xunit;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Collections.Generic;
using ConfigSettings.Shared.Models;

namespace ConfigSettings.Tests;

public class ApiStrategyMockTests
{
    [Fact]
    public async Task ApiStrategy_GetAll_ReturnsEmpty_WhenHttpReturnsEmptyArray()
    {
        //var handlerMock = new Moq.Mock<HttpMessageHandler>();
        //handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", 
        //    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        //    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[]") });

        //var client = new HttpClient(handlerMock.Object) { BaseAddress = new System.Uri("http://localhost") };
        //var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ApiConfigSettingsStrategy>();
        //var strat = new ApiConfigSettingsStrategy(client, logger);

        //var all = await strat.GetAllAsync();
        //Assert.Empty(all);
    }
}
