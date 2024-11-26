namespace ProjectPRN221.Util
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();

            if (path.StartsWith("/adminpage"))
            {
                var role = context.Session.GetString("Role");
                if (role == null || role.ToLower() != "admin")
                {
                    context.Response.Redirect("/AccessDenied");
                    return;
                }
            }

            await _next(context);
        }
    }

}