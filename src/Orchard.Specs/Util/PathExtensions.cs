﻿using System.IO;
using Path = Bleroy.FluentPath.Path;

namespace Orchard.Specs.Util {
    public static class PathExtensions {
        public static Path GetRelativePath(this Path path, Path basePath) {
            if (path.Equals(basePath))
                return Path.Get(".");

            if (path.Parent.Equals(basePath))
                return path.FileName;

            return path.Parent.GetRelativePath(basePath).Combine(path.FileName);
        }


        public static Path DeepCopy(this Path sourcePath, Path targetPath) {
            sourcePath
                .GetFiles("*", true /*recursive*/)
                .ForEach(file => FileCopy(sourcePath, targetPath, file));
            return sourcePath;
        }

        public static Path ShallowCopy(this Path sourcePath, string pattern, Path targetPath) {
            sourcePath
                .GetFiles(pattern, false /*recursive*/)
                .ForEach(file => FileCopy(sourcePath, targetPath, file));
            return sourcePath;
        }

        private static void FileCopy(Path sourcePath, Path targetPath, Path sourceFile) {
            var targetFile = targetPath.Combine(sourceFile.GetRelativePath(sourcePath));
            targetFile.Parent.CreateDirectory();
            File.Copy(sourceFile, targetFile, true /*overwrite*/);
        }
    }
}
