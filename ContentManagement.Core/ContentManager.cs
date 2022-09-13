using OneBitSoftware.Utilities;
using System.IO.Abstractions;
using System.Security.AccessControl;
using System.Security.Cryptography;

namespace ContentManagement.Core
{
    /// <summary>
    /// An implementation of a component responsible for the execution of operations related to file content management.
    /// </summary>
    /// <typeparam name="int">The type of the unique identifier of the contained entities within our databases. - int</typeparam>
    public class ContentManager : IContentManager<Guid>
    {
        #region Fields

        private readonly ConfigurationOptions _configurationOptions;
        private readonly IFileSystem _fileSystem;

        #endregion

        #region Ctor

        public ContentManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _configurationOptions = new ConfigurationOptions();
        }
        public ContentManager(ConfigurationOptions configurationOptions) : this(fileSystem: new FileSystem())
        {
            _configurationOptions = configurationOptions;
        }

        public ContentManager(ConfigurationOptions configurationOptions, IFileSystem fileSystem) : this(fileSystem)
        {
            _configurationOptions = configurationOptions;
        }

        #endregion

        #region Utilities

        private static bool CanExecute(IDirectoryInfo fileTree, FileSystemRights fileSystemRights)
        {
            try
            {
                var allow = false;
                var deny = false;
                var accessControlList = fileTree.GetAccessControl();
                if (accessControlList == null)
                    return false;

                (allow, deny) = CheckRights(allow, deny, accessControlList, fileSystemRights);

                return allow && !deny;
            }
            catch (UnauthorizedAccessException ex)
            {
                return false;
            }
        }

        private static (bool, bool) CheckRights(bool allow, bool deny, DirectorySecurity accessControlList, FileSystemRights fileSystemRights)
        {
            //get the access rules that pertain to a valid SID/NTAccount.
            var accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            if (accessRules == null || accessRules.Count == 0)
                return (!allow, deny);

            //we want to go over these rules to ensure a valid SID has access
            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((fileSystemRights & rule.FileSystemRights) != fileSystemRights) 
                    continue;

                if (rule.AccessControlType == AccessControlType.Allow)
                    allow = true;
                else if (rule.AccessControlType == AccessControlType.Deny)
                    deny = true;
            }

