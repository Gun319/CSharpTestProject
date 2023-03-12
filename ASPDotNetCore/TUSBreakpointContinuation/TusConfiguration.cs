using System.Text;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;
using tusdotnet.Models.Expiration;
using tusdotnet.Stores;

namespace TUSBreakpointContinuation
{
    public static class TusConfiguration
    {
        public static DefaultTusConfiguration CreateTusConfiguration(WebApplicationBuilder builder)
        {
            var tusFilesPath = Path.Combine(builder.Environment.WebRootPath, "tusFiles"); // 文件存放路径

            // 检查文件夹是否存在
            if (!Directory.Exists(tusFilesPath))
                Directory.CreateDirectory(tusFilesPath);

            return new DefaultTusConfiguration()
            {
                UrlPath = "/uploadfile",
                Store = new TusDiskStore(tusFilesPath),
                MetadataParsingStrategy = MetadataParsingStrategy.AllowEmptyValues, // 源数据是否允许空值
                Expiration = new AbsoluteExpiration(TimeSpan.FromMinutes(5)), // 文件过期后不再更新
                Events = new Events
                {
                    OnFileCompleteAsync = async ctx =>
                    {
                        var file = await ctx.GetFileAsync(); // 获取上传文件
                        var metadata = await file.GetMetadataAsync(ctx.CancellationToken); // 获取上传文件源数据
                        var fileName = metadata["name"].GetString(Encoding.UTF8); // 获取上传文件源数据中的文件目标名称

                        var extensionName = Path.GetExtension(fileName);

                        // 将上传文件转换为目标文件
                        File.Move(Path.Combine(tusFilesPath, ctx.FileId), Path.Combine(tusFilesPath, $"{ctx.FileId}{extensionName}"));

                        // 删除当前上传的文件
                        var terminationStore = ctx.Store as ITusTerminationStore;
                        await terminationStore!.DeleteFileAsync(file.Id, ctx.CancellationToken);
                    }
                }
            };
        }
    }
}
