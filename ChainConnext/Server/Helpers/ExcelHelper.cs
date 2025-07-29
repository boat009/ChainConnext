using ChainConnext.Shared;
using ExcelDataReader;

namespace ChainConnext.Server.Helpers
{
    public class ExcelHelper
    {
        public static ExecResult ImportExcel(string path)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (var stream = File.OpenRead(path))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet(
                            new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            }
                            );
                        Rs.IsSuccess = true;
                        Rs.JsonData = BaseShared.DataTableToJson(result.Tables[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("because it is being used by another process"))
                {
                    Rs.Msg = "ไฟล์ Excel เปิดอยู่ กรุณาปิดไฟล์ ด้วย";
                }
                else if (ex.Message.Contains("A column named"))
                {
                    string messageerror = ex.Message.Replace("A column named '", string.Empty).Replace("' already belongs to this DataTable.", string.Empty);
                    Rs.Msg = string.Format("หัว Column ชื่อ {0} ในไฟล์ มีซ้ำกันอยู่ กรุณาเปลี่ยนชื่อหัว Column ชื่อ {0} ที่อยู่ถัดไปเป็น {0}1", messageerror);
                }
            }
            return Rs;
        }
        public static ExecResult ImportExcels(string path)
        {
            ExecResult Rs = new ExecResult();
            Rs.IsSuccess = false;
            try
            {
                using (var stream = File.OpenRead(path))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet(
                            new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            }
                            );
                        Rs.IsSuccess = true;
                        Rs.JsonData = BaseShared.DataSetToJson(result);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("because it is being used by another process"))
                {
                    Rs.Msg = "ไฟล์ Excel เปิดอยู่ กรุณาปิดไฟล์ ด้วย";
                }
                else if (ex.Message.Contains("A column named"))
                {
                    string messageerror = ex.Message.Replace("A column named '", string.Empty).Replace("' already belongs to this DataTable.", string.Empty);
                    Rs.Msg = string.Format("หัว Column ชื่อ {0} ในไฟล์ มีซ้ำกันอยู่ กรุณาเปลี่ยนชื่อหัว Column ชื่อ {0} ที่อยู่ถัดไปเป็น {0}1", messageerror);
                }
            }
            return Rs;
        }
    }
}
