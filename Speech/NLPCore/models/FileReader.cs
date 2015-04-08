using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore.models
{
    public class FileReader : Reader
    {
        Queue<FileInfo> files;
        Instance cur;
        string content = null;
        StreamReader reader;
        int line;
        FileInfo currentFile;
        private string filter;

        public FileReader(String path) :
            this(path, "UTF-8", null)
        {
            
        }
        /**
         * 
         * @param path 路径名
         * @param charsetName 字符编码
         * @param filter 文件类型过滤
         */
        public FileReader(string path, string charsetName, string filter)
        {
            files = new Queue<FileInfo>();
            this.filter = filter;
            DirectoryInfo fpath = new DirectoryInfo(path);

            if (fpath.Exists)
            {
                FileInfo[] flist = fpath.GetFiles();
                for (int i = 0; i < flist.Length; i++)
                {
                    if (flist[i].Exists)
                    {
                        if (filter == null)
                            files.Enqueue(flist[i]);
                        else if (flist[i].Name.EndsWith(filter))
                            files.Enqueue(flist[i]);
                    }
                }
            }
            getFile();
        }

        private bool getFile()
        {
            if (files.Count == 0)
            {
                return false;
            }

            currentFile = files.Dequeue();
            if (currentFile == null)
                return false;

            reader = new StreamReader(currentFile.FullName, Encoding.UTF8);

            line = 0;
            return true;
        }

        public override bool hasNext()
        {
            while (true)
            {
                content = reader.ReadLine();
                line++;
                if (content == null)
                {
                    reader.Close();
                    if (!getFile())
                    {
                        return false;
                    }
                    continue;
                }
                content = content.Trim();
                if (content.Length == 0)
                    continue;
                else
                    return true;

            }
        }

        public override Instance next()
        {
            int idx = currentFile.Name.IndexOf(".");
            return new Instance(content, currentFile.Name.Substring(0, idx));
        }
    }
}
