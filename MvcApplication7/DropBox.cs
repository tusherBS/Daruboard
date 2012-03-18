using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using AppLimit.CloudComputing.SharpBox;

namespace Daruyanagi
{
    public class DropBox : IDisposable
    {
        public readonly CloudStorage Storage = new CloudStorage();
        public readonly ICloudStorageConfiguration Configuration = CloudStorage
            .GetCloudConfigurationEasy(nSupportedCloudConfigurations.DropBox);

        public ICloudStorageAccessToken AccessToken { get; private set; }
        public ICloudStorageAccessToken StorageToken { get; private set; }

        public ICloudDirectoryEntry Root { get { return Storage.GetRoot(); } }

        public DropBox() : this("App_Data/DropBoxToken")
        {

        }

        public DropBox(string path)
        {
            AccessToken = GetAccessToken(path);
            StorageToken = Storage.Open(Configuration, AccessToken);
        }

        public ICloudStorageAccessToken GetAccessToken(string path)
        {
            using (var f = File.Open(
                Path.Combine(MvcApplication.PhysicalDir, path),
                FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Storage.DeserializeSecurityToken(f);
            }
        }

        public ICloudDirectoryEntry this[string path]
        {
            get
            {
                return Storage.GetFolder(path);
            }
        }

        public void Dispose()
        {
            if (Storage.IsOpened) Storage.Close();
        }
    }
}