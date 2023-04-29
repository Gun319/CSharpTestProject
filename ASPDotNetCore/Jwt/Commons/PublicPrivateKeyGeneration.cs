using System.Security.Cryptography;

namespace Jwt.Commons
{
    /// <summary>
    /// 公私钥生成
    /// </summary>
    public class PublicPrivateKeyGeneration
    {
        /// <summary>
        /// 创建公私钥对
        /// </summary>
        /// <returns>公私钥存储 struct</returns>
        public Secret CreateSecret()
        {
            using RSA rsa = RSA.Create();
            Secret secret = new()
            {
                PrivateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey()),
                PublicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey())
            };
            return secret;
        }

        public async void FileSave(string filePath, Secret secret)
        {
            FileInfo fileInfo = new(filePath);

            if (fileInfo.Exists)
                fileInfo.Delete();

            StreamWriter sw = fileInfo.AppendText();
            await sw.WriteAsync($"----私钥----\n{secret.PrivateKey}\n----公钥----\n{secret.PublicKey}");
            await sw.DisposeAsync();
        }

        public struct Secret
        {
            public string PublicKey;
            public string PrivateKey;
        }
    }
}
