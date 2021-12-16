namespace FileTypeChecker.Async
{
    using FileTypeChecker;
    using FileTypeChecker.Abstracts;
    using FileTypeChecker.Common;
    using FileTypeChecker.Async.Extensions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides you a method for file type validation.
    /// </summary>
    public class FileTypeValidator : FileTypeChecker.FileTypeValidator
    {
        private const string EmptyCollectionErrorMessage = "Can't search in collection with no items!";
     
        /// <summary>
        /// Checks that the particular type is supported.
        /// </summary>
        /// <param name="fileContent">File to check as stream.</param>
        /// <returns>If current type is supported</returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ObjectDisposedException"></exception>
        public static async Task<bool> IsTypeRecognizableAsync(Stream fileContent)
        {
            DataValidator.ThrowIfNull(fileContent, nameof(Stream));

            return (await FileTypes
                .ToAsyncEnumerable()
                .WhereAwait(async x => await x.DoesMatchWithAsync(fileContent))
                .ToListAsync())
                .Any();
        }

        /// <summary>
        /// Get details about current file type.
        /// </summary>
        /// <param name="fileContent">File to check as stream.</param>
        /// <returns>Instance of <see cref="IFileType}"/> type. If the type is not recognized returns <see cref="null}"/></returns>
        /// /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static async Task<IFileType?> GetFileTypeAsync(Stream fileContent)
        {
            DataValidator.ThrowIfNull(fileContent, nameof(Stream));

            return await GetBestMatchAsync(fileContent);
        }

        /// <summary>
        /// Validates that the file is from certain type
        /// </summary>
        /// <typeparam name="T">Type that implements FileType</typeparam>
        /// <param name="fileContent">File as stream</param>
        /// <returns>True if file match the desired type otherwise returns false.</returns>
        public static Task<bool> IsAsync<T>(Stream fileContent) where T : FileType, IFileType, new()
           => fileContent.IsAsync<T>();

        /// <summary>
        /// Validates that the current file is image.
        /// </summary>
        /// <param name="fileContent">File to check as stream.</param>
        /// <returns>Returns true if the provided file is image otherwise returns false. Supported image types are: Bitmap, JPEG, GIF and PNG.</returns>
        public static Task<bool> IsImageAsync(Stream fileContent)
           => fileContent.IsImageAsync();

        /// <summary>
        /// Validates that the current file is archive.
        /// </summary>
        /// <param name="fileContent"File to check as stream.></param>
        /// <returns>Returns true if the provided file is archive otherwise returns false. Supported archive types are: Extensible archive, Gzip, Rar, 7Zip, Tar and Zip.</returns>
        public static Task<bool> IsArchiveAsync(Stream fileContent)
            => fileContent.IsArchiveAsync();

        internal static async Task<IFileType?> GetBestMatchAsync(Stream fileContent)
        {
            var matches = await FileTypes
                .ToAsyncEnumerable()
                .WhereAwait(async x => await x.DoesMatchWithAsync(fileContent))
                .ToListAsync();

            return ReturnBestMatch(fileContent, matches);
        }

        private static IFileType? FindBestMatch(Stream fileContent, IEnumerable<IFileType> result)
        {
            try
            {
                var scoreboard = CreateScoreboard(fileContent, result);
                return FindMaxScore(scoreboard);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        private static IEnumerable<MatchScore> CreateScoreboard(Stream fileContent, IEnumerable<IFileType> result)
        {
            var scoreboard = new List<MatchScore>();

            for (int typeIndex = 0; typeIndex < result.Count(); typeIndex++)
            {
                var currentType = result.ElementAt(typeIndex) as FileType;
                var currentScore = currentType.GetMatchingNumber(fileContent);

                scoreboard.Add(new MatchScore(currentType, currentScore));
            }

            return scoreboard;
        }

        private static IFileType FindMaxScore(IEnumerable<MatchScore> matches)
        {
            if (matches.Count() == 0)
                throw new InvalidOperationException(EmptyCollectionErrorMessage);

            int maxScore = int.MinValue;
            IFileType? bestMatch = null;

            foreach (var match in matches)
            {
                if (!(match.Score > maxScore))
                    continue;

                maxScore = match.Score;
                bestMatch = match.Type;
            }

            return bestMatch;
        }

        private static IFileType? ReturnBestMatch(Stream fileContent, IEnumerable<IFileType> matches)
            => matches.Count() == 0
                ? null
                : matches.Count() == 1
                    ? matches.First()
                    : FindBestMatch(fileContent, matches);
    }
}