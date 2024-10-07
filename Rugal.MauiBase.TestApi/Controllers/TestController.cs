using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Rugal.MauiBase.TestApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        public TestController()
        {

        }

        [HttpGet]
        public dynamic TestGet(string Param)
        {
            return new
            {
                Param,
            };
        }

        public record class PostParam
        {
            public string Param { get; set; }
        }

        [HttpPost]
        public dynamic TestPost(int Count, PostParam Param)
        {
            Param.Param = $"Count: {Count} = {Param.Param}";
            return Param;
        }

        [HttpPost]
        public dynamic TestPost2(string Param)
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        [HttpPost]
        public dynamic TestForm(IFormFile File, [FromForm] PostParam Body2)
        {
            var Form = Request.Form;
            //return Body.Param;
            return Body2.Param;
        }
    }
}
