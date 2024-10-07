using Rugal.MauiBase.Core.Model;
using Rugal.MauiBase.Core.Service;

namespace Rugal.MauiBase.Test.Api
{
    public class TestService : ApiServiceBase
    {
        public override string BasePath => "Test";
        public ApiInfo<TestGetParam, TestGetResult> TestGet => new();
        public ApiInfo<TestPostParam, TestPostResult> TestPost => new(HttpMethodType.Post);
        public ApiInfo<string> TestPost2 => new(HttpMethodType.Post);
        public ApiInfo<TestFormModel, string> TestForm => new(HttpMethodType.Post);
        public ApiInfo<string> TestForm2 => new(HttpMethodType.Post);
    }
}