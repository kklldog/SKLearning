using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SKWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly Kernel _kernel;

        //public ChatController(Kernel Kernel)
        //{
        //    _kernel = Kernel;
        //}

        public ChatController(Kernel Kernel, IChatCompletionService chat)
        {
            _kernel = Kernel;
        }

        public string Get()
        {
            return "ok";
        }
    }
}
