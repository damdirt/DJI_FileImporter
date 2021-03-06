﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace DJI_FileImporter
{
    class Program
    {
        static void Main(String[] args)
        {
            Console.WriteLine($"Welcome to DJI files importer\n");
            String sourceFolderPath, destinationFolderPath, panoramasDestinationFolderPath, timelapsePhotosDestinationFolderPath;
            sourceFolderPath = destinationFolderPath = panoramasDestinationFolderPath = timelapsePhotosDestinationFolderPath = String.Empty;

            var defaultSourceFolderPath = ConfigurationManager.AppSettings["defaultSourcePath"];
            var defaultDestinationFolderPath = ConfigurationManager.AppSettings["defaultDestinationPath"];

            // Asking for source folder path
            Program.AskForSourceFolderPath(defaultSourceFolderPath, ref sourceFolderPath);

            // Asking destination folder
            Program.AskForDestinationFolderPath(defaultDestinationFolderPath, ref destinationFolderPath);

            // Asking for importing Panoramas
            Boolean hasToImportPanoramas = false;
            Program.AskForPanoramaImport(defaultDestinationFolderPath, ref panoramasDestinationFolderPath, ref hasToImportPanoramas);

            // Asking for importing Photo Timelapses
            Boolean hasToImportTimelapsePhotos = false;
            Program.AskForPhotoTimelapseImport(defaultDestinationFolderPath, ref timelapsePhotosDestinationFolderPath, ref hasToImportTimelapsePhotos);

            // Asking for deleting files after copy
            Boolean deleteAllFiles = false;
            Program.HasToDeleteSourceFilesAfterCopy(sourceFolderPath, ref deleteAllFiles);

            // Getting files
            IEnumerable<FileDetail> files = Program.GetMediaFiles(sourceFolderPath);

            // Copying files
            Program.CopyFiles(files, destinationFolderPath);

            // Copying Panoramas
            Program.CopyPanoramas(hasToImportPanoramas, defaultSourceFolderPath, panoramasDestinationFolderPath);

            // Copying Timelapses
            Program.CopyTimelapsePhotos(hasToImportTimelapsePhotos, defaultSourceFolderPath, timelapsePhotosDestinationFolderPath);

            // Deleting files
            Program.DeleteFiles(files, deleteAllFiles, sourceFolderPath);

            //Deleting Panoramas
            Program.DeletePanoramas(deleteAllFiles, hasToImportPanoramas, sourceFolderPath);

            //Deleting Timelapse Photos
            Program.DeleteTimelapsePhotos(deleteAllFiles, hasToImportTimelapsePhotos, sourceFolderPath);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void AskForSourceFolderPath(String defaultSourceFolderPath, ref String sourceFolderPath)
        {
            do
            {
                Console.Write($@"Enter source folder (default is '{defaultSourceFolderPath}'): ");
                sourceFolderPath = Console.ReadLine();

                if (String.IsNullOrEmpty(sourceFolderPath))
                {
                    sourceFolderPath = defaultSourceFolderPath;
                }

            } while (!Program.IsFolderExist(sourceFolderPath));
            Console.WriteLine($"Source path: {sourceFolderPath}\n");
        }

        private static void AskForDestinationFolderPath(String defaultDestinationFolderPath, ref String destinationFolderPath)
        {
            do
            {
                Console.Write($@"Enter destination folder (default is '{defaultDestinationFolderPath}'): ");
                destinationFolderPath = Console.ReadLine();
                if (String.IsNullOrEmpty(destinationFolderPath))
                {
                    destinationFolderPath = defaultDestinationFolderPath;
                }
            } while (!Program.IsFolderExist(destinationFolderPath));
            Console.WriteLine($"Destination path: {destinationFolderPath}\n");
        }

        private static void AskForPanoramaImport(String defaultDestinationFolderPath, ref String panoramasDestinationFolderPath, ref Boolean hasToImportPanoramas)
        {
            var panoramaFolderName = ConfigurationManager.AppSettings["panoramaFolderName"];
            defaultDestinationFolderPath = Path.Combine(defaultDestinationFolderPath, panoramaFolderName);

            Console.Write($"Import Panoramas to {defaultDestinationFolderPath} (Y/N) ? ");

            String hasToImportPanoramasUserEntry = String.Empty;

            hasToImportPanoramasUserEntry = Console.ReadLine();
            hasToImportPanoramas = String.IsNullOrEmpty(hasToImportPanoramasUserEntry) || String.Equals(hasToImportPanoramasUserEntry, "Y", StringComparison.OrdinalIgnoreCase);

            if (hasToImportPanoramas)
            {
                do
                {
                    Console.Write($@"Enter panorama destination folder (default is '{defaultDestinationFolderPath}'): ");
                    panoramasDestinationFolderPath = Console.ReadLine();
                    if (String.IsNullOrEmpty(panoramasDestinationFolderPath))
                    {
                        panoramasDestinationFolderPath = defaultDestinationFolderPath;
                    }
                } while (!Program.IsFolderExist(panoramasDestinationFolderPath));
                Console.WriteLine($"Destination path: {panoramasDestinationFolderPath}");
            }

            Console.WriteLine();
        }

        private static void AskForPhotoTimelapseImport(String defaultDestinationFolderPath, ref String timelapsePhotosDestinationFolderPath, ref Boolean hasToImportTimelapsePhotos)
        {
            var timelapsePhotosFolderName = ConfigurationManager.AppSettings["timelapsePhotoFolderName"];
            defaultDestinationFolderPath = Path.Combine(defaultDestinationFolderPath, timelapsePhotosFolderName);

            Console.Write($"Import Timelapses Photos to {defaultDestinationFolderPath} (Y/N) ? ");

            String hasToImportTimelapsePhotosUserEntry = String.Empty;

            hasToImportTimelapsePhotosUserEntry = Console.ReadLine();
            hasToImportTimelapsePhotos = String.IsNullOrEmpty(hasToImportTimelapsePhotosUserEntry) || String.Equals(hasToImportTimelapsePhotosUserEntry, "Y", StringComparison.OrdinalIgnoreCase);

            if (hasToImportTimelapsePhotos)
            {
                do
                {
                    Console.Write($@"Enter timelapse photos destination folder (default is '{defaultDestinationFolderPath}'): ");
                    timelapsePhotosDestinationFolderPath = Console.ReadLine();
                    if (String.IsNullOrEmpty(timelapsePhotosDestinationFolderPath))
                    {
                        timelapsePhotosDestinationFolderPath = defaultDestinationFolderPath;
                    }
                } while (!Program.IsFolderExist(timelapsePhotosDestinationFolderPath));
                Console.WriteLine($"Destination path: {timelapsePhotosDestinationFolderPath}");
            }

            Console.WriteLine();
        }

        private static void CopyFiles(IEnumerable<FileDetail> files, String destinationFolderPath)
        {
            var copiedFilesCount = 0;
            foreach (var file in files)
            {
                Program.CopyFile(file, destinationFolderPath, ref copiedFilesCount);
            }

            Console.WriteLine("Copy of Media files finished");
            Console.WriteLine($"{files.Count()} were processed");
            Console.WriteLine($"{copiedFilesCount} files were copied in {destinationFolderPath}\n");
        }

        private static void HasToDeleteSourceFilesAfterCopy(String sourceFolderPath, ref Boolean deleteAllFiles)
        {
            Console.Write($"Delete all files from {sourceFolderPath} after copy (Y/N) ? ");
            String deleteAllFilesUserEntry = String.Empty;
            do
            {
                deleteAllFilesUserEntry = Console.ReadLine();
                deleteAllFiles = String.Equals(deleteAllFilesUserEntry, "Y", StringComparison.OrdinalIgnoreCase);
            } while (String.IsNullOrEmpty(deleteAllFilesUserEntry));
        }

        private static void DeleteFiles(IEnumerable<FileDetail> files, Boolean deleteAllFiles, String sourceFolderPath)
        {
            if (!deleteAllFiles || !files.Any())
                return;

            Console.WriteLine($"Deleting all files in {sourceFolderPath}");

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file.FilePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occured during deleting file '{file.FilePath}'");
                    Console.WriteLine("Error message:");
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine("Deleting finished\n");
        }

        private static void DeletePanoramas(Boolean deleteAllFiles, Boolean hasToImportPanoramas, String sourceFolderPath)
        {
            if (deleteAllFiles && hasToImportPanoramas)
            {
                String panoramaFolderName = ConfigurationManager.AppSettings["panoramaFolderName"];
                sourceFolderPath = Path.Combine(sourceFolderPath, panoramaFolderName);

                if (Program.IsFolderExist(sourceFolderPath))
                {
                    Console.WriteLine($"Deleting all panoramas in {sourceFolderPath}");

                    foreach (String folder in Directory.GetDirectories(sourceFolderPath))
                    {
                        Directory.Delete(folder, true);
                    }

                    Console.WriteLine("Deleting finished\n");
                }
            }
        }

        private static void DeleteTimelapsePhotos(Boolean deleteAllFiles, Boolean hasToImportTimelapsePhotos, String sourceFolderPath)
        {
            if (deleteAllFiles && hasToImportTimelapsePhotos)
            {
                String timelapsePhotosFolderName = ConfigurationManager.AppSettings["timelapsePhotoFolderName"];
                sourceFolderPath = Path.Combine(sourceFolderPath, timelapsePhotosFolderName);

                if (Program.IsFolderExist(sourceFolderPath))
                {
                    Console.WriteLine($"Deleting all Timelapse photos in {sourceFolderPath}");

                    foreach (String folder in Directory.GetDirectories(sourceFolderPath))
                    {
                        Directory.Delete(folder, true);
                    }

                    Console.WriteLine("Deleting finished\n");
                }
            }
        }

        private static void CopyFile(FileDetail file, String destinationFolderPath, ref Int32 copiedFilesCount)
        {
            var parentFolderName = file.CreationDate.ToString(ConfigurationManager.AppSettings["dateFolderFormat"]);
            var parentFolderPath = Path.Combine(destinationFolderPath, parentFolderName);
            if (!Directory.Exists(parentFolderPath))
            {
                Directory.CreateDirectory(parentFolderPath);
            }

            var destinationFilePath = Path.Combine(parentFolderPath, file.FileName);
            // check if file already exists
            if (Program.IsFileExist(destinationFilePath))
            {
                Console.WriteLine($"File {destinationFilePath} already exists. Copy aborted for this file.");
                return;
            }
            Console.WriteLine($"...Copying file {file.FilePath} to {destinationFilePath}...");
            try
            {
                File.Copy(file.FilePath, destinationFilePath);
                ++copiedFilesCount;
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured during copying file {file.FilePath} to {destinationFilePath}");
                Console.WriteLine("Error message:");
                Console.WriteLine(e.Message);
            }
        }

        internal static Boolean IsFileExist(String path)
        {
            return File.Exists(path);
        }
        internal static Boolean IsFolderExist(String path)
        {
            return Directory.Exists(path);
        }

        internal static IEnumerable<FileDetail> GetMediaFiles(String path)
        {
            var mediaFolderName = ConfigurationManager.AppSettings["mediaFolderName"];
            path = Path.Combine(path, mediaFolderName);

            List<FileDetail> fileDetails = new List<FileDetail>();
            IEnumerable<String> filePaths = Directory.GetFiles(path);

            foreach (String filePath in filePaths)
            {
                FileDetail fileDetail = new FileDetail();
                fileDetail.CreationDate = File.GetCreationTime(filePath);
                fileDetail.FilePath = filePath;
                fileDetail.FileName = Path.GetFileName(filePath);

                fileDetails.Add(fileDetail);
            }

            return fileDetails;
        }

        internal static void CopyPanoramas(Boolean hasToImportPanoramas, String sourcePath, String destinationPath)
        {
            if (hasToImportPanoramas)
            {
                String panoramaFolderName = ConfigurationManager.AppSettings["panoramaFolderName"];
                sourcePath = Path.Combine(sourcePath, panoramaFolderName);

                if (Program.IsFolderExist(sourcePath))
                {
                    Int32 copiedFoldersCount = 0;
                    Program.CopyDirectory(sourcePath, destinationPath, ref copiedFoldersCount);

                    Console.WriteLine("Copy of Panoramas files finished");
                    Console.WriteLine($"{Directory.GetDirectories(sourcePath).Count()} were processed");
                    Console.WriteLine($"{copiedFoldersCount} folders were copied in {destinationPath}\n");
                }
                else
                {
                    Console.WriteLine($"Folder {sourcePath} can't be found.");
                }
            }
        }

        internal static void CopyTimelapsePhotos(Boolean hasToImportTimelapsePhotos, String sourcePath, String destinationPath)
        {
            if (hasToImportTimelapsePhotos)
            {
                String photoTimelapseFolderName = ConfigurationManager.AppSettings["timelapsePhotoFolderName"];
                sourcePath = Path.Combine(sourcePath, photoTimelapseFolderName);

                if (Program.IsFolderExist(sourcePath))
                {
                    Int32 copiedFoldersCount = 0;
                    Program.CopyDirectory(sourcePath, destinationPath, ref copiedFoldersCount);

                    Console.WriteLine("Copy of Timelapses Photos files finished");
                    Console.WriteLine($"{Directory.GetDirectories(sourcePath).Count()} were processed");
                    Console.WriteLine($"{copiedFoldersCount} folders were copied in {destinationPath}\n");
                }
                else
                {
                    Console.WriteLine($"Folder {sourcePath} can't be found.");
                }
            }
        }

        internal static void CopyDirectory(String sourcePath, String destinationPath, ref Int32 copiedFoldersCount)
        {
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            foreach (String file in Directory.GetFiles(sourcePath))
            {
                String destination = Path.Combine(destinationPath, Path.GetFileName(file));
                if (!Program.IsFileExist(destination))
                    File.Copy(file, destination);
            }

            foreach (String folder in Directory.GetDirectories(sourcePath))
            {
                String destination = Path.Combine(destinationPath, Path.GetFileName(folder));
                if (!Directory.Exists(destination))
                {
                    Console.WriteLine($"...Copying directory {folder} to {destination}...");
                    CopyDirectory(folder, destination, ref copiedFoldersCount);
                    copiedFoldersCount++;
                }
            }
        }
    }

    internal class FileDetail
    {
        public String FileName { get; set; }
        public String FilePath { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
