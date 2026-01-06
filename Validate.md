## 1 . Custom Middleware

We will be creating custom middleware to add validation for all input requests, and that will be centralized code for all api endpoints
```
public class RequestValidationMiddleware
{
     private readonly RequestDelegate _next;
     private readonly ILogger<RequestValidationMiddleware> _logger;

     public RequestValidationMiddleware(RequestDelegate next, ILogger<RequestValidationMiddleware> logger)
     {
         _next = next;
         _logger = logger;
     }

     public async Task InvokeAsync(HttpContext httpContext)
     {
         // Check if the request is JSON (Content-Type: application/json)
         if (httpContext.Request.ContentType?.ToLower() == "application/json")
         {
             // Read the body content
             var body = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
            // Typically, you will handle a generic validation
         }

         // Proceed to the next middleware or controller if validation passes
         await _next(httpContext);
     }
}
```
## 2. Model Validation with Data Annotations
One of the most common and clean ways to validate input in .Net Core is using data annotations. This technique uses attributes to define validation rules on model properties.
```
Step 1: Create a new model and add some validation attributes
     public class  LoginModel
     {
       [Required]
       [StringLength(50, MinimumLength = 5)]
       public string UserName { get; set; }
       
       [Required]
       public string Password { get; set; }
     }
Step 2: Controller
   [HttpPost]
    public IActionResult Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);  // Returns validation errors to the client
        }
        return Ok();

    }
```
## 3. Validation Requests using Action Attributes
```
Step 1: Create a new ActionFilter and inherit from ActionFilterAttribute
 public class ValidateInputActionFilter : ActionFilterAttribute
 {
    
     public ValidateInputActionFilter()
     {
        
     }

     public override void OnActionExecuting(ActionExecutingContext context)
     {
         // Check if the model state is valid
         if (!context.ModelState.IsValid)
         {
             // If the model state is invalid, log the error and return a bad request response
             context.Result = new BadRequestObjectResult(context.ModelState);
             return; // Prevents the action method from being called
         }

         base.OnActionExecuting(context);
     }
 }

 Step 2: Add ValidateInputActionFilter to each controller method you want to check.
    [ValidateInputActionFilter]
    [HttpPost]
    public IActionResult Login(UserModel user)
    {
      return Ok();
    }
```
