
namespace ContentManagement.Core
{
    /// <summary>
    /// A class providing length information about a <see cref="Stream"/>.
    /// When we retrieve files from network locations, streams can be a network stream with no length.
    /// </summary>
    public class StreamInfo : IEquatable<StreamInfo>
    {
        #region Properties

        /// <summary>
        /// Gets or sets Length
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Gets or sets stream
        /// </summary>
        public Stream Stream { get; set; }

        /// <summary>
        /// Gets the length of the stream info as an unsigned integer.
        /// If for some reason the current instance's <see cref="Length"/> is lower than 0, this value will be Zero.
        /// </summary>
        public ulong UnsignedLength => Length < 0 ? 0 : (ulong)Length;

        #endregion

        #region Methods

        public bool Equals(StreamInfo? other)
        {
            if (Length == other.Length &&
                UnsignedLength == other.UnsignedLength)
                return true;

            return false;
        }

        #endregion
    }
}
