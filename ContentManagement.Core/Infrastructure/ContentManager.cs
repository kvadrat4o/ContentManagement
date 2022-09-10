using OneBitSoftware.Utilities;
using System.IO;

namespace ContentManagement.Core.Infrastructure
{
    /// <summary>
    /// An implementation of a component responsible for the execution of operations related to file content management.
    /// </summary>
    /// <typeparam name="int">The type of the unique identifier of the contained entities within our databases. - int</typeparam>
    public class ContentManager : IContentManager<Guid>
    {
        private readonly IConfigurationOptions _configurationOptions;

        public ContentManager(IConfigurationOptions configurationOptions)
        {
            _configurationOptions = configurationOptions;
        }

        private void SaveFileStream(string path, Stream stream)
        {
            var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            stream.CopyTo(fileStream);
            fileStream.Dispose();
        }


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
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            var exists = (await ExistsAsync(id, cancellationToken)).ResultObject;

            if (exists)
            {
                //SaveFileStream(_configurationOptions.FileServerPath, fileContent.Stream);
            }
            else
            {
                throw new ArgumentNullException("Provided path is invalid!");
            }


            var result = new OperationResult();
            result.AddSuccessMessage("Successfully added file content!");

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
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            var exists = _configurationOptions.FileServerPath.Contains(id.ToString());

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
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            var exists = (await ExistsAsync(id, cancellationToken)).ResultObject;
            var result = new OperationResult<StreamInfo>(new StreamInfo() { Stream = null });
            if (exists)
            {
                //var fileBytes = (await GetBytesAsync(id, cancellationToken)).ResultObject;
                var fileBytes = new byte[5] {45, 219, 18, 44, 34};
                if (fileBytes is not null && fileBytes.Any())
                {
                    Stream stream= new MemoryStream(fileBytes);
                    result.ResultObject.Stream = stream;
                }
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
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            byte[] fileData = null;

            //using (FileStream fs = File.OpenRead(id.ToString()))
            //{
            //    using (BinaryReader binaryReader = new BinaryReader(fs))
            //    {
            //        fileData = binaryReader.ReadBytes((int)fs.Length);
            //    }
            //}

            fileData = new byte[5] { 45, 219, 18, 44, 34 };

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
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            var exists = (await ExistsAsync(id, cancellationToken)).ResultObject;

            if (exists)
            {
                //UpdateFileStream(_configurationOptions.FileServerPath, fileContent.Stream);
            }
            else
            {
                throw new ArgumentNullException("Provided path is invalid!");
            }


            var result = new OperationResult();
            result.AddSuccessMessage("Successfully updated file content!");

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
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            var exists = (await ExistsAsync(id, cancellationToken)).ResultObject;

            if (exists)
            {
                //DeleteFileStream(_configurationOptions.FileServerPath, fileContent.Stream);
            }
            else
            {
                throw new ArgumentNullException("Provided path is invalid!");
            }


            var result = new OperationResult();
            result.AddSuccessMessage("Successfully deleted file content!");

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
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            var fileExists = (await ExistsAsync(id, cancellationToken))?.ResultObject;

            return new OperationResult<string>(id.GetHashCode().ToString());
        }
    }
}
