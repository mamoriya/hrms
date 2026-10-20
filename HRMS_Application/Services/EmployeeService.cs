using HRMS_Application.Models;
using HRMS_Application.Repositories;

namespace HRMS_Application.Services
{
    public class EmployeeService
    {
        private readonly EmployeeRepository _repository;

        public EmployeeService(EmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<EmployeeProfile> CreateEmployee(CreateEmployeeRequest request)
        {
            if (request == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(request.LastName))
            {
                return null;
            }

            if (await _repository.EmailExists(request.Email))
            {
                return null;
            }

            if (!await _repository.DepartmentExists(request.DepartmentId))
            {
                return null;
            }
            if (request.Role != "Employee" && request.Role != "Manager")
            {
                return null;
            }

            var user = new User
            {
                Email = request.Email,
                Password = request.Password,
                Role = request.Role
            };


            

            var employee = new EmployeeProfile
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                JobTitle = request.JobTitle,
                DepartmentId = request.DepartmentId,
                ManagerId = request.ManagerId,
                JoiningDate = request.JoiningDate
            };

            return await _repository.CreateEmployeeWithLeaveBalances(
                user,
                employee);

        }
        public async Task<List<EmployeeProfile>> GetAllEmployees()
        {
            return await _repository.GetAllEmployees();
        }
        public async Task<EmployeeProfile?> UpdateEmployee(int id, EmployeeProfile employee)
        {
            var existingEmployee = await _repository.GetEmployeeById(id);

            if (existingEmployee == null)
            {
                return null;
            }

            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.PhoneNumber = employee.PhoneNumber;
            existingEmployee.Address = employee.Address;

            return await _repository.UpdateEmployee(existingEmployee);
        }

        public async Task<int> UpdateEmployee( int id, UpdateEmployeeRequest request)
        {
            var employee = await _repository.GetEmployeeById(id);

            if (employee == null)
            {
                return 0;
            }

            if (request.Role != "Employee" &&
                request.Role != "Manager")
            {
                return 2;
            }

            bool changingManagerToEmployee =
                employee.User.Role == "Manager" &&
                request.Role == "Employee";

            if (changingManagerToEmployee)
            {
                await _repository.ClearManagerFromDirectReports(id);

                employee.ManagerId = null;
            }
            else
            {
                employee.ManagerId = request.ManagerId;
            }

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.PhoneNumber = request.PhoneNumber;
            employee.Address = request.Address;
            employee.JobTitle = request.JobTitle;
            employee.DepartmentId = request.DepartmentId;
            employee.JoiningDate = request.JoiningDate;
            employee.User.Role = request.Role;

            await _repository.UpdateEmployee(employee);

            return 1;
        }

        public async Task<EmployeeProfile?> GetEmployeeById(int id)
        {
            return await _repository.GetEmployeeById(id);
        }
        public async Task<EmployeeProfile> UpdateEmployeeProfile( EmployeeProfile employee)
        {
            return await _repository.UpdateEmployee(employee);
        }


        public async Task<EmployeeProfile?> GetEmployeeByUserId(int userId)
        {
            return await _repository.GetEmployeeByUserId(userId);
        }


        public async Task<bool> IsEmployeeOwner(int employeeId, int userId)
        {
            var employee = await _repository.GetEmployeeById(employeeId);

            if (employee == null)
            {
                return false;
            }

            return employee.UserId == userId;
        }

        public async Task<int> DeleteEmployee(int id)
        {
            var employee = await _repository.GetEmployeeById(id);

            if (employee == null)
            {
                return 0;
            }

            var hasDirectReports = await _repository.HasDirectReports(id);

            if (hasDirectReports)
            {
                return 2;
            }

            await _repository.DeleteEmployeeWithRelatedData(id);

            return 1;
        }

        public async Task<int> DeleteEmployeeWithNewManager( int employeeId, int? newManagerId)
        {
            var employee = await _repository.GetEmployeeById(employeeId);

            if (employee == null)
            {
                return 0;
            }

            var hasDirectReports =
                await _repository.HasDirectReports(employeeId);

            if (!hasDirectReports)
            {
                await _repository.DeleteEmployeeWithRelatedData(employeeId);

                return 1;
            }

            // Only validate new manager if one was selected
            if (newManagerId.HasValue)
            {
                var isValidManager =
                    await IsValidManager(newManagerId.Value);

                if (!isValidManager)
                {
                    return 2;
                }

                if (employeeId == newManagerId.Value)
                {
                    return 3;
                }
            }

            // newManagerId can be:
            // - actual Manager ID
            // - null for last-manager case
            await _repository.DeleteEmployeeWithRelatedData(
                employeeId,
                newManagerId
            );

            return 1;
        }

        public async Task<List<EmployeeProfile>> GetDirectReports(int managerId)
        {
            return await _repository.GetDirectReports(managerId);
        }

        public async Task<bool> IsValidManager(int managerId)
        {
            var manager = await _repository.GetManagerById(managerId);

            return manager != null;
        }

        public async Task<bool> ReassignEmployeesToNewManager(int oldManagerId, int newManagerId)
        {
            var oldManager = await _repository.GetEmployeeById(oldManagerId);

            if (oldManager == null)
            {
                return false;
            }

            var newManager = await _repository.GetManagerById(newManagerId);

            if (newManager == null)
            {
                return false;
            }

            if (oldManagerId == newManagerId)
            {
                return false;
            }

            await _repository.ReassignDirectReports(
                oldManagerId,
                newManagerId
            );

            return true;
        }
    }
}
