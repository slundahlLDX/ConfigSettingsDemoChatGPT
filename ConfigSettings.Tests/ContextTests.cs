using ConfigSettings.Shared;
using ConfigSettings.Shared.Interfaces;
using ConfigSettings.Shared.Models;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ConfigSettings.Tests;

public class ContextTests
{
    [Fact]
    public async Task Context_Uses_CurrentStrategy_GetAll()
    {
        var mock = new Mock<IConfigSettingsStrategy>();
        mock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ConfigSetting> { new ConfigSetting { ConfigSettingsId = 1, FieldName = "T" } });

        var ctx = new ConfigSettingsContext(mock.Object);
        var all = await ctx.GetAllAsync();

        Assert.Single(all);
        mock.Verify(m => m.GetAllAsync(), Times.Once);
    }
}
