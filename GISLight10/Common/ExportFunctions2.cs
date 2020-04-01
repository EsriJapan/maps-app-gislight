using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;

namespace ESRIJapan.GISLight10.Common
{
    /// <summary>
    /// 属性値集計で使用されるエクスポート処理
    /// </summary>
    /// <history>
    ///  2010-11-01 新規作成
    ///  2011-06-17 レイヤのフィルタ設定対応
    /// </history>
    public static class ExportFunctions2
    {
        /// <summary>
        /// 指定フィールドをMDBにエクスポートする
        /// </summary>
        /// <param name="workspace_mdb">MDBファイルの出力先フォルダのパス</param>
        /// <param name="name_mdb">MDBファイル名</param>
        /// <param name="pFLayer">フィーチャレイヤ</param>
        /// <param name="tableName">テーブル名</param>
        /// <param name="summaryFieldNum">集計対象フィールド名</param>
        /// <param name="keyFieldNums">キーフィールド名</param>
        public static void ExportMDB(String workspace_mdb, String name_mdb, IFeatureLayer pFLayer, String tableName, int summaryFieldNum, params int[] keyFieldNums)
        {
            ExportMDB(workspace_mdb, name_mdb, pFLayer, tableName, summaryFieldNum, false, keyFieldNums);
        }


        /// <summary>
        /// 指定フィールドをMDBにエクスポートする
        /// </summary>
        /// <param name="workspace_mdb">MDBファイルの出力先フォルダのパス</param>
        /// <param name="name_mdb">MDBファイル名</param>
        /// <param name="pFLayer">フィーチャレイヤ</param>
        /// <param name="tableName">テーブル名</param>
        /// <param name="summaryFieldNum">集計対象フィールド名</param>
        /// <param name="keyFieldNums">キーフィールド名</param>
        /// <param name="blnSelection">選択フィーチャのみ対象にするかのフラグ</param>
        public static void ExportMDB(String workspace_mdb, String name_mdb, IFeatureLayer pFLayer, String tableName, int summaryFieldNum, bool blnSelection, params int[] keyFieldNums)
        {
            IWorkspace pWorkspace = null;
            IWorkspace2 workspace2 = null;
            IFields fields = null;

            ESRI.ArcGIS.Geodatabase.IFeatureWorkspace featureWorkspace = null;
            ESRI.ArcGIS.Geodatabase.ITable table = null;
            ESRI.ArcGIS.Geodatabase.IField field = null;
            ESRI.ArcGIS.Geodatabase.IFieldEdit fieldEdit = null;
            ESRI.ArcGIS.Geodatabase.IFieldChecker fieldChecker = null;
            ESRI.ArcGIS.Geodatabase.IEnumFieldError enumFieldError = null;
            ESRI.ArcGIS.Geodatabase.IFields validatedFields = null;
            ESRI.ArcGIS.Carto.IDisplayTable pDisplayTable = null;
            //ESRI.ArcGIS.Carto.IFeatureSelection featureSelection = null;
            ESRI.ArcGIS.Geodatabase.ISelectionSet selectionSet = null;

            IRowBuffer rowBuffer = null;
            ICursor outCursor = null;
            ICursor cursor = null;
            IRow inRow = null;

            try
            {
                pWorkspace = CreateEmptyMDB(workspace_mdb, name_mdb);

                // テーブル結合済みのフィーチャレイヤを取得
                IGeoFeatureLayer pGeoFLayer = (IGeoFeatureLayer)pFLayer;
                int fieldCount = keyFieldNums.Length + 1;

                int[] FieldNums = new int[fieldCount];
                FieldNums[0] = summaryFieldNum;
                for (int i = 0; i < keyFieldNums.Length; i++)
                {
                    FieldNums[i + 1] = keyFieldNums[i];
                }

                workspace2 = (IWorkspace2)pWorkspace;
                fields = pGeoFLayer.DisplayFeatureClass.Fields;

                // create the behavior clasid for the featureclass
                ESRI.ArcGIS.esriSystem.UID uid =
                    new ESRI.ArcGIS.esriSystem.UIDClass();

                featureWorkspace = (ESRI.ArcGIS.Geodatabase.IFeatureWorkspace)pWorkspace; // Explicit Cast

                uid.Value = "esriGeoDatabase.Object";

                //ESRI.ArcGIS.Geodatabase.IObjectClassDescription 
                ESRI.ArcGIS.Geodatabase.IObjectClassDescription objectClassDescription =
                    new ESRI.ArcGIS.Geodatabase.ObjectClassDescriptionClass();


                // create the fields using the required fields method
                ESRI.ArcGIS.Geodatabase.IFieldsEdit fieldsEdit = (ESRI.ArcGIS.Geodatabase.IFieldsEdit)objectClassDescription.RequiredFields;// (ESRI.ArcGIS.Geodatabase.IFieldsEdit)fields; // Explicit Cast

                // エクスポート対象フィールドの定義をコピー
                for (int i = 0; i < fieldCount; i++)
                {
                    field = new ESRI.ArcGIS.Geodatabase.FieldClass();
                    fieldEdit = (ESRI.ArcGIS.Geodatabase.IFieldEdit)field; // Explicit Cast

                    int keyNum = FieldNums[i];
                    fieldEdit.Name_2 = ExportFunctions2.GetFieldName(fields.get_Field(keyNum).Name);
                    fieldEdit.Type_2 = fields.get_Field(keyNum).Type;
                    fieldEdit.IsNullable_2 = fields.get_Field(keyNum).IsNullable;
                    fieldEdit.AliasName_2 = fields.get_Field(keyNum).AliasName;
                    fieldEdit.DefaultValue_2 = fields.get_Field(keyNum).DefaultValue;
//                    fieldEdit.Editable_2 = fields.get_Field(keyNum).Editable;
                    fieldEdit.Editable_2 = true;
                    fieldEdit.Length_2 = fields.get_Field(keyNum).Length;

                    fieldsEdit.AddField(field); // add field to field collection
                }
                fields = (ESRI.ArcGIS.Geodatabase.IFields)fieldsEdit; // Explicit Cast

                // Use IFieldChecker to create a validated fields collection.
                fieldChecker = new ESRI.ArcGIS.Geodatabase.FieldCheckerClass();
                fieldChecker.ValidateWorkspace = (ESRI.ArcGIS.Geodatabase.IWorkspace)pWorkspace;
                fieldChecker.Validate(fields, out enumFieldError, out validatedFields);

                // テーブルを作成
                table = featureWorkspace.CreateTable(tableName, validatedFields, uid, null, "");

                // テーブルのコピーに使用するカーソル
                rowBuffer = table.CreateRowBuffer();
                outCursor = table.Insert(true);
                if (blnSelection == false)
                {
                    // 20110617 レイヤのフィルタ設定反映
                    //cursor = (ICursor)pGeoFLayer.DisplayFeatureClass.Search(null, true);
                    cursor = (ICursor)pGeoFLayer.SearchDisplayFeatures(null, true);
                }
                else
                {
                    pDisplayTable = (IDisplayTable)pFLayer;
                    selectionSet = pDisplayTable.DisplaySelectionSet;
                    selectionSet.Search(null, false, out cursor);
                }
                inRow = cursor.NextRow();

                // キーフィールドの値をコピーする
                int ii = 0;
                while (inRow != null)
                {
                    for (int j = 0; j < fieldCount; j++)
                    {
                        rowBuffer.set_Value(j + 1, inRow.get_Value(FieldNums[j]));
                    }
                    outCursor.InsertRow(rowBuffer);
                    inRow = cursor.NextRow();
                    ii++;
                    if ((ii % 100) == 0)
                    {
                        outCursor.Flush();
                    }
                }
                outCursor.Flush();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                // COMの解放
                if (cursor != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(cursor);
                if (outCursor != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(outCursor);
                if (pWorkspace != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pWorkspace);
                if (workspace2 != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(workspace2);
                if (featureWorkspace != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(featureWorkspace);
                if (rowBuffer != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(rowBuffer);
                if (table != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(table);
                if (fields != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(fields);
                if (fieldChecker != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(fieldChecker);
                if (enumFieldError != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(enumFieldError);
            }
        
        }

        /// <summary>
        /// テンポラリMDBでの集計処理およびCSVファイルへのエクスポート
        /// </summary>
        /// <param name="temporaryMDB">テンポラリMDBパス</param>
        /// <param name="sqlCommand">集計実行SQL</param>
        /// <param name="outFileName">CSV出力先パス</param>
        public static void ExportToCSV(String temporaryMDB, String sqlCommand, String outFileName)
        {
            // MDBからDataTableオブジェクトに値を取得
            DataTable dtTbl = new DataTable();
            GetDataTabl_FromMDB(temporaryMDB, sqlCommand, ref dtTbl);

            // ------------CSV出力------------
            System.IO.StreamWriter sw = null;
            try
            {
                if (File.Exists(outFileName))
                {
                    File.Delete(outFileName);
                }

                sw = new System.IO.StreamWriter(outFileName, false, Encoding.GetEncoding("Shift_jis"));
                int fieldCount = dtTbl.Columns.Count;
                int rowCount = dtTbl.Rows.Count;

                // タイトル出力
                StringBuilder strBuf = new StringBuilder("");
                for (int i = 0; i < fieldCount; i++)
                {
                    strBuf.Append(dtTbl.Columns[i].ColumnName);
                    if (i < fieldCount - 1) strBuf.Append(",");
                }
                strBuf.Append(Environment.NewLine);
                sw.Write(strBuf);
                // 各行の値を出力
                for (int i = 0; i < rowCount; i++)
                {
                    strBuf = new StringBuilder("");
                    for (int j = 0; j < fieldCount; j++)
                    {
                        strBuf.Append(dtTbl.Rows[i][j].ToString());
                        if (j < fieldCount - 1) strBuf.Append(",");
                    }
                    strBuf.Append(Environment.NewLine);
                    sw.Write(strBuf.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }
            }
        }
        
        /// <summary>
        /// テンポラリMDBでの集計処理およびDBFファイルへのエクスポート
        /// </summary>
        /// <param name="temporaryMDB">テンポラリMDBパス</param>
        /// <param name="sqlCommand">集計実行SQL</param>
        /// <param name="outFileName">出力先DBFファイルパス</param>
        public static void ExportToDBF(String temporaryMDB, String sqlCommand, String outFileName)
        {
            System.Data.OleDb.OleDbCommand oleCommand = null;
            System.Data.OleDb.OleDbConnection oleCon = null;
            DataTable dtTbl = new DataTable();
            
            try
            {
                if (File.Exists(outFileName))
                {
                    File.Delete(outFileName);
                }

                // MDBからDataTableオブジェクトに値を取得
                GetDataTabl_FromMDB(temporaryMDB, sqlCommand, ref dtTbl);

                // ------------DBFへエクスポート------------
                String output_path = 
                    System.IO.Path.GetDirectoryName(outFileName);
                String table_name = 
                    System.IO.Path.GetFileNameWithoutExtension(outFileName);

                oleCon = new System.Data.OleDb.OleDbConnection();
                oleCon.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                    "Data Source=" + output_path + ";" +
                    "Extended Properties=dBase IV";
                oleCon.Open();

                // テーブル作成SQL
                StringBuilder SQLCreateCommand = new StringBuilder("");
                SQLCreateCommand.Append("CREATE Table ");
                SQLCreateCommand.Append(table_name);
                SQLCreateCommand.Append(" (");
                for (int i = 0; i < dtTbl.Columns.Count; i++)
                {
                    SQLCreateCommand.Append(dtTbl.Columns[i].ColumnName);
                    SQLCreateCommand.Append(" ");
                    SQLCreateCommand.Append(
                        ExportFunctions2.GetFieldTypeString(dtTbl.Columns[i]));

                    if (i < dtTbl.Columns.Count - 1)
                    {
                        SQLCreateCommand.Append(",");
                    }
                }
                SQLCreateCommand.Append(")");
                oleCommand = 
                    new System.Data.OleDb.OleDbCommand(SQLCreateCommand.ToString(), oleCon);
                oleCommand.ExecuteNonQuery();

                // 行ごとの要素を挿入する
                for (int i = 0; i < dtTbl.Rows.Count; i++)
                {
                    StringBuilder str_fields_name = new StringBuilder("");
                    StringBuilder str_values = new StringBuilder("");
                    for (int j = 0; j < dtTbl.Columns.Count; j++)
                    {
                        str_fields_name.Append(dtTbl.Columns[j].ColumnName);
                        if (dtTbl.Rows[i][j].ToString() == "")
                        {
                            str_values.Append("NULL");
                        }
                        else if (dtTbl.Columns[j].DataType.ToString() == "System.String")
                        {
                            str_values.Append("'");
                            str_values.Append(dtTbl.Rows[i][j].ToString());
                            str_values.Append("'");
                        }
                        else
                        {
                            str_values.Append(dtTbl.Rows[i][j].ToString());
                        }

                        if (j < dtTbl.Columns.Count - 1)
                        {
                            str_fields_name.Append(",");
                            str_values.Append(",");
                        }
                    }
                    SQLCreateCommand = new StringBuilder("");
                    SQLCreateCommand.Append("insert into ");
                    SQLCreateCommand.Append(table_name);
                    SQLCreateCommand.Append(" (");
                    SQLCreateCommand.Append(str_fields_name);
                    SQLCreateCommand.Append(") values(");
                    SQLCreateCommand.Append(str_values);
                    SQLCreateCommand.Append(")");

                    oleCommand.CommandText = SQLCreateCommand.ToString();
                    oleCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (oleCon != null)
                {
                    oleCon.Close();
                    oleCon.Dispose();
                }

                if (dtTbl != null)
                {
                    dtTbl.Dispose();
                }
            }
        }

        /// <summary>
        /// テンポラリMDBでの集計処理およびMDBファイルへのエクスポート
        /// </summary>
        /// <param name="temporaryMDB">テンポラリMDBパス</param>
        /// <param name="sqlCommand">集計実行SQL</param>
        /// <param name="outFileName">出力先MDBファイルパス</param>
        /// <history>
        ///  2012-07-19 新規作成 (ej7035)
        ///  mdbに対応
        /// </history>
        public static void ExportToMDB(String temporaryMDB, String sqlCommand, String outFileName)
        {
            System.Data.OleDb.OleDbCommand oleCommand = null;
            System.Data.OleDb.OleDbConnection oleCon = null;
            DataTable dtTbl = new DataTable();

            try
            {
                // MDBからDataTableオブジェクトに値を取得
                GetDataTabl_FromMDB(temporaryMDB, sqlCommand, ref dtTbl);

                // ------------DBFへエクスポート------------
                String output_path =
                    System.IO.Path.GetDirectoryName(outFileName);
                String table_name =
                    System.IO.Path.GetFileNameWithoutExtension(outFileName);

                // テーブルが選択された場合
                if (output_path.IndexOf(".mdb") > 0)
                {
                    output_path = System.IO.Path.GetDirectoryName(output_path);
                    outFileName = outFileName.Substring(0, outFileName.LastIndexOf(@"\"));
                }

                if (!File.Exists(outFileName))
                {
                    // 空のmdb作成
                    CreateEmptyMDB(output_path, table_name + ".mdb");
                }

                oleCon = new System.Data.OleDb.OleDbConnection();
                oleCon.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                    "Data Source=" + outFileName + ";";
                oleCon.Open();

                // テーブル作成SQL
                StringBuilder SQLCreateCommand = new StringBuilder("");

                // テーブル存在チェック
                SQLCreateCommand.Append("SELECT count(*) FROM ");
                SQLCreateCommand.Append(table_name);
                oleCommand =
                    new System.Data.OleDb.OleDbCommand(SQLCreateCommand.ToString(), oleCon);

                try
                {
                    System.Data.OleDb.OleDbDataReader dataReader = oleCommand.ExecuteReader();
                    if (dataReader.HasRows == true)
                    {
                        dataReader.Close();
                        SQLCreateCommand = new StringBuilder("");
                        SQLCreateCommand.Append("DROP TABLE  ");
                        SQLCreateCommand.Append(table_name);
                        oleCommand.CommandText = SQLCreateCommand.ToString();
                        oleCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception oleEx)
                {
                    // エラーの時はテーブルが存在しない
                }

                SQLCreateCommand = new StringBuilder("");
                SQLCreateCommand.Append("CREATE Table ");
                SQLCreateCommand.Append(table_name);
                SQLCreateCommand.Append(" (");
                for (int i = 0; i < dtTbl.Columns.Count; i++)
                {
                    SQLCreateCommand.Append(dtTbl.Columns[i].ColumnName);
                    SQLCreateCommand.Append(" ");
                    SQLCreateCommand.Append(
                        ExportFunctions2.GetFieldTypeString(dtTbl.Columns[i]));

                    if (i < dtTbl.Columns.Count - 1)
                    {
                        SQLCreateCommand.Append(",");
                    }
                }
                SQLCreateCommand.Append(")");
                oleCommand =
                    new System.Data.OleDb.OleDbCommand(SQLCreateCommand.ToString(), oleCon);
                oleCommand.ExecuteNonQuery();

                // 行ごとの要素を挿入する
                for (int i = 0; i < dtTbl.Rows.Count; i++)
                {
                    StringBuilder str_fields_name = new StringBuilder("");
                    StringBuilder str_values = new StringBuilder("");
                    for (int j = 0; j < dtTbl.Columns.Count; j++)
                    {
                        str_fields_name.Append(dtTbl.Columns[j].ColumnName);
                        if (dtTbl.Rows[i][j].ToString() == "")
                        {
                            str_values.Append("NULL");
                        }
                        else if (dtTbl.Columns[j].DataType.ToString() == "System.String")
                        {
                            str_values.Append("'");
                            str_values.Append(dtTbl.Rows[i][j].ToString());
                            str_values.Append("'");
                        }
                        else
                        {
                            str_values.Append(dtTbl.Rows[i][j].ToString());
                        }

                        if (j < dtTbl.Columns.Count - 1)
                        {
                            str_fields_name.Append(",");
                            str_values.Append(",");
                        }
                    }
                    SQLCreateCommand = new StringBuilder("");
                    SQLCreateCommand.Append("insert into ");
                    SQLCreateCommand.Append(table_name);
                    SQLCreateCommand.Append(" (");
                    SQLCreateCommand.Append(str_fields_name);
                    SQLCreateCommand.Append(") values(");
                    SQLCreateCommand.Append(str_values);
                    SQLCreateCommand.Append(")");

                    oleCommand.CommandText = SQLCreateCommand.ToString();
                    oleCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (oleCon != null)
                {
                    oleCon.Close();
                    oleCon.Dispose();
                }

                if (dtTbl != null)
                {
                    dtTbl.Dispose();
                }
            }
        }

        /// <summary>
        /// テンポラリMDBでの集計処理およびGDBファイルへのエクスポート
        /// </summary>
        /// <param name="temporaryMDB">テンポラリMDBパス</param>
        /// <param name="sqlCommand">集計実行SQL</param>
        /// <param name="outFileName">出力先GDBファイルパス</param>
        public static void ExportToGDB(String temporaryMDB, String sqlCommand, String outFileName)
        {
            //System.Data.OleDb.OleDbCommand oleCommand = null;
            System.Data.OleDb.OleDbConnection oleCon = null;
            DataTable dtTbl = new DataTable();
            IWorkspace ws = null;

            try
            {
                // MDBからDataTableオブジェクトに値を取得
                GetDataTabl_FromMDB(temporaryMDB, sqlCommand, ref dtTbl);

                // ------------DBFへエクスポート------------
                String output_path =
                    System.IO.Path.GetDirectoryName(outFileName);
                String table_name =
                    System.IO.Path.GetFileNameWithoutExtension(outFileName);

                // テーブルが選択された場合
                if (output_path.IndexOf(".gdb") > 0)
                {
                    output_path = System.IO.Path.GetDirectoryName(output_path);
                    outFileName = outFileName.Substring(0, outFileName.LastIndexOf(@"\"));
                }

                if (!Directory.Exists(outFileName))
                {
                    // 空のgdb作成
                    ws = CreateEmptyGDB(output_path, table_name + ".gdb");
                }
                else
                {
                    IWorkspaceFactory workspaceFactory = SingletonUtility.NewFileGDBWorkspaceFactory(); // new FileGDBWorkspaceFactoryClass();
                    ws = workspaceFactory.OpenFromFile(outFileName, 0);
                }

                IFeatureWorkspace fws = ws as IFeatureWorkspace;
                IWorkspace2 ws2 = ws as IWorkspace2;

                // テーブル存在チェック
                if (ws2.get_NameExists(esriDatasetType.esriDTTable, table_name) == true)
                {
                    IEnumDataset enumDataset = ws.get_Datasets(esriDatasetType.esriDTTable);
                    IDataset dataSet = enumDataset.Next();
                    while (dataSet != null)
                    {
                        if (dataSet.Name == table_name && dataSet.CanDelete() == true)
                        {
                            dataSet.Delete();
                        }
                        dataSet = enumDataset.Next();
                    }
                }

                IField pfield;
                IFields pfields = new Fields();
                IFieldEdit pfieidEdit;
                IFieldsEdit pfieidsEdit = pfields as IFieldsEdit;

                for (int i = 0; i < dtTbl.Columns.Count; i++)
                {
                    pfield = new Field();
                    pfieidEdit = pfield as IFieldEdit;

                    pfieidEdit.Name_2 = dtTbl.Columns[i].ColumnName;

                    if (ExportFunctions2.GetFieldTypeString(dtTbl.Columns[i]) == "INTEGER")
                    {
                        pfieidEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
                    }
                    else if (ExportFunctions2.GetFieldTypeString(dtTbl.Columns[i]) == "LONG")
                    {
                        pfieidEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                    }
                    else if (ExportFunctions2.GetFieldTypeString(dtTbl.Columns[i]) == "DOUBLE")
                    {
                        pfieidEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                    }
                    else if (ExportFunctions2.GetFieldTypeString(dtTbl.Columns[i]) == "TEXT(254)")
                    {
                        pfieidEdit.Type_2 = esriFieldType.esriFieldTypeString;
                        pfieidEdit.Length_2 = 254;
                    }
                    pfieidsEdit.AddField(pfield);
                }

                pfields = pfieidsEdit;

                UID clsID = new UIDClass();
                clsID.Value = "esriGeoDatabase.Object";

                

                // テーブル作成
                fws.CreateTable(table_name, pfields, clsID, null, "");

                // データ作成SQL
                StringBuilder SQLCreateCommand = new StringBuilder("");

                // 行ごとの要素を挿入する
                for (int i = 0; i < dtTbl.Rows.Count; i++)
                {
                    StringBuilder str_fields_name = new StringBuilder("");
                    StringBuilder str_values = new StringBuilder("");
                    for (int j = 0; j < dtTbl.Columns.Count; j++)
                    {
                        str_fields_name.Append(dtTbl.Columns[j].ColumnName);
                        if (dtTbl.Rows[i][j].ToString() == "")
                        {
                            str_values.Append("NULL");
                        }
                        else if (dtTbl.Columns[j].DataType.ToString() == "System.String")
                        {
                            str_values.Append("'");
                            str_values.Append(dtTbl.Rows[i][j].ToString());
                            str_values.Append("'");
                        }
                        else
                        {
                            str_values.Append(dtTbl.Rows[i][j].ToString());
                        }

                        if (j < dtTbl.Columns.Count - 1)
                        {
                            str_fields_name.Append(",");
                            str_values.Append(",");
                        }
                    }
                    SQLCreateCommand = new StringBuilder("");
                    SQLCreateCommand.Append("insert into ");
                    SQLCreateCommand.Append(table_name);
                    SQLCreateCommand.Append(" (");
                    SQLCreateCommand.Append(str_fields_name);
                    SQLCreateCommand.Append(") values(");
                    SQLCreateCommand.Append(str_values);
                    SQLCreateCommand.Append(")");

                    ws.ExecuteSQL(SQLCreateCommand.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (oleCon != null)
                {
                    oleCon.Close();
                    oleCon.Dispose();
                }

                if (dtTbl != null)
                {
                    dtTbl.Dispose();
                }
            }
        }

        /// <summary>
        /// 結合されたフィールドのフィールド名のみを取得する
        /// </summary>
        /// <param name="field_name">テーブル結合状態のフィールド名</param>
        /// <returns>フィールド名</returns>
        public static String GetFieldName(String field_name)
        {
            String[] split_name = field_name.Split('.');
            return split_name[split_name.Length - 1];
        }

        /// <summary>
        /// 空のMDBを新規作成する
        /// </summary>
        /// <param name="workspace_mdb">MDBファイルのパス</param>
        /// <param name="name_mdb">MDBファイル名</param>
        private static IWorkspace CreateEmptyMDB(String workspace_mdb, String name_mdb)
        {
            IWorkspaceFactory workspaceFactory = null;
            IWorkspaceName workspaceName = null;
            IName name = null;
            IWorkspace workspace = null;

            try
            {
                workspaceFactory = SingletonUtility.NewAccessWorkspaceFactory(); //new AccessWorkspaceFactoryClass();
                workspaceName = workspaceFactory.Create(workspace_mdb, name_mdb, null, 0);

                name = (IName)workspaceName;
                workspace = (IWorkspace)name.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                //ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(workspaceFactory);
                if (workspaceName != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(workspaceName);
                if (name != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(name);
            }
            return workspace;
        }

        /// <summary>
        /// 空のGDBを新規作成する
        /// </summary>
        /// <param name="workspace_mdb">GDBファイルのパス</param>
        /// <param name="name_mdb">GDBファイル名</param>
        private static IWorkspace CreateEmptyGDB(String workspace_gdb, String name_gdb)
        {
            IWorkspaceFactory workspaceFactory = null;
            IWorkspaceName workspaceName = null;
            IName name = null;
            IWorkspace workspace = null;

            try
            {
                workspaceFactory = SingletonUtility.NewFileGDBWorkspaceFactory(); // new FileGDBWorkspaceFactoryClass();
                workspaceName = workspaceFactory.Create(workspace_gdb, name_gdb, null, 0);

                name = (IName)workspaceName;
                workspace = (IWorkspace)name.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                //ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(workspaceFactory);
                if (workspaceName != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(workspaceName);
                if (name != null)
                    ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(name);
            }
            return workspace;
        }

        /// <summary>
        /// MDBファイルのテーブルをDataTableオブジェクトに取得する
        /// </summary>
        /// <param name="dataSourceName">MDBファイルのパス</param>
        /// <param name="sqlCommand">テーブル取得時の抽出条件となるSQLコマンド</param>
        /// <param name="dtTbl">DataTableオブジェクト</param>
        public static void GetDataTabl_FromMDB(String dataSourceName, String sqlCommand, ref System.Data.DataTable dtTbl)
        {
            System.Data.OleDb.OleDbConnection dbCon = new System.Data.OleDb.OleDbConnection();
            System.Data.OleDb.OleDbCommand dbCmd;

            try
            {
                dbCon.ConnectionString =
                    "Provider=Microsoft.Jet.OLEDB.4.0;" +
                    "Data Source=" + dataSourceName + ";" +
                    "Persist Security Info=false";
                dbCon.Open();

                dtTbl = new DataTable();
                dbCmd = new System.Data.OleDb.OleDbCommand(sqlCommand, dbCon);
                System.Data.OleDb.OleDbDataAdapter dbAdp = 
                    new System.Data.OleDb.OleDbDataAdapter(dbCmd);
                dbAdp.Fill(dtTbl);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                dbCon.Close();
                dbCon.Dispose();
            }
        }

        /// <summary>
        /// System型からACCESSのSQLのデータ型文字列を返す
        /// </summary>
        /// <param name="in_column"></param>
        /// <returns></returns>
        private static String GetFieldTypeString(DataColumn in_column)
        {
            String strDataType = in_column.DataType.ToString();

            if (strDataType == "System.Int16")
            {
                strDataType = "INTEGER";
            }
            else if (strDataType == "System.Int32" || strDataType == "System.Int64")
            {
                strDataType = "LONG";
            }
            else if (strDataType == "System.Single" || strDataType == "System.Double")
            {
                strDataType = "DOUBLE";
            }
            else if (strDataType == "System.String")
            {
                strDataType = "TEXT(254)";
            }
            return strDataType;
        }
    }
}
