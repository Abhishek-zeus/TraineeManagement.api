namespace TraineeManagement.myapp.Middleware
{
    public class CorrelationMiddleware
    {
        private readonly RequestDelegate _next;
        private const string HeaderName = "X-Correlation-Id";
        public CorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<CorrelationMiddleware> logger)
        {
            //Honor incoming ID if caller sypplied else create a fresh one
            var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var existing) ? existing.ToString() : Guid.NewGuid().ToString();
            //set Id for response
            context.Response.Headers[HeaderName] = correlationId;
            context.Items["CorrelationId"] = correlationId; //any controller can read this
            // Use string formatting inside the scope definition
            using (logger.BeginScope("CorrelationId:{CorrelationId}", correlationId))
            {
                await _next(context);
            }

        }
    }
}