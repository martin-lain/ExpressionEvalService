using System;
using System.Globalization;
using ExpressionEvalService.BL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExpressionEvalService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ComputeController : ControllerBase
    {
        private readonly ILogger<ComputeController> _logger;
        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

        public ComputeController(ILogger<ComputeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get(string expr)
        {
            _logger.LogDebug($"Evaluating ${expr}");
            if (expr is null || expr.Length == 0) return "No expression";
            expr = expr.Replace(" ", "+");
            try
            {
                var value = Evaluator.Evaluate(expr);
                return value.ToString(_culture);
            }catch(ExpresionException e)
            {
                return e.Message;
            }
            catch (Exception e)
            {
                _logger.LogError(e, expr);
                return "Expression error";
            }
        }
    }
}
