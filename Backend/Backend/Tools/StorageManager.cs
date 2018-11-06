using Microsoft.Win32;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class StorageManager
{

    public static string UploadToStorage(HttpPostedFileBase file, int id = 0, string name = null)
    {
        // Generate a name for the blob
        if (name == null)
        {
            name = id + "_" + Guid.NewGuid().ToString() + "_" + DateTime.Now.Second + GetDefaultExtension(file.ContentType);
        }

        CloudStorageAccount storageAcc = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=hacktheirl;AccountKey={key};EndpointSuffix=core.windows.net");

        // Create the blob client.
        CloudBlobClient blobClient = storageAcc.CreateCloudBlobClient();

        // Retrieve a reference to a container.
        CloudBlobContainer container = blobClient.GetContainerReference("images");

        // Create the container if it doesn't already exist.
        container.CreateIfNotExists();

        container.SetPermissions(
            new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

        // Retrieve reference to a blob.
        CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);

        blockBlob.UploadFromStream(file.InputStream);

        return "https://hacktheirl.blob.core.windows.net/images" + "/" + name;
    }

    public static string GetDefaultExtension(string mimeType)
    {
        string result;
        RegistryKey key;
        object value;

        key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
        value = key != null ? key.GetValue("Extension", null) : null;
        result = value != null ? value.ToString() : string.Empty;

        return result;
    }
}