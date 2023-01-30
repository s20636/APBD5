using Cwiczenia5.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Cwiczenia5.Services
{
    public class DbService: IDbService
    {
        private readonly IConfiguration _configuration;
        public DbService(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public async Task<int> RegisterProductInWarehouseAsync(Warehouse warehouse)
        {
            if (warehouse.Amount <= 0)
            {
                throw new ArgumentException("Ilość nie jest większa od 0.");
            }
            using var con = new SqlConnection(_configuration.GetConnectionString("DefaultDbCon"));
            using var com = new SqlCommand();
            com.Connection = con;
            await con.OpenAsync();
            com.CommandText = "SELECT COUNT(idProduct) FROM Product WHERE idProduct = @idProduct";
            com.Parameters.AddWithValue("@idProduct", warehouse.IdProduct);
            int result = (int)await com.ExecuteScalarAsync();
            if (result == 0) 
            {
                throw new ArgumentException("Produkt o podanym id nie istnieje.");
            }
            com.CommandText = "SELECT COUNT(idWarehouse) FROM Warehouse WHERE idWarehouse = @idWarehouse";
            com.Parameters.AddWithValue("@idWarehouse", warehouse.IdWarehouse);
            result = (int)await com.ExecuteScalarAsync();
            if (result == 0)
            {
                throw new ArgumentException("Hurtownia o podanym id nie istnieje.");
            }
            com.CommandText = "SELECT COUNT(idOrder) FROM [Order] WHERE idProduct = @idProduct AND amount = @amount";
            com.Parameters.AddWithValue("@amount", warehouse.Amount);
            result = (int)await com.ExecuteScalarAsync();
            if (result == 0)
            {
                throw new ArgumentException("W bazie nie ma zlecenia zakupu produktu o podanym id.");
            }
            com.CommandText = "SELECT CreatedAt FROM [Order] WHERE idProduct = @idProduct AND amount = @amount";
            DateTime date = (DateTime)await com.ExecuteScalarAsync();
            if (date > warehouse.CreatedAt)
            {
                throw new ArgumentException("Zbyt wczesny termin zamówienia w żądaniu.");
            }
            com.CommandText = "SELECT COUNT(idProductWarehouse) FROM Product_Warehouse WHERE idOrder = " +
                "(SELECT idOrder FROM [Order] WHERE idProduct = @idProduct AND amount = @amount)";
            result = (int)await com.ExecuteScalarAsync();
            if (result > 0)
            {
                throw new ArgumentException("Zlecenie zostało już zrealizowane.");
            }
            result = 0;
            SqlTransaction tran = (SqlTransaction)await con.BeginTransactionAsync();
            com.Transaction = tran;
            try
            {
                com.CommandText = "UPDATE [Order] SET FulfilledAt = @createdAt WHERE idOrder = " +
                    "(SELECT idOrder FROM [Order] WHERE idProduct =@idProduct AND amount =@amount)";
                com.Parameters.AddWithValue("@createdAt", warehouse.CreatedAt);
                await com.ExecuteNonQueryAsync();
                com.CommandText = "SELECT Price FROM Product WHERE idProduct = @idProduct";
                com.Parameters.AddWithValue("@price", await com.ExecuteScalarAsync());
                com.CommandText = "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                    "VALUES(@idWarehouse, @idProduct, (SELECT idOrder FROM [Order] WHERE idProduct = @idProduct AND amount = @amount)" +
                    ", @amount, @amount * @price, @createdAt)";
                
                await com.ExecuteNonQueryAsync();
                await tran.CommitAsync();
                com.CommandText = "SELECT idProductWarehouse FROM Product_Warehouse WHERE idProduct = @idProduct AND idWarehouse = @idWarehouse";
                result = (int)await com.ExecuteScalarAsync();
            }
            catch (SqlException ex) 
            {
                await tran.RollbackAsync(); 
            }
            return result;

        }

        public async Task<int> RegisterProductInWarehouseWithProcedureAsync(Warehouse warehouse)
        {
            using var con = new SqlConnection(_configuration.GetConnectionString("DefaultDbCon"));
            using var com = new SqlCommand("AddProductToWarehouse", con);
            com.Connection = con;
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@idProduct", warehouse.IdProduct);
            com.Parameters.AddWithValue("@amount", warehouse.Amount);
            com.Parameters.AddWithValue("@idWarehouse", warehouse.IdWarehouse);
            com.Parameters.AddWithValue("@createdAt", warehouse.CreatedAt);
            await con.OpenAsync();
            return await com.ExecuteNonQueryAsync(); ;
        }
    }
}
