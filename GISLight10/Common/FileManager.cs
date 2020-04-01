using System;
using System.Collections.Generic;
using System.IO;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// ファイル、フォルダを管理するクラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    /// </history>
    public class FileManager
    {
        /// <summary>
        /// 指定フォルダ内の全ファイル、全サブフォルダ削除
        /// </summary>
        /// <param name="directoryPath">ファイルを削除するフォルダ</param>
        /// <param name="undeletableFiles">削除不可ファイル</param>
        /// <param name="undeletableDirectorys">削除不可フォルダ</param>
        public static void DeleteFilesInDirectory(string directoryPath,
            out List<string> undeletableFiles, out List<string> undeletableDirectorys)
        {
            DirectoryInfo targetDirectoryInfo = new DirectoryInfo(directoryPath);

            // 全ファイルを削除
            undeletableFiles = DeleteFiles(targetDirectoryInfo);
            //全フォルダを削除
            undeletableDirectorys = DeleteDirectorys(targetDirectoryInfo);
        }

        /// <summary>
        /// 指定フォルダ内の全ファイル削除
        /// </summary>
        /// <param name="dirInfo">ファイルを削除するフォルダ</param>
        private static List<string> DeleteFiles(DirectoryInfo dirInfo)
        {
            List<string> undeletableList = new List<string>();

            foreach (FileInfo fi in dirInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                try
                {
                    if (fi.Exists)
                    {
                        fi.Delete();
                    }
                }
                catch (Exception)
                {
                    undeletableList.Add(fi.FullName);
                    continue;
                }
            }

            return undeletableList;
        }

        /// <summary>
        /// 指定フォルダ内の全サブフォルダ削除
        /// </summary>
        /// <param name="dirInfo">サブフォルダを削除するフォルダ</param>
        private static List<string> DeleteDirectorys(DirectoryInfo dirInfo)
        {
            List<string> dirList = CreateDirectorysList(dirInfo);
            List<string> undeletableList = new List<string>();

            if (dirList.Count > 0)
            {
                // 深い階層からフォルダを削除
                for (int i = dirList.Count; i > 0; i--)
                {
                    try
                    {
                        if (Directory.Exists(dirList[i - 1]))
                        {
                            Directory.Delete(dirList[i - 1]);
                        }
                    }
                    catch (Exception)
                    {
                        undeletableList.Add(dirList[i - 1]);
                        continue;
                    }
                }
            }

            // リストを反転
            undeletableList.Reverse();

            return undeletableList;
        }

        /// <summary>
        /// 指定フォルダ内の全サブフォルダリスト作成
        /// </summary>
        /// <param name="dirInfo">リストを作成するフォルダ</param>
        private static List<string> CreateDirectorysList(DirectoryInfo dirInfo)
        {
            List<string> directoryList = new List<string>();

            foreach (DirectoryInfo di in dirInfo.GetDirectories("*", SearchOption.AllDirectories))
            {
                if (di.Exists)
                {
                    directoryList.Add(di.FullName);
                }   
            }

            return directoryList;
        }
    }
}
