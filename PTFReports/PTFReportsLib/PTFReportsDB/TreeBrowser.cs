/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.Collections.Generic;
using System;
using System.Linq;

namespace PTF.Reports.PTFReportsDB
{
    public class TreeBrowser
    {
        private readonly Folder m_Root;
        private readonly IEnumerable<Folder> m_folders;

        public TreeBrowser(Folder root, IEnumerable<Folder> folders)
        {
            m_Root = root;
            m_folders = folders;
        }

        public IEnumerable<Folder> Browse()
        {
            return BrowserInternalRecursive(m_Root);
        }

        private IEnumerable<Folder> BrowserInternalRecursive(Folder folder)
        {
            if (folder != null)
            {
                foreach (Folder f in folder.Folders1)
                {
                    if (m_folders.Contains(f))
                        yield return f;
                    foreach (var f1 in BrowserInternalRecursive(f))
                        if (m_folders.Contains(f))
                            yield return f1;
                }
            }
        }
    }
}
