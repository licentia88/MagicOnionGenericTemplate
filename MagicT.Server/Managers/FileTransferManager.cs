using System.Net;
using Benutomo;
using Grpc.Core;
using MagicOnion;

namespace MagicT.Server.Managers;

/// <summary>
/// Manages file transfer operations including writing and deleting files on remote paths.
/// </summary>
[AutomaticDisposeImpl]
public partial class FileTransferManager : IDisposable
{
    
    ~FileTransferManager()
    {
        Dispose(false);
        // GC.WaitForPendingFinalizers();
    }
    
    /// <summary>
    /// Writes a file to a remote path asynchronously.
    /// </summary>
    /// <param name="fileBytes">The file bytes to write.</param>
    /// <param name="server">The server address.</param>
    /// <param name="domain">The domain name.</param>
    /// <param name="username">The username for authentication.</param>
    /// <param name="password">The password for authentication.</param>
    /// <param name="path">The remote path to write the file to.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ReturnStatusException">Thrown when the operation is unauthorized or the file already exists.</exception>
    public async Task WriteFileToRemotePathAsync(byte[] fileBytes, string server, string domain, string username, string password, string path, string fileName)
    {
        var remoteFilePath = GetRemoteFilePath(server, path, fileName);
        _ = new NetworkCredential(username, password, domain);

        try
        {
            await using var fileStream = new FileStream(remoteFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
            await fileStream.WriteAsync(fileBytes);
        }
        catch (UnauthorizedAccessException)
        {
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
        }
        catch (Exception)
        {
            throw new ReturnStatusException(StatusCode.AlreadyExists, nameof(StatusCode.AlreadyExists));
        }
    }

    /// <summary>
    /// Writes a file to a remote path.
    /// </summary>
    /// <param name="fileBytes">The file bytes to write.</param>
    /// <param name="server">The server address.</param>
    /// <param name="domain">The domain name.</param>
    /// <param name="username">The username for authentication.</param>
    /// <param name="password">The password for authentication.</param>
    /// <param name="path">The remote path to write the file to.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <exception cref="ReturnStatusException">Thrown when the operation is unauthorized or the file already exists.</exception>
    public void WriteFileToRemotePath(byte[] fileBytes, string server, string domain, string username, string password, string path, string fileName)
    {
        var remoteFilePath = GetRemoteFilePath(server, path, fileName);
        _ = new NetworkCredential(username, password, domain);

        try
        {
            using var fileStream = new FileStream(remoteFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fileStream.Write(fileBytes, 0, fileBytes.Length);
        }
        catch (UnauthorizedAccessException)
        {
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
        }
        catch (Exception)
        {
            throw new ReturnStatusException(StatusCode.AlreadyExists, nameof(StatusCode.AlreadyExists));
        }
    }

    /// <summary>
    /// Deletes a file from a remote path.
    /// </summary>
    /// <param name="filePath">The path of the file to delete.</param>
    /// <exception cref="ReturnStatusException">Thrown when the file is not found or the operation is unauthorized.</exception>
    public static void DeleteFileFromRemote(string filePath)
    {
        try
        {
            File.Delete(filePath);
        }
        catch (FileNotFoundException)
        {
            throw new ReturnStatusException(StatusCode.NotFound, nameof(StatusCode.NotFound));
        }
        catch (UnauthorizedAccessException)
        {
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
        }
        catch (Exception ex)
        {
            throw new ReturnStatusException(StatusCode.NotFound, ex.Message);
        }
    }

    /// <summary>
    /// Writes a file asynchronously.
    /// </summary>
    /// <param name="fileBytes">The file bytes to write.</param>
    /// <param name="path">The path to write the file to.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ReturnStatusException">Thrown when the operation is unauthorized or the file already exists.</exception>
    public async Task WriteFileAsync(byte[] fileBytes, string path, string fileName)
    {
        var remoteFilePath = GetRemoteFilePath(path, fileName);

        try
        {
            await using var fileStream = new FileStream(remoteFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
            await fileStream.WriteAsync(fileBytes);
        }
        catch (UnauthorizedAccessException)
        {
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
        }
        catch (Exception)
        {
            throw new ReturnStatusException(StatusCode.AlreadyExists, nameof(StatusCode.AlreadyExists));
        }
    }

    /// <summary>
    /// Writes a file.
    /// </summary>
    /// <param name="fileBytes">The file bytes to write.</param>
    /// <param name="path">The path to write the file to.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <exception cref="ReturnStatusException">Thrown when the operation is unauthorized or the file already exists.</exception>
    public void WriteFile(byte[] fileBytes, string path, string fileName)
    {
        var remoteFilePath = GetRemoteFilePath(path, fileName);

        try
        {
            using var fileStream = new FileStream(remoteFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fileStream.Write(fileBytes, 0, fileBytes.Length);
        }
        catch (UnauthorizedAccessException)
        {
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
        }
        catch (Exception)
        {
            throw new ReturnStatusException(StatusCode.AlreadyExists, nameof(StatusCode.AlreadyExists));
        }
    }

    /// <summary>
    /// Deletes a file.
    /// </summary>
    /// <param name="filePath">The path of the file to delete.</param>
    /// <exception cref="ReturnStatusException">Thrown when the file is not found or the operation is unauthorized.</exception>
    public static void DeleteFile(string filePath)
    {
        try
        {
            File.Delete(filePath);
        }
        catch (FileNotFoundException)
        {
            throw new ReturnStatusException(StatusCode.NotFound, nameof(StatusCode.NotFound));
        }
        catch (UnauthorizedAccessException)
        {
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
        }
        catch (Exception ex)
        {
            throw new ReturnStatusException(StatusCode.NotFound, ex.Message);
        }
    }

    /// <summary>
    /// Resolves the domain to an IP address.
    /// </summary>
    /// <param name="domain">The domain name to resolve.</param>
    /// <returns>The IP address of the domain.</returns>
    public string ResolveDomain(string domain)
    {
        var host = Dns.GetHostEntry(domain);
        var hostIp = host.AddressList.Last().ToString();
        return hostIp;
    }

    /// <summary>
    /// Constructs the remote file path.
    /// </summary>
    /// <param name="server">The server address.</param>
    /// <param name="path">The remote path.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The constructed remote file path.</returns>
    private string GetRemoteFilePath(string server, string path, string fileName)
    {
        return $@"\\{server}\{path}\{fileName}";
    }

    /// <summary>
    /// Constructs the remote file path.
    /// </summary>
    /// <param name="path">The remote path.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The constructed remote file path.</returns>
    private string GetRemoteFilePath(string path, string fileName)
    {
        return $@"\\{path}\{fileName}";
    }
}