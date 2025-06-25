using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Diagnostics;
using System.IO;

namespace CoreConsoleRunner
{
    internal static class DatabaseUtility
    {
        private static string GenerateUniqueFileName(string baseName = "Temp")
        {
            return $"{baseName}_{Guid.NewGuid():N}.dwg";
        }

        private static string CreateClonePath(string originalPath)
        {
            string dir = Path.GetDirectoryName(originalPath);
            string tempName = GenerateUniqueFileName(Path.GetFileNameWithoutExtension(originalPath));
            return Path.Combine(dir, tempName);
        }

        private static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath)
                {
                    Attributes = FileAttributes.Normal
                };
                fileInfo.Delete();
            }
        }

        public class OpenDatabaseSwapper : IDisposable
        {
            private readonly Database _currentDb;
            private readonly string _clonedPath;
            private readonly bool _ownsDb;

            public OpenDatabaseSwapper(Database db)
            {
                _currentDb = db;
                _ownsDb = false;
            }

            public OpenDatabaseSwapper(string drawingPath, bool clone = false)
            {
                _ownsDb = true;
                if (clone)
                {
                    _clonedPath = CreateClonePath(drawingPath);
                    File.Copy(drawingPath, _clonedPath, true);
                    _currentDb = WorkingDatabaseManager.Open(_clonedPath);
                }
                else
                {
                    _currentDb = WorkingDatabaseManager.Open(drawingPath);
                }
            }

            public Database OpenDatabase => _currentDb;

            public void Dispose()
            {
                if (_ownsDb)
                    _currentDb?.Dispose();

                if (!string.IsNullOrEmpty(_clonedPath))
                    DeleteFile(_clonedPath);
            }
        }

        public class WorkingDatabaseSwapper : IDisposable
        {
            private readonly Database _previousDb = HostApplicationServices.WorkingDatabase;
            private readonly Database _currentDb;
            private readonly string _clonedPath;
            private readonly bool _ownsDb;

            public WorkingDatabaseSwapper(Database db)
            {
                _currentDb = db;
                _ownsDb = false;
                HostApplicationServices.WorkingDatabase = _currentDb;
            }

            public WorkingDatabaseSwapper(string drawingPath, bool clone = false)
            {
                _ownsDb = true;
                if (clone)
                {
                    _clonedPath = CreateClonePath(drawingPath);
                    File.Copy(drawingPath, _clonedPath, true);
                    _currentDb = WorkingDatabaseManager.InitializeWorkingDatabase(_clonedPath);
                }
                else
                {
                    _currentDb = WorkingDatabaseManager.InitializeWorkingDatabase(drawingPath);
                }
                HostApplicationServices.WorkingDatabase = _currentDb;
            }

            public Database WorkingDatabase => _currentDb;

            public void Dispose()
            {
                HostApplicationServices.WorkingDatabase = _previousDb;
                if (_ownsDb)
                    _currentDb?.Dispose();

                if (!string.IsNullOrEmpty(_clonedPath))
                    DeleteFile(_clonedPath);
            }
        }

        public static class WorkingDatabaseManager
        {
            public static Database Open(string drawingPath)
            {
                Debug.Assert(File.Exists(drawingPath), "Drawing path must exist.");
                var db = new Database(false, true);
                db.ReadDwgFile(drawingPath, FileOpenMode.OpenForReadAndAllShare, false, "");
                db.DisableUndoRecording(false);
                return db;
            }

            public static Database InitializeWorkingDatabase(string drawingPath)
            {
                var db = Open(drawingPath);
                HostApplicationServices.WorkingDatabase = db;
                return db;
            }
        }
    }
}
