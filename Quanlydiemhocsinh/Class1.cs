using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class DatabaseHelper
{

    private static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\sql\\Quanlydiemhocsinh\\Quanlydiemhocsinh\\Quanlidiem.mdf;Integrated Security=True";

    public static SqlConnection GetConnection()
    {
        SqlConnection connection = new SqlConnection(connectionString);
        try
        {
            connection.Open();
            return connection;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi kết nối cơ sở dữ liệu: " + ex.Message);
            return null;
        }
    }
    
    public static void CloseConnection(SqlConnection connection)
    {
        if (connection != null && connection.State == ConnectionState.Open)
        {
            connection.Close();
        }
    }

    public static List<Lop> GetDanhSachLop()
    {
        List<Lop> danhSachLop = new List<Lop>();
        SqlConnection connection = GetConnection();

        if (connection != null)
        {
            try
            {
                string query = "SELECT LopID, TenLop FROM Lop";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    danhSachLop.Add(new Lop
                    {
                        LopID = reader["LopID"].ToString(),
                        TenLop = reader["TenLop"].ToString()
                    });
                }

                reader.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Lỗi SQL khi lấy danh sách lớp: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khác khi lấy danh sách lớp: " + ex.Message);
            }
            finally
            {
                CloseConnection(connection);
            }
        }
        return danhSachLop;
    }

    public static List<DiemHocSinh> GetDiemHocSinhTheoLopVaMonHoc(string lopID, string monHoc, string hocKy)
    {
        List<DiemHocSinh> danhSachDiem = new List<DiemHocSinh>();
        SqlConnection connection = GetConnection();

        if (connection != null)
        {
            try
            {
                string query = @"
                SELECT
                    hs.ID AS HocSinhID,
                    hs.HoTen AS TenHocSinh,
                    d.MonHoc AS TenMonHoc,
                    d.HocKy,
                    d.NamHoc,
                    d.TX1,
                    d.TX2,
                    d.TX3,
                    d.TX4,
                    d.GiuaKy,
                    d.CuoiKy,
                    d.DiemID
                FROM HocSinh hs
                INNER JOIN Diem d ON hs.ID = d.HocSinhID
                WHERE hs.LopID = @LopID AND d.MonHoc = @MonHoc AND d.HocKy = @HocKy
                ORDER BY hs.HoTen;
            ";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@LopID", lopID);
                command.Parameters.AddWithValue("@MonHoc", monHoc);
                command.Parameters.AddWithValue("@HocKy", hocKy);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    danhSachDiem.Add(new DiemHocSinh
                    {
                        HocSinhID = reader["HocSinhID"].ToString(),
                        TenHocSinh = reader["TenHocSinh"].ToString(),
                        TenMonHoc = reader["TenMonHoc"].ToString(),
                        HocKy = reader["HocKy"].ToString(),
                        NamHoc = reader["NamHoc"].ToString(),
                        TX1 = reader["TX1"] == DBNull.Value ? (double?)null : Convert.ToDouble(reader["TX1"]),
                        TX2 = reader["TX2"] == DBNull.Value ? (double?)null : Convert.ToDouble(reader["TX2"]),
                        TX3 = reader["TX3"] == DBNull.Value ? (double?)null : Convert.ToDouble(reader["TX3"]),
                        TX4 = reader["TX4"] == DBNull.Value ? (double?)null : Convert.ToDouble(reader["TX4"]),
                        GiuaKy = reader["GiuaKy"] == DBNull.Value ? (double?)null : Convert.ToDouble(reader["GiuaKy"]),
                        CuoiKy = reader["CuoiKy"] == DBNull.Value ? (double?)null : Convert.ToDouble(reader["CuoiKy"]),
                        DiemID = Convert.ToInt32(reader["DiemID"])
                    });
                }

                reader.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Lỗi SQL khi lấy điểm học sinh: " + ex.Message);
            }
            finally
            {
                CloseConnection(connection);
            }
        }
        return danhSachDiem;
    }

    public static bool CapNhatDiem(int diemID, double? tx1, double? tx2, double? tx3, double? tx4, double? giuaKy, double? cuoiKy)
    {
        SqlConnection connection = GetConnection();
        if (connection != null)
        {
            try
            {
                string query = @"
                UPDATE Diem 
                SET TX1 = @TX1, 
                    TX2 = @TX2, 
                    TX3 = @TX3, 
                    TX4 = @TX4, 
                    GiuaKy = @GiuaKy, 
                    CuoiKy = @CuoiKy 
                WHERE DiemID = @DiemID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DiemID", diemID);
                command.Parameters.AddWithValue("@TX1", tx1 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@TX2", tx2 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@TX3", tx3 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@TX4", tx4 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@GiuaKy", giuaKy ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CuoiKy", cuoiKy ?? (object)DBNull.Value);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Lỗi SQL khi cập nhật điểm: " + ex.Message);
                return false;
            }
            finally
            {
                CloseConnection(connection);
            }
        }
        return false;
    }
    }

    

public class Lop
{
    public string LopID { get; set; }
    public string TenLop { get; set; }
}

public class DiemHocSinh
{
    public string HocSinhID { get; set; }
    public string TenHocSinh { get; set; }
    public string TenMonHoc { get; set; }
    public string HocKy { get; set; }
    public string NamHoc { get; set; }
    public double? TX1 { get; set; }
    public double? TX2 { get; set; }
    public double? TX3 { get; set; }
    public double? TX4 { get; set; }
    public double? GiuaKy { get; set; }
    public double? CuoiKy { get; set; }
    public int DiemID { get; set; }
}