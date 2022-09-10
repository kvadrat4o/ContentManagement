using OneBitSoftware.Utilities;

namespace ContentManagement.Core.Infrastructure
{
    /// <summary>
    /// An interface defining the structure of a component responsible for the execution of operations related to file content management.
    /// </summary>
    /// <typeparam name="TKey">The type of the unique identifier of the contained entities within our databases.</typeparam>
    public interface IContentManager<in TKey> where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// Creates and persists a file content.
        /// </summary>
        /// <param name="id">The <typeparamref name="TKey"/> unique identifier that should be associated with the new content.</param>
        /// <param name="fileContent">The <see cref="StreamInfo"/> representing the file content to be stored.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous state of the operation. It wraps inside an <see cref="OperationResult"/> of the Create operation.
        /// </returns>
        Task<OperationResult> StoreAsync(TKey id, StreamInfo fileContent, CancellationToken cancellationToken);

        /// <summary>
        /// Use this method to validate that a file content entity with the requested <paramref name="id"/> exists.
        /// </summary>
        /// <param name="id">The unique identifier of the requested file content.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>Returns a <see cref="Task"/> that represents the asynchronous operation. It contains an <see cref="OperationResult"/> associated with this operation.</returns>
        Task<OperationResult<bool>> ExistsAsync(TKey id, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a <see cref="StreamInfo"/> object representing the file content identified by the provided <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The unique Id of the entity, of type <typeparamref name="TKey"/>, to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>Returns a <see cref="Task"/> that represents the asynchronous operation. It contains an <see cref="OperationResult"/> associated with this operation.</returns>
        Task<OperationResult<StreamInfo>> GetAsync(TKey id, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a byte array of a file content entity.
        /// </summary>
        /// <param name="id">The unique Id of the entity, of type <typeparamref name="TKey"/>, to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A <see cref="Task"/> of <see cref="OperationResult"/> representing the byte array.</returns>
        Task<OperationResult<byte[]>> GetBytesAsync(TKey id, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a file content.
        /// </summary>
        /// <param name="id">The unique Id of the file content entity of type <typeparamref name="TKey"/>.</param>
        /// <param name="fileContent">The a <see cref="StreamInfo"/> object representing the file content to be updated.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous state of the operation. It wraps inside an <see cref="OperationResult"/> of the Update operation.
        /// </returns>
        Task<OperationResult> UpdateAsync(TKey id, StreamInfo fileContent, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a file content entity identified by the provided <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The unique identifier of the file content entity to be deleted.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>Returns a <see cref="Task"/> that represents the asynchronous operation. It contains an <see cref="OperationResult"/> associated with this operation.</returns>
        Task<OperationResult> DeleteAsync(TKey id, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the hash of a file content entity.
        /// </summary>
        /// <param name="id">The unique Id of the file, of type <typeparamref name="TKey"/>, to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        /// <returns>Returns a <see cref="Task"/> that represents the asynchronous operation. It contains an <see cref="OperationResult"/> associated with this operation.</returns>
        Task<OperationResult<string>> GetHashAsync(TKey id, CancellationToken cancellationToken);
    }
}
