using System;
using System.Collections.Generic;
using System.Linq;

namespace Settings
{
    // The motivation for this class and the FileExtension class is to make it work like a SET, and handle things like Contains(string) robustly against
    // case insensitivity. The only real issue is the difficulty in creating (such as for default) but I don't see that happening often, if ever.
    // But I really hate this name, and want a much shorter one if we can think of it.
    public class FileExtensionSet : HashSet<FileExtension>
    {
        public FileExtensionSet()
        {
        }

        public FileExtensionSet(string settings)
            : this(settings, new[] { ';' })
        {
        }

        public FileExtensionSet(string settings, char[] separators)
            : this(settings.Split(separators, StringSplitOptions.RemoveEmptyEntries))
        {
        }

        public FileExtensionSet(IEnumerable<string> extensions)
            : base(extensions.Select(extension => new FileExtension(extension)))
        {
        }

        public FileExtensionSet(IEnumerable<FileExtension> extensions)
            : base(extensions)
        {
        }

        // Entire justification for inheriting HashSet<FileExtension>, as we can't override this if we used HashSet<string> and we'd like to inherit
        // the ISet<> interface as easily as we reasonably can. Alternatively, we could do the heavy lifting so we could avoid creating the
        // FileExtension class.
        public bool Contains(string item)
        {
            return Contains(new FileExtension(item));
        }

        public override string ToString()
        {
            return string.Join(";", this);
        }
    }

    public sealed class FileExtension
    {
        private readonly string extension;

        public FileExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentNullException("extension");
            }

            this.extension = "." + extension.Trim().TrimStart('.').ToLower();
        }

        public override bool Equals(object obj)
        {
            var p = obj as FileExtension;
            if (p == null)
            {
                return false;
            }

            // Constructor ensures this is formatted similarly and lowercase
            return p.extension == extension;
        }

        public override int GetHashCode()
        {
            return extension.GetHashCode();
        }

        public override string ToString()
        {
            return extension;
        }
    }
}