            return (allow, deny);
        }

        private static string BytesToStringConverted(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            using (var streamReader = new StreamReader(stream))
                return streamReader.ReadToEnd();
        }

        private string GetHashFromStreamAsync(StreamInfo? content)
        {
            byte[] emptyBuffer = new byte[0];
            var hasher = SHA256.Create();
            hasher.Initialize();

            var buffer = new byte[content.Stream.Length];
            int readBytes;
            while ((readBytes = content.Stream.Read(buffer, 0, buffer.Length)) > 0)
                hasher.TransformBlock(buffer, 0, readBytes, buffer, 0);

            hasher.TransformFinalBlock(emptyBuffer, 0, 0);
            return BytesToStringConverted(hasher.Hash);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates and persists a file content.
        /// </summary>
        /// <param name="id">The <typeparamref name="Guid"/> unique identifier that should be associated with the new content.</param>
        /// <param name="fileContent">The <see cref="StreamInfo"/> representing the file content to be stored.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous state of the operation. It wraps inside an <see cref="OperationResult"/> of the Create operation.
        /// </returns>
        public async Task<OperationResult> StoreAsync(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            var result = new OperationResult();

            try
            {
                var directory = _fileSystem.DirectoryInfo.FromDirectoryName(_configurationOptions.FileServerPath);

                if (!directory.Exists)
                    directory = _fileSystem.Directory.CreateDirectory(_configurationOptions.FileServerPath);

                var canWrite = CanExecute(directory, FileSystemRights.Write);

                if (canWrite)
                {
                    var fileExists = _fileSystem.Directory.EnumerateFiles(_configurationOptions.FileServerPath).FirstOrDefault(f => f.Contains(id.ToString()));
                    if (!string.IsNullOrWhiteSpace(fileExists))
                        result.AppendError("File alreadyExists!");
                    else
                    {
                        using (var fileStream = _fileSystem.FileStream.Create(directory.FullName + "\\" + id.ToString(), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                            fileContent.Stream.CopyTo(fileStream);
                        result.AddSuccessMessage("Successfully created file and added file content!");
                    }
                }
                else
                    result.AppendError("User does not have rights to perform this action!");
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// Use this method to validate that a file content entity with the requested <paramref name="id"/> exists.
        /// </summary>
        /// <param name="id">The unique identifier of the requested file content.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>Returns a <see cref="Task"/> that represents the asynchronous operation. It contains an <see cref="OperationResult"/> associated with this operation.</returns>
        public async Task<OperationResult<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            var exists = false;

            try
            {
                var directory = _fileSystem.Directory.FileSystem.DirectoryInfo.FromDirectoryName(_configurationOptions.FileServerPath);

                var hasReadAccess = CanExecute(directory, FileSystemRights.Read);

                if (hasReadAccess)
                {
                    var fileExists = _fileSystem.Directory.EnumerateFiles(_configurationOptions.FileServerPath).FirstOrDefault(f => f.Contains(id.ToString())) is not null;

                    exists = directory.Exists && fileExists && hasReadAccess;
                }
                else
                    throw new AccessViolationException("User does not have rights to perform such actions!");
            }
            catch (Exception ex)
            {
                throw;
            }

            return new OperationResult<bool>(exists);
        }

        /// <summary>
        /// Retrieves a <see cref="StreamInfo"/> object representing the file content identified by the provided <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The unique Id of the entity, of type <typeparamref name="Guid"/>, to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>Returns a <see cref="Task"/> that represents the asynchronous operation. It contains an <see cref="OperationResult"/> associated with this operation.</returns>
        public async Task<OperationResult<StreamInfo>> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            var result = new OperationResult<StreamInfo>(new StreamInfo() { Stream = new MemoryStream() });

            var fileBytes = (await GetBytesAsync(id, cancellationToken)).ResultObject;

            if (fileBytes is not null)
            {
                using var stream = new MemoryStream(fileBytes);
                    result.ResultObject.Stream = new MemoryStream(fileBytes);
            }

            return result;
        }

        /// <summary>
        /// Retrieves a byte array of a file content entity.
        /// </summary>
        /// <param name="id">The unique Id of the entity, of type <typeparamref name="Guid"/>, to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A <see cref="Task"/> of <see cref="OperationResult"/> representing the byte array.</returns>
        public async Task<OperationResult<byte[]>> GetBytesAsync(Guid id, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            byte[] fileData = null;

            var exists = (await ExistsAsync(id, cancellationToken)).ResultObject;

            if (exists)
            {
                var directory = _fileSystem.DirectoryInfo.FromDirectoryName(_configurationOptions.FileServerPath);
                var streamInfo = _fileSystem.FileInfo.FromFileName(id.ToString());

                using (var fs = _fileSystem.FileStream.Create(directory.FullName + "\\" + id.ToString(), FileMode.Open, FileAccess.ReadWrite))
                    using (var binaryReader = new BinaryReader(fs))
                        fileData = binaryReader.ReadBytes((int)fs.Length);
            }

            return new OperationResult<byte[]>(fileData);
        }

        /// <summary>
        /// Updates a file content.
        /// </summary>
        /// <param name="id">The unique Id of the file content entity of type <typeparamref name="Guid"/>.</param>
        /// <param name="fileContent">The a <see cref="StreamInfo"/> object representing the file content to be updated.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous state of the operation. It wraps inside an <see cref="OperationResult"/> of the Update operation.
        /// </returns>
        public async Task<OperationResult> UpdateAsync(Guid id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            var result = new OperationResult();

            try
            {

                //var fileTree = new DirectoryInfo(_configurationOptions.FileServerPath);
                var directory = _fileSystem.DirectoryInfo.FromDirectoryName(_configurationOptions.FileServerPath);

                var canWrite = CanExecute(directory, FileSystemRights.Write);

                if (canWrite)
                {
                    var fileExists = _fileSystem.Directory.EnumerateFiles(_configurationOptions.FileServerPath).FirstOrDefault(f => f.Contains(id.ToString()));
                    if (!string.IsNullOrWhiteSpace(fileExists))
                    {
                        var file = _fileSystem.FileInfo.FromFileName(id.ToString());
                        using (var fileStream = _fileSystem.FileStream.Create(file.Name, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                            fileContent.Stream.CopyTo(fileStream);
                        result.AddSuccessMessage("Successfully updated file content!");
                    }
                    else
                        result.AppendError("Provided path is not valid!");
                }
                else
                    result.AppendError("User does not have rights to perform this action!");
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// Deletes a file content entity identified by the provided <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The unique identifier of the file content entity to be deleted.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>Returns a <see cref="Task"/> that represents the asynchronous operation. It contains an <see cref="OperationResult"/> associated with this operation.</returns>
        public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            var result = new OperationResult();

            try
            {
                //var fileTree = new DirectoryInfo(_configurationOptions.FileServerPath);
                var directory = _fileSystem.DirectoryInfo.FromDirectoryName(_configurationOptions.FileServerPath);

                var canWrite = CanExecute(directory, FileSystemRights.Write);

                if (canWrite)
                {
                    var fileExists = _fileSystem.Directory.EnumerateFiles(_configurationOptions.FileServerPath).FirstOrDefault(f => f.Contains(id.ToString())) is not null;
                    if (fileExists)
                    {
                        var file = _fileSystem.FileInfo.FromFileName(id.ToString());

                        using (var fs = _fileSystem.FileStream.Create(directory.FullName + "\\" + id.ToString(), FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose))
                            result.AddSuccessMessage("Successfully removed file content!");
                    }
                    else
                        result.AppendError("Provided path is not valid!");
                }
                else
                    result.AppendError("User does not have rights to perform this action!");
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// Retrieves the hash of a file content entity.
        /// </summary>
        /// <param name="id">The unique Id of the file, of type <typeparamref name="TKey"/>, to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>Returns a <see cref="Task"/> that represents the asynchronous operation. It contains an <see cref="OperationResult"/> associated with this operation.</returns>
        public async Task<OperationResult<string>> GetHashAsync(Guid id, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            var hash = string.Empty;
            var content = (await GetAsync(id, cancellationToken)).ResultObject;

            if (content.Stream.Length > 0)
                hash = GetHashFromStreamAsync(content);

            return new OperationResult<string>(hash);
        }

        #endregion
    }
}
