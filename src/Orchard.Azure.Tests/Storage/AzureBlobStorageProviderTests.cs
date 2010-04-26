﻿using System;
using System.Configuration;
using System.IO;
using NUnit.Framework;
using System.Diagnostics;
using Orchard.Azure.Storage;
using Microsoft.WindowsAzure;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using System.Text;

namespace Orchard.Azure.Tests.Storage {
    [TestFixture]
    public class AzureBlobStorageProviderTests {

        #region Azure Environment initialization

        private Process _dsService;

        [TestFixtureSetUp]
        public void FixtureSetup() {
            var count = Process.GetProcessesByName("DSService").Length;
            if (count == 0)
            {
                var start = new ProcessStartInfo
                                {
                                    Arguments = "/devstore:start",
                                    FileName =
                                        Path.Combine(ConfigurationManager.AppSettings["AzureSDK"], @"bin\csrun.exe")
                                };

                _dsService = new Process { StartInfo = start };
                _dsService.Start();
                _dsService.WaitForExit();
            }

            CloudStorageAccount devAccount;
            CloudStorageAccount.TryParse("UseDevelopmentStorage=true", out devAccount);

            _azureBlobStorageProvider = new AzureBlobStorageProvider("default", devAccount);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown() {

            if(_dsService != null)
                _dsService.Close();
        }

        [SetUp]
        public void Setup() {
            // ensure default container is empty before running any test
            DeleteAllBlobs();
        }
        
        #endregion

        private AzureBlobStorageProvider _azureBlobStorageProvider;

        private void DeleteAllBlobs() {
            foreach(var blob in _azureBlobStorageProvider.Container.ListBlobs()) {
                if(blob is CloudBlob) {
                    ((CloudBlob) blob).DeleteIfExists();
                }

                if (blob is CloudBlobDirectory) {
                    DeleteAllBlobs((CloudBlobDirectory)blob);
                }
            }
        }

        private static void DeleteAllBlobs(CloudBlobDirectory cloudBlobDirectory) {
            foreach (var blob in cloudBlobDirectory.ListBlobs()) {
                if (blob is CloudBlob) {
                    ((CloudBlob)blob).DeleteIfExists();
                }

                if (blob is CloudBlobDirectory) {
                    DeleteAllBlobs((CloudBlobDirectory)blob);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFileShouldOnlyAcceptRelativePath() {
            _azureBlobStorageProvider.CreateFile("foo.txt");
            _azureBlobStorageProvider.GetFile("/foot.txt");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFileThatDoesNotExistShouldThrow() {
            _azureBlobStorageProvider.GetFile("notexisting");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteFileThatDoesNotExistShouldThrow() {
            _azureBlobStorageProvider.DeleteFile("notexisting");
        }

        [Test]
        public void CreateFileShouldReturnCorrectStorageFile() {
            var storageFile = _azureBlobStorageProvider.CreateFile("foo.txt");

            Assert.AreEqual(".txt", storageFile.GetFileType());
            Assert.AreEqual("foo.txt", storageFile.GetName());
            Assert.That(storageFile.GetPath().EndsWith("/default/foo.txt"), Is.True);
            Assert.AreEqual(0, storageFile.GetSize());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateFileShouldThrowAnExceptionIfAlreadyExisting() {
            var storageFile = _azureBlobStorageProvider.CreateFile("foo.txt");
            Assert.AreEqual(storageFile.GetSize(), 0);

            _azureBlobStorageProvider.CreateFile("foo.txt");
        }

        [Test]
        public void RenameFileShouldCreateANewFileAndRemoveTheOld() {
            _azureBlobStorageProvider.CreateFile("foo1.txt");
            _azureBlobStorageProvider.RenameFile("foo1.txt", "foo2.txt");

            var files = _azureBlobStorageProvider.ListFiles("");

            Assert.AreEqual(1, files.Count());
            Assert.That(files.First().GetPath().EndsWith("foo2.txt"), Is.True);
        }

        [Test]
        public void CreateFileShouldBeFolderAgnostic()
        {
            _azureBlobStorageProvider.CreateFile("foo.txt");
            _azureBlobStorageProvider.CreateFile("folder/foo.txt");
            _azureBlobStorageProvider.CreateFile("folder/folder/foo.txt");

            Assert.AreEqual(1, _azureBlobStorageProvider.ListFiles("").Count());
            Assert.AreEqual(1, _azureBlobStorageProvider.ListFiles("folder").Count());
            Assert.AreEqual(1, _azureBlobStorageProvider.ListFiles("folder/folder").Count());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateFolderThatExistsShouldThrow() {
            // sebros: In Azure, the folder concept is just about checking files prefix. So until a file exists, a folder is nothing
            _azureBlobStorageProvider.CreateFile("folder/foo.txt");
            _azureBlobStorageProvider.CreateFolder("folder");
        }

        [Test]
        public void DeleteFolderShouldDeleteFilesAlso() {
            _azureBlobStorageProvider.CreateFile("folder/foo1.txt");
            _azureBlobStorageProvider.CreateFile("folder/foo2.txt");
            _azureBlobStorageProvider.CreateFile("folder/folder/foo1.txt");
            _azureBlobStorageProvider.CreateFile("folder/folder/foo2.txt");

            Assert.AreEqual(2, _azureBlobStorageProvider.ListFiles("folder").Count());
            Assert.AreEqual(2, _azureBlobStorageProvider.ListFiles("folder/folder").Count());

            _azureBlobStorageProvider.DeleteFolder("folder");

            Assert.AreEqual(0, _azureBlobStorageProvider.ListFiles("folder").Count());
            Assert.AreEqual(0, _azureBlobStorageProvider.ListFiles("folder/folder").Count());
        }

        [Test]
        public void ShouldRenameFolders() {
            _azureBlobStorageProvider.CreateFile("folder1/foo.txt");
            _azureBlobStorageProvider.CreateFile("folder2/foo.txt");
            _azureBlobStorageProvider.CreateFile("folder1/folder2/foo.txt");
            _azureBlobStorageProvider.CreateFile("folder1/folder2/folder3/foo.txt");

            Assert.AreEqual(1, _azureBlobStorageProvider.ListFiles("folder1").Count());
            Assert.AreEqual(1, _azureBlobStorageProvider.ListFiles("folder2").Count());
            Assert.AreEqual(1, _azureBlobStorageProvider.ListFiles("folder1/folder2").Count());
            Assert.AreEqual(1, _azureBlobStorageProvider.ListFiles("folder1/folder2/folder3").Count());

            _azureBlobStorageProvider.RenameFolder("folder1/folder2", "folder1/folder4");

            Assert.AreEqual(1, _azureBlobStorageProvider.ListFiles("folder1").Count());
            Assert.AreEqual(1, _azureBlobStorageProvider.ListFiles("folder2").Count());
            Assert.AreEqual(0, _azureBlobStorageProvider.ListFiles("folder1/folder2").Count());
            Assert.AreEqual(1, _azureBlobStorageProvider.ListFiles("folder1/folder4").Count());
            Assert.AreEqual(1, _azureBlobStorageProvider.ListFiles("folder1/folder4/folder3").Count());
        }

        [Test]
        public void ShouldReadWriteFiles() {
            const string teststring = "This is a test string.";

            var foo = _azureBlobStorageProvider.CreateFile("folder1/foo.txt");

            using(var stream = foo.OpenWrite())
                using (var writer = new StreamWriter(stream))
                    writer.Write(teststring);

            Assert.AreEqual(22, foo.GetSize());

            string content;
            using ( var stream = foo.OpenRead() )
            using ( var reader = new StreamReader(stream) ) {
                content = reader.ReadToEnd();
            }

            Assert.AreEqual(teststring, content);
        }
    }
}
