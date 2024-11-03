using Microsoft.AspNetCore.Mvc;
using Serpent.Server.Gateway.Mappers;
using Serpent.Server.Gateway.Models.In.StartGameRequests;
using Serpent.Server.Gateway.Models.Out.StartGameResponces;
using Serpent.Server.Gateway.Services;

namespace Serpent.Server.Gateway.Controllers;

public partial class GatewayController
{
    [HttpPost("startGame")]
    [ProducesResponseType<StartGameResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<StartGameResponse>> StartGameAsync(
        StartGameRequest request,
        IStartGameService startGameService,
        CancellationToken cancellationToken)
    {
        var requestDto = request.ToDto();
        
        var result = await startGameService.StartGameAsync(requestDto, cancellationToken);

        var response = result.ToResponse();

        return response;
    }
}