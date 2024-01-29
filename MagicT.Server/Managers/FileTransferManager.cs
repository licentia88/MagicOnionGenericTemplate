using System.Net;
using Benutomo;
using Grpc.Core;
using MagicOnion;

namespace MagicT.Server.Managers;

//[RegisterSingleton]
[AutomaticDisposeImpl]
public partial class FileTransferManager : IDisposable, IAsyncDisposable
{
 
    public async Task WriteFileToRemotePathAsync(byte[] fileBytes, string server, string domain, string username, string password, string path, string fileName)
    {
        var remoteFilePath = $@"\\{server}\{path}\{fileName}";

        _ = new NetworkCredential(username, password, domain);


        try
        {
            await using var fileStream = new FileStream(remoteFilePath + "", FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
            await fileStream.WriteAsync(fileBytes);
        }
        catch (UnauthorizedAccessException)
        {
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
        }
        catch (Exception)
        {
            throw new ReturnStatusException(StatusCode.AlreadyExists, nameof(StatusCode.AlreadyExists));
            //Console.WriteLine($"Error writing to remote path: {ex.Message}");
            // Handle other exceptions
        }
    }

    public void WriteFileToRemotePath(byte[] fileBytes, string server, string domain, string username, string password, string path, string fileName)
    {
        var remoteFilePath = $@"\\{server}\{path}\{fileName}";

        _ = new NetworkCredential(username, password, domain);


        try
        {
            using var fileStream = new FileStream(remoteFilePath + "", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fileStream.Write(fileBytes, 0, fileBytes.Length);
        }
        catch (UnauthorizedAccessException)
        {
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
        }
        catch (Exception)
        {
            throw new ReturnStatusException(StatusCode.AlreadyExists, nameof(StatusCode.AlreadyExists));
            //Console.WriteLine($"Error writing to remote path: {ex.Message}");
            // Handle other exceptions
        }
    }
 
    public static void DeleteFileFromRemote(string filePath)
    {
        try
        {
            // Attempt to delete the file
            File.Delete(filePath);
        }
        catch (FileNotFoundException)
        {
            // Handle the case where the file does not exist
            throw new ReturnStatusException(StatusCode.NotFound, nameof(StatusCode.NotFound));
        }
        catch (UnauthorizedAccessException)
        {
            // Handle the case where there's no permission to delete the file
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
        }

        catch (Exception ex)
        {
            // Handle any other unexpected exceptions
            throw new ReturnStatusException(StatusCode.NotFound, ex.Message);
        }
    }

    
    public async Task WriteFileAsync(byte[] fileBytes, string path, string fileName)
    {
        var remoteFilePath = $@"\\{path}\{fileName}";
 
        try
        {
            await using var fileStream = new FileStream(remoteFilePath + "", FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
            await fileStream.WriteAsync(fileBytes);
        }
        catch (UnauthorizedAccessException)
        {
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
        }
        catch (Exception)
        {
            throw new ReturnStatusException(StatusCode.AlreadyExists, nameof(StatusCode.AlreadyExists));
            //Console.WriteLine($"Error writing to remote path: {ex.Message}");
            // Handle other exceptions
        }
    }

    public void WriteFile(byte[] fileBytes, string path, string fileName)
    {
        var remoteFilePath = $@"\\{path}\{fileName}";
 
        try
        {
            using var fileStream = new FileStream(remoteFilePath + "", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fileStream.Write(fileBytes, 0, fileBytes.Length);
        }
        catch (UnauthorizedAccessException)
        {
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
        }
        catch (Exception)
        {
            throw new ReturnStatusException(StatusCode.AlreadyExists, nameof(StatusCode.AlreadyExists));
            //Console.WriteLine($"Error writing to remote path: {ex.Message}");
            // Handle other exceptions
        }
    }
 
    public static void DeleteFile(string filePath)
    {
        try
        {
            // Attempt to delete the file
            File.Delete(filePath);
        }
        catch (FileNotFoundException)
        {
            // Handle the case where the file does not exist
            throw new ReturnStatusException(StatusCode.NotFound, nameof(StatusCode.NotFound));
        }
        catch (UnauthorizedAccessException)
        {
            // Handle the case where there's no permission to delete the file
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
        }

        catch (Exception ex)
        {
            // Handle any other unexpected exceptions
            throw new ReturnStatusException(StatusCode.NotFound, ex.Message);
        }
    }
    
    public string ResolveDomain(string domain)
    {
        var host = Dns.GetHostEntry(domain);
        var hostIp = host.AddressList.Last().ToString();
        return hostIp;
    }
}