using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// 設定ファイル(XMLファイル)操作クラス
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    ///  2011-01-25 xmlコメントの修正
    /// </history>
    public class XMLAccessClass
    {
        private XmlDocument xmlDocument = null;
        //private FileStream fs = null;

        // 設定ファイルはApplication Dataフォルダに保存
        private string roamingDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        // 設定ファイル保存先サブディレクトリ(ESRI\GISLight10)
        private string subDirectoryPath = Properties.Settings.Default.UserSettingsDirectoryPath;
        // 設定ファイル名
        private string fileName = Properties.Settings.Default.UserSettingsXmlName;
        // 設定ファイルフルパス
        private string fullPath = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected XMLAccessClass()
        {
            xmlDocument = new XmlDocument();

            // 設定ファイルのフルパスを取得
            fullPath = CreateFullPath();

        }

        /// <summary>
        /// 設定ファイル読み込み
        /// </summary>
        protected void LoadXMLDocument()
        {
            try
            {
                xmlDocument.Load(fullPath);

            }
            catch (Exception ex)
            {


                throw ex;
            }
        }

        /// <summary>
        /// 設定ファイル保存
        /// </summary>
        protected void SaveXMLDocument()
        {
            FileStream fs = null;

            try
            {

                fs = new FileStream(fullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                fs.Seek(0, SeekOrigin.Begin);
                fs.SetLength(0);

                xmlDocument.Save(fs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        /// <summary>
        /// 設定ファイルから値を取得
        /// </summary>
        /// <param name="tName">値を取得するタグ名</param>
        /// <returns>タグ名に対応する値の文字列</returns>
        protected string GetXMLValue(string tName)
        {
            try
            {
                string value = null;

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName(tName);

                if (nodeList.Count <= 0)
                {
                    throw new Exception();
                }

                foreach (XmlNode node in nodeList)
                {
                    value = node.InnerText;
                }

                return value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 設定ファイルから属性値を取得
        /// </summary>
        /// <param name="tName">値を取得するタグ名</param>
        /// <param name="aName">値を取得する属性名</param>
        /// <returns>属性値</returns>
        protected string GetXMLAttributeValue(string tName, string aName)
        {
            try
            {
                string value = null;

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName(tName);

                if (nodeList.Count <= 0)
                {
                    throw new Exception();
                }

                foreach (XmlNode node in nodeList)
                {
                    value = node.Attributes[aName].Value;
                }

                return value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 設定ファイルに値を設定
        /// </summary>
        /// <param name="tName">値を設定するタグ名</param>
        /// <param name="value">設定する値</param>
        protected void SetXMLValue(string tName, string value)
        {
            try
            {
                XmlNodeList nodeList = xmlDocument.GetElementsByTagName(tName);

                if (nodeList.Count <= 0)
                {
                    throw new Exception();
                }

                foreach (XmlNode node in nodeList)
                {
                    node.InnerText = value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 設定ファイルのフルパス作成
        /// </summary>
        private string CreateFullPath()
        {
            StringBuilder fullPath = new StringBuilder();

            fullPath.Append(roamingDirectoryPath);
            fullPath.Append("\\");
            fullPath.Append(subDirectoryPath);
            fullPath.Append("\\");
            fullPath.Append(fileName);

            return fullPath.ToString();
        }


        /// <summary>
        /// 整数型ノードの内容を取得
        /// </summary>
        /// <param name="tName">タグ名</param>
        /// <returns>整数型ノードの内容の整数型配列</returns>
        protected int[] GetIntNodeValue(string tName)
        {
            try
            {
                XmlNodeList nodeList = xmlDocument.GetElementsByTagName(tName);
                if (nodeList.Count <= 0)
                {
                    return null;
                    //throw new Exception();
                }

                System.Collections.Generic.List<int> intList = 
                    new System.Collections.Generic.List<int>();

                System.Collections.IEnumerator cnodes = nodeList[0].GetEnumerator();
                while (cnodes.MoveNext())
                {
                    XmlNode colnode = cnodes.Current as XmlElement;
                    intList.Add(Convert.ToInt32(colnode.InnerText));
                }

                int[] items = new int[intList.Count];
                int cnt = 0;
                foreach(int eachcolor in intList)
                {
                    items[cnt] = eachcolor;
                    cnt++;
                }
                return items;
            }
            catch (Exception ex)
            {
                throw ex; ;
            }
        }

        /// <summary>
        /// 指定ノードの子ノードをXML形式で取得します
        /// </summary>
        /// <param name="tName">タグ名</param>
        /// <returns>子ノードの内容を示すXML (群)</returns>
        protected string[] GetXMLNodes(string tName) {
			List<string>	strXMLs = new List<string>();
            try {
                XmlNodeList nodeList = xmlDocument.GetElementsByTagName(tName);
                if(nodeList.Count <= 0) {
                    throw new Exception();
                }

                foreach(XmlNode node in nodeList[0].ChildNodes) {
                    strXMLs.Add(node.OuterXml);
                }

                return strXMLs.ToArray();
            }
            catch(Exception ex) {
                throw ex;
            }
        }
        
        /// <summary>
        /// 指定ノードの子ノードをXML形式で設定します（全差し替え）
        /// </summary>
        /// <param name="tName">タグ名</param>
        /// <param name="ChildNodesXML">子ノードの内容を示すXML (群)</param>
        protected void SetXMLNodes(string tName, string[] ChildNodesXML) {
            try {
                XmlNodeList nodeList = xmlDocument.GetElementsByTagName(tName);
                if(nodeList.Count <= 0) {
                    throw new Exception();
                }

                XmlNode	xmlParent = nodeList[0];
                xmlParent.InnerXml = string.Join("", ChildNodesXML);
            }
            catch(Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// 引数指定された整数型配列の内容を整数型ノードに保存
        /// </summary>
        /// <param name="tName">タグ名</param>
        /// <param name="intValues">整数型配列</param>
        protected void SetIntNodeValue(string tName, int[] intValues)
        {
            try
            {
                XmlNodeList nodeList = xmlDocument.GetElementsByTagName(tName);

                if (nodeList.Count <= 0)
                {
                    throw new Exception();
                }

                System.Collections.Generic.List<int> intList =
                    new System.Collections.Generic.List<int>();

                System.Collections.IEnumerator cnodes = nodeList[0].GetEnumerator();
                
                int cnt = 0;
                while (cnodes.MoveNext())
                {
                    XmlNode colnode = cnodes.Current as XmlElement;
                    colnode.InnerText = intValues[cnt].ToString();
                    cnt++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 引数指定された整数型配列の内容を整数型ノードに追加
        /// </summary>
        /// <param name="tName">タグ名</param>
        /// <param name="intValues">整数型配列</param>
        protected void CreateIntNodeValue(string tName, int[] intValues)
        {
            try
            {
                XmlNode elm = xmlDocument.CreateNode(XmlNodeType.Element, tName, null);
                
                XmlNode root2 = xmlDocument.DocumentElement.FirstChild;
                root2.AppendChild(elm);
                
                for (int i = 0; i < intValues.Length; i++)
                {
                    XmlNode newElem = xmlDocument.CreateNode(XmlNodeType.Element, "int", null);
                    newElem.InnerText = intValues[i].ToString();
                    elm.AppendChild(newElem);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
