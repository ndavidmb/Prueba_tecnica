using Application.DTOs.Matches;
using Application.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;

namespace Test.Controllers;

public class MatchesControllerTests
{
    private readonly Mock<IMatchService> _matchService = new();
    private readonly MatchesController _sut;

    public MatchesControllerTests()
    {
        _sut = new MatchesController(_matchService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithAllMatches()
    {
        var matches = new List<MatchListItemDto>
        {
            new(10, new TeamDto(100, "River Plate"), new TeamDto(200, "Boca Juniors"), null, null, MatchStatus.UpcomingMatch),
            new(11, new TeamDto(101, "Independiente"), new TeamDto(201, "Racing Club"), 3, 1, MatchStatus.FullTime)
        };
        _matchService.Setup(s => s.GetAllAsync()).ReturnsAsync(matches);

        var result = await _sut.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(matches, okResult.Value);
    }

    [Fact]
    public async Task GetAll_WhenNoMatchesExist_ReturnsOkWithEmptyCollection()
    {
        _matchService.Setup(s => s.GetAllAsync()).ReturnsAsync([]);

        var result = await _sut.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Empty(Assert.IsAssignableFrom<IEnumerable<MatchListItemDto>>(okResult.Value));
    }
}
