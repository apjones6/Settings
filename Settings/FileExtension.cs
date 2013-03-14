using System;

namespace Settings
{
    /// <summary>
    /// A class representing a single file extension, which allows simple equality checking in the
    /// face of case inequality and the existence of preceeding '.' characters.
    /// </summary>
    public sealed class FileExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileExtension"/> class with the provided
        /// file extension.
        /// </summary>
        /// <param name="extension">The file extension string.</param>
        public FileExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentNullException("extension");
            }

            Value = "." + extension.Trim().TrimStart('.').ToLower();
        }

        /// <summary>
        /// Gets the extension value, which is lowercase and includes a preceeding '.' character.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Implicitly converts a string extension to a FileExtension instance if able.
        /// </summary>
        /// <param name="extension">The string extension to convert.</param>
        /// <returns>A FileExtension instance.</returns>
        public static implicit operator FileExtension(string extension)
        {
            return new FileExtension(extension);
        }

        /// <summary>
        /// Compares this instance to the provided object and indicates whether the two should be
        /// considered equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>A value of true if the instances are equal, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            var p = obj as FileExtension;
            if (p == null)
            {
                return false;
            }

            // Constructor ensures this is formatted similarly and lowercase
            return p.Value == Value;
        }

        /// <summary>
        /// Returns a hashcode value for this instance, to allow efficient placement and location 
        /// into hash based containers.
        /// </summary>
        /// <returns>An integer hashcode value.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of this FileExtension instance.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
