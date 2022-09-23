using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccoreconsoleBug
{
    internal class DatabaseUtility
    {

        private static string GetGUID()
        {

            Guid guid = Guid.NewGuid();
            string str = guid.ToString("N");
            return str;
        }
        public class OpenDatabaseSwapper : IDisposable
        {
            public OpenDatabaseSwapper(Database db)
            {
                m_OpenedNewDatabase = false;
                m_DatabaseCurrent = db;
            }

            public OpenDatabaseSwapper(string drawingPath, bool needCopyDatabase = false)
            {
                m_OpenedNewDatabase = true;

                Initialize(drawingPath, needCopyDatabase);
            }

            public void Dispose()
            {
                if (m_OpenedNewDatabase)
                {
                    m_DatabaseCurrent.Dispose();
                }

                if (m_DrawingPathCloned != null)
                {
                    this.DeleteFile(m_DrawingPathCloned);
                }
            }

            public Database OpenDatabase
            {
                get
                {
                    return m_DatabaseCurrent;
                }
            }


            private void Initialize(string drawingPath, bool needCopyDatabase = false)
            {
                System.Diagnostics.Debug.Assert(File.Exists(drawingPath), @"The drawing path can't be null");

                if (needCopyDatabase)
                {
                    string todwgName = string.Format("{0}.dwg", GetGUID()); // add guid to avoid it's being used by another process.- support the parallel.
                    m_DrawingPathCloned = drawingPath.Replace(@".dwg", todwgName);

                    this.DeleteFile(m_DrawingPathCloned);
                    File.Copy(drawingPath, m_DrawingPathCloned, true);
                    m_DatabaseCurrent = WorkingDatabaseManager.Open(m_DrawingPathCloned);
                }
                else
                {
                    m_DatabaseCurrent = WorkingDatabaseManager.Open(drawingPath);
                }
            }


            private void DeleteFile(string drawingPath)
            {
                if (File.Exists(drawingPath))
                {
                    FileInfo fileInfo = new FileInfo(drawingPath)
                    {
                        Attributes = FileAttributes.Normal
                    };

                    fileInfo.Delete();
                }
            }

            private Database m_DatabaseCurrent = null;
            private string m_DrawingPathCloned = null;
            private bool m_OpenedNewDatabase = false;
        }


        public class WorkingDatabaseSwapper : IDisposable
        {
            public WorkingDatabaseSwapper(Database db)
            {
                m_OpenedNewDatabase = false;
                m_DatabaseCurrent = db;
                HostApplicationServices.WorkingDatabase = m_DatabaseCurrent;
            }

            public WorkingDatabaseSwapper(string drawingPath, bool needCopyDatabase = false)
            {
                m_OpenedNewDatabase = true;
                Initialize(drawingPath, needCopyDatabase);
            }

            public void Dispose()
            {
                HostApplicationServices.WorkingDatabase = m_DatabasePrevious;
                if (m_OpenedNewDatabase)
                {
                    m_DatabaseCurrent.Dispose();
                }

                if (m_DrawingPathCloned != null)
                {
                    DeleteFile(m_DrawingPathCloned);
                }
            }

            public Database WorkingDatabase
            {
                get
                {
                    return m_DatabaseCurrent;
                }
            }

            private void Initialize(string drawingPath, bool needCopyDatabase = false)
            {
                System.Diagnostics.Debug.Assert(File.Exists(drawingPath), @"The drawing path can't be null");

                if (needCopyDatabase)
                {
                    string todwgName = string.Format("{0}.dwg", GetGUID()); // add guid to avoid it's being used by another process.- support the parallel.
                    m_DrawingPathCloned = drawingPath.Replace(@".dwg", todwgName);

                    this.DeleteFile(m_DrawingPathCloned);
                    File.Copy(drawingPath, m_DrawingPathCloned, true);
                    m_DatabaseCurrent = WorkingDatabaseManager.InitializeWorkingDatabase(m_DrawingPathCloned);
                }
                else
                {
                    m_DatabaseCurrent = WorkingDatabaseManager.InitializeWorkingDatabase(drawingPath);
                }
            }


            private void DeleteFile(string drawingPath)
            {
                if (File.Exists(drawingPath))
                {
                    FileInfo fileInfo = new FileInfo(drawingPath);
                    fileInfo.Attributes = FileAttributes.Normal;

                    fileInfo.Delete();
                }
            }

            private Database m_DatabasePrevious = HostApplicationServices.WorkingDatabase;
            private Database m_DatabaseCurrent = null;
            private string m_DrawingPathCloned = null;
            private bool m_OpenedNewDatabase = false;
        }


        public class WorkingDatabaseManager
        {
            public static Database Open(string drawingPath)
            {
                System.Diagnostics.Debug.Assert(drawingPath != null, @"The drawing path can't be null");

                Database db = new Database(false, true);
                db.ReadDwgFile(drawingPath, FileOpenMode.OpenForReadAndAllShare, false, "");
                db.DisableUndoRecording(false);

                return db;
            }

            public static Database InitializeWorkingDatabase(string drawingPath)
            {
                Database db = WorkingDatabaseManager.Open(drawingPath);
                HostApplicationServices.WorkingDatabase = db;

                return db;
            }
        }
    }
}
