using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Serpent.Server.Gateway.Mappers;
using Serpent.Server.Gateway.Models.In.FinishGameRequests;
using Serpent.Server.Gateway.Models.Out.FinishGameResponses;
using Serpent.Server.Gateway.Services;
using Serpent.Server.Gateway.Validators;

namespace Serpent.Server.Gateway.Controllers;

public partial class GatewayController
{
    [HttpPut("finishGame")]
    [ProducesResponseType<FinishGameResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FinishGameResponse>> StartGameAsync(
        FinishGameRequest finishGameRequest,
        IFinishGameService finishGameService)
    {
        var validator = new FinishGameRequestValidator();
        
        await validator.ValidateAndThrowAsync(finishGameRequest);
        
        var result = await finishGameService.FinishGameAsync(finishGameRequest.GameId!.Value);

        var response = result.ToResponse();

        return response;
    }
    
}