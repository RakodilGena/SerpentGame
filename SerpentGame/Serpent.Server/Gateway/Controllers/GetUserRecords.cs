using Microsoft.AspNetCore.Mvc;
using Serpent.Server.Gateway.Mappers;
using Serpent.Server.Gateway.Models.Out.UserRecordsResponses;
using Serpent.Server.Gateway.Services;

namespace Serpent.Server.Gateway.Controllers;

public partial class GatewayController
{
    [HttpGet("getUserRecords")]
    [ProducesResponseType<UserRecordsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserRecordsResponse>> GetUserRecordsAsync(
        IUserRecordsService userRecordsService,
        CancellationToken cancellationToken)
    {
        var result = await userRecordsService.GetUserRecordsAsync(cancellationToken);

        var response = result.ToResponse();

        return response;
    }
}