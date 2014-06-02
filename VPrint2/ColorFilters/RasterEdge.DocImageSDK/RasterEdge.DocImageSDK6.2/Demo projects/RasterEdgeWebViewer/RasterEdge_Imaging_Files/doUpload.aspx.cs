using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Collections.Generic;
using RasterEdge.Imaging.WebViewer;

public partial class doUpload : System.Web.UI.Page
{
        protected void Page_Load(object sender, EventArgs e)
		{
            if (Request.Files.Count > 0)
            {
                HttpPostedFile file = Request.Files[0];

                string msg = "";
                string error = "";
                if (file.ContentLength == 0)
                    error = "The length of file is 0";
                else
                {
                    string rootPath = Server.MapPath("../RasterEdge_Cache");
                    string fileName = Path.GetFileName(file.FileName);
                    string suffix = getFileSuffix(fileName);
                    //int index = getNameIndex(rootPath,fileName);
                    int index = getNameIndex(rootPath);
                    //string combineName = Session.SessionID + "_" + index + "_" + fileName;
                    string combineName = Session.SessionID + "_" + index + suffix;

                    string fileSavePath = rootPath + "\\" + combineName;
                    file.SaveAs(fileSavePath);
                    msg = combineName;
                }
                string result = msg;
                Response.Write(result);
                Response.End();
		    }
	    }


        protected int getNameIndex(string rootPath)
        {
            int resultIndex = 0;
            try
            {
                string[] Files = Directory.GetFiles(rootPath);
                List<int> _indexList = new List<int>();
                for (int i = 0; i < Files.Length; i++)
                {
                    Files[i] = Files[i].Substring(rootPath.Length + 1);
                    if (Files[i].IndexOf(Session.SessionID) == 0)
                    {
                        int length = Session.SessionID.ToString().Length + 1;
                        Files[i] = Files[i].Substring(length);
                        string suffix = getFileSuffix(Files[i]);
                        int endIndex = Files[i].IndexOf(suffix);
                        int index = Convert.ToInt32(Files[i].Substring(0, endIndex));
                        _indexList.Add(index);
                    }
                }
                int count = _indexList.Count;
                if (count != 0)
                {
                    _indexList.Sort();
                    resultIndex = _indexList[count - 1] + 1;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, LogType.ERROR);
            }
            return resultIndex;
        }

        protected string getFileSuffix(string fileUrl)
        {
            int posi = fileUrl.LastIndexOf(".");

            if (posi < 0)
            {
                Logger.Log("Input File is not Found! Maybe file name is error", LogType.ERROR);
                return "";
            }

            string suffix = fileUrl.Substring(posi);

            suffix = suffix.ToLower();

            return suffix;
        }
}
