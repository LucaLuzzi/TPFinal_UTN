using Microsoft.Data.SqlClient;
using TPFinal_LuzziLuca.Models;

namespace TPFinal_LuzziLuca.Services
{
    public interface IRepositoryEmployees
    {
        Task DeleteEmployee(int id);
        Task<bool> Exist(string Fullname);
        Task<Employee> GetEmployeeById(int id);
        Task<List<Employee>> GetEmployees();
        Task<Job> GetJobById_Employee(int id);
        Task InsertJobAndSalary(string Fullname, string JobName, decimal Salary);
        Task insertNewEmployee(Employee employee);
        Task UpdateEmployee(Employee employee);
    }

    public class RepositoryEmployees : IRepositoryEmployees
    {
        private readonly string _connectionString;
        public RepositoryEmployees(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task insertNewEmployee(Employee employee)
        {
            string query = "insert_newEmployee";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    SqlParameter fullname = new SqlParameter("@Fullname", System.Data.SqlDbType.NVarChar, 50);
                    fullname.Value = employee.Fullname;
                    cmd.Parameters.Add(fullname);

                    SqlParameter email = new SqlParameter("@Email", System.Data.SqlDbType.NVarChar, 50);
                    email.Value = employee.Email;
                    cmd.Parameters.Add(email);

                    SqlParameter age = new SqlParameter("@Age", System.Data.SqlDbType.NVarChar, 50);
                    age.Value = employee.Age;
                    cmd.Parameters.Add(age);

                    try
                    {
                        await connection.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("Error de SQL Server: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public async Task InsertJobAndSalary(string Fullname, string JobName, decimal Salary)
        {
            string query = "insert_newJob";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    int Id_Employee = await FindEmployee(Fullname);

                    SqlParameter jobName = new SqlParameter("@JobName", System.Data.SqlDbType.NVarChar, 50);
                    jobName.Value = JobName;
                    cmd.Parameters.Add(jobName);

                    SqlParameter id_Employee = new SqlParameter("@Id_Employee", System.Data.SqlDbType.Int);
                    id_Employee.Value = Id_Employee;
                    cmd.Parameters.Add(id_Employee);

                    SqlParameter salary = new SqlParameter("@Salary", System.Data.SqlDbType.Decimal);
                    salary.Value = Salary;
                    cmd.Parameters.Add(salary);

                    try
                    {
                        await connection.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("Error de SQL Server: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

            }
        }

        private async Task<int> FindEmployee(string Fullname)
        {
            string query = @"SELECT Id FROM Employees WHERE Fullname = @Fullname";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Fullname", Fullname);
                    try
                    {
                        await connection.OpenAsync();
                        int id = (int) await cmd.ExecuteScalarAsync();
                        return id;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                    finally { connection.Close(); }
                }
            }
        }

        public async Task<bool> Exist(string Fullname)
        {
            string query = "SELECT CAST(CASE WHEN (SELECT COUNT(*) FROM Employees WHERE Fullname = @Fullname) > 0 THEN 1 ELSE 0 END AS BIT)";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        try
                        {
                            cmd.Parameters.AddWithValue("@Fullname", Fullname);

                            await connection.OpenAsync();
                            bool exist = (bool)await cmd.ExecuteScalarAsync();

                            if (exist)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            throw;
                        }
                        finally { connection.Close(); }
                    }
                }
        }

        public async Task<List<Employee>> GetEmployees()
        {
            List<Employee> employees = new List<Employee>();
            string query = @"SELECT e.Id, e.Fullname, e.Email, e.Age, j.Name, j.Salary FROM Employees e INNER JOIN Jobs j ON e.Id = j.Id_Employee";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Employee employee = new Employee();

                                employee.Id = Convert.ToInt32(reader["Id"]);
                                employee.Fullname = Convert.ToString(reader["Fullname"]);
                                employee.Email = reader["Email"].ToString();
                                employee.Age = Convert.ToInt32(reader["Age"]);
                                employee.JobName = reader["Name"].ToString();
                                employee.Salary = Convert.ToDecimal(reader["Salary"]);

                                employees.Add(employee);
                            }
                        }
                        return employees;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                    finally { connection.Close(); }
                }
            }
        }

        public async Task UpdateEmployee(Employee employee)
        {
            string query = @"Update_Employee";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Fullname", employee.Fullname);
                    cmd.Parameters.AddWithValue("@Id", employee.Id);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@Age", employee.Age);
                    cmd.Parameters.AddWithValue("@JobName", employee.JobName);
                    cmd.Parameters.AddWithValue("@Salary", employee.Salary);
                    try
                    {
                        await connection.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                    finally { connection.Close(); }
                }
            }
        }

        public async Task DeleteEmployee(int id)
        {
            string query = @"Delete_Employee";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    try
                    {
                        await connection.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                    finally { connection.Close(); }
                }
            }
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            string query = "SELECT * FROM Employees WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    try
                    {
                        await connection.OpenAsync();
                        Employee employee = new Employee();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                employee.Fullname = Convert.ToString(reader["Fullname"]);
                                employee.Email = reader["Email"].ToString();
                                employee.Age = Convert.ToInt32(reader["Age"]);
                            }
                        }
                        return employee;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                    finally { connection.Close(); }
                }
            }
        }

        public async Task<Job> GetJobById_Employee(int id)
        {
            string query = "SELECT * FROM Jobs WHERE Id_Employee = @Id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    try
                    {
                        await connection.OpenAsync();
                        Job job = new Job();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                job.Name = Convert.ToString(reader["Name"]);
                                job.Salary = Convert.ToDecimal(reader["Salary"]);
                            }
                        }
                        return job;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                    finally { connection.Close(); }
                }
            }
        }
    }
}
