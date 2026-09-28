using Application.DTOs.Matches;
using Application.Services;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;

namespace Test.Controllers;

public class AdminMatchesControllerTests
{
    private readonly Mock<IMatchService> _matchService = new();
    private readonly AdminMatchesController _sut;

    public AdminMatchesControllerTests()
    {
        _sut = new AdminMatchesController(_matchService.Object);
    }

    [Fact]
    public async Task UpdateResult_WithExistingMatch_ReturnsOkWithResult()
    {
        const int matchId = 10;
        var dto = new UpdateMatchResultDto(LocalGoals: 3, VisitorGoals: 1);
        var expected = new MatchResponseDto(matchId, 100, 200, dto.LocalGoals, dto.VisitorGoals, MatchStatus.FullTime);
        _matchService.Setup(s => s.UpdateResultAsync(matchId, dto)).ReturnsAsync(expected);

        var result = await _sut.UpdateResult(matchId, dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task UpdateResult_WhenMatchDoesNotExist_ReturnsBadRequest()
    {
        const int matchId = 999;
        var dto = new UpdateMatchResultDto(1, 0);
        _matchService.Setup(s => s.UpdateResultAsync(matchId, dto))
            .ThrowsAsync(new DomainException("El partido no existe."));

        var result = await _sut.UpdateResult(matchId, dto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }
}
