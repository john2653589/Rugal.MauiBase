using Microsoft.Extensions.Configuration;
using Rugal.MauiBase.Core.Model;
using Rugal.MauiBase.Core.Page;
using Rugal.MauiBase.Test.Api;

namespace Rugal.MauiBase.Test
{
    public class MainPageModel : InpcModel
    {
        public TestGetResult TestGet
        {
            get => GetValue<TestGetResult>();
            set => SetValue(value);
        }

        public TestPostResult TestPost
        {
            get => GetValue<TestPostResult>();
            set => SetValue(value);
        }
    }

    public partial class MainPage : ModelPage<MainPageModel>
    {
        public MainPage()
        {
            InitializeComponent();

            Binder
                .WithProperty(Label.TextProperty)
                    .Bind(Label_TestGet, Item => Item.TestGet.Param)
                    .Bind(Label_TestPost, Item => Item.TestPost.Param);

            Client.WithToken("asdadsad");

            TestUpload();
            //Loop();
        }

        private async void TestUpload()
        {
            var FilePath = $"{FileSystem.AppDataDirectory}/test.jpg";
            while (true)
            {
                Client.AsService<TestService>()
                       .ApiCall_FormParam(Api => Api.TestForm, new TestFormModel()
                       {
                           Param = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                       }, Option =>
                       {
                           Option.AddFile("File", File.ReadAllBytes(FilePath), "image/*");
                       })
                       .ApiCall_Form(Api => Api.TestForm2, Option =>
                       {
                           Option.AddFile("File", File.ReadAllBytes(FilePath), "image/*");
                       });


                await Task.Delay(3000);
            }

        }

        private async void Loop()
        {
            var Count = 1;
            while (true)
            {
                Client.AsService<TestService>()
                    .ApiCall_Param(Api => Api.TestGet, new()
                    {
                        Param = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                Count++;
                await Task.Delay(1000);
            }
        }
    }
}