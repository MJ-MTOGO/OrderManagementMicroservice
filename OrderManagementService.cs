using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace OrderManagementService;

public class Function : IHttpFunction
{
    public async Task HandleAsync(HttpContext context)
    {
        await context.Response.WriteAsync("GET: OrderManagementService", context.RequestAborted);
        // gemmer data..
        // du sender response til brugeren af apien

        //publish message to topic.... processorder
    }


}

