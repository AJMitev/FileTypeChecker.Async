namespace FileTypeChecker.Async.Tests
{
    using NUnit.Framework;
    using System.IO;
    using System.Threading.Tasks;
    using FileTypeChecker.Async.Extensions;
    using FileTypeChecker.Types;

    [TestFixture]
    public class StreamExtensionsAsyncTests
    {
        [Test]
        public async Task Is_ShouldReturnTrueIfTheTypesMatch()
        {
            using var fileStream = File.OpenRead("./files/test.bmp");
            var expected = true;
            var actual = await fileStream.IsAsync<Bitmap>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task Is_ShouldReturnFalseIfTypesDidNotMatch()
        {
            using var fileStream = File.OpenRead("./files/test.bmp");
            var expected = false;
            var actual = await fileStream.IsAsync<Gzip>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task IsImage_ShouldReturnTrueIfTheTypesMatch()
        {
            using var fileStream = File.OpenRead("./files/test.jpg");
            var expected = true;
            var actual = await fileStream.IsImageAsync();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task IsImage_ShouldReturnFalseIfTypesDidNotMatch()
        {
            using var fileStream = File.OpenRead("./files/test.exe");
            var expected = false;
            var actual = await fileStream.IsImageAsync();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task IsArchive_ShouldReturnTrueIfTheTypesMatch()
        {
            using var fileStream = File.OpenRead("./files/test.zip");
            var expected = true;
            var actual = await fileStream.IsArchiveAsync();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task IsArchive_ShouldReturnFalseIfTypesDidNotMatch()
        {
            using var fileStream = File.OpenRead("./files/test.bmp");
            var expected = false;
            var actual = await fileStream.IsArchiveAsync();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task IsExecutable_ShouldReturnTrueIfTheTypesMatch()
        {
            using var fileStream = File.OpenRead("./files/test.exe");
            var expected = true;
            var actual = await fileStream.IsExecutableAsync();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task IsExecutable_ShouldReturnFalseIfTypesDidNotMatch()
        {
            using var fileStream = File.OpenRead("./files/test.bmp");
            var expected = false;
            var actual = await fileStream.IsExecutableAsync();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task IsDocumentAsync_ShouldReturnFalseIfTypesDidNotMatch()
        {
            using var fileStream = File.OpenRead("./files/test.bmp");
            var expected = false;
            var actual = await fileStream.IsDocumentAsync();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task IsDocumentAsync_ShouldReturnTrueIfTheTypesMatch()
        {
            using var fileStream = File.OpenRead("./files/test.doc");
            var expected = true;
            var actual = await fileStream.IsDocumentAsync();

            Assert.AreEqual(expected, actual);
        }
    }
}