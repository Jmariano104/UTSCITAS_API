using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    [Route("/error")]
    public IActionResult Error()
    {
        return Problem(
            detail: "Ocurrió un error interno en el servidor",
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
}
