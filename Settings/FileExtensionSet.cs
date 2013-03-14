using System;
using System.Collections.Generic;
using System.Linq;

namespace Settings
{
    /// <summary>
    /// A HashSet{T} of FileExtensions, allowing simple checking of the existence of a provided file
    /// extension within the container, as well as enhancements for using string extensions directly
    /// in a robust manner.
    /// </summary>
    /// <remarks>
    /// The motivation for this class and the FileExtension class is to make it work like a SET, and 
    /// handle things like Contains(string) robustly against case insensitivity. The only real issue 
    /// is the difficulty in creating (such as for default) but I don't see that happening often, if 
    /// ever. I think this could be better named however.
    /// </remarks>
    public sealed class FileExtensionSet : HashSet<FileExtension>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileExtensionSet"/> class.
        /// </summary>
        public FileExtensionSet()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExtensionSet"/> class from the provided
        /// string settings.
        /// </summary>
        /// <param name="settings">The string settings to decode.</param>
        public FileExtensionSet(string settings)
            : this(settings, ';')
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExtensionSet"/> class from the provided
        /// string settings.
        /// </summary>
        /// <param name="settings">The string settings to decode.</param>
        /// <param name="separators">An array of separator characters.</param>
        public FileExtensionSet(string settings, params char[] separators)
            : this(settings.Split(separators, StringSplitOptions.RemoveEmptyEntries))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExtensionSet"/> class from the provided
        /// extensions array.
        /// </summary>
        /// <param name="extensions">An array of string extensions.</param>
        public FileExtensionSet(params string[] extensions)
            : base(extensions.Select(extension => new FileExtension(extension)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExtensionSet"/> class from the provided
        /// extensions enumerable.
        /// </summary>
        /// <param name="extensions">An enumerable of FileExtensions.</param>
        public FileExtensionSet(IEnumerable<FileExtension> extensions)
            : base(extensions)
        {
        }

        /// <summary>
        /// Returns a string representation of this FileExtensionSet instance.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            return string.Join(";", this);
        }
    }
}
