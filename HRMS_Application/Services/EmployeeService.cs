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

        public async Task<EmployeeProfile> CreateEmployee(EmployeeProfile employee)
        {
            if (employee == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(employee.FirstName))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(employee.LastName))
            {
                return null;
            }
            if (await _repository.EmailExists(employee.User.Email))
            {
                return null;
            }
            if (!await _repository.DepartmentExists(employee.DepartmentId))
            {
                return null;
            }

            return await _repository.CreateEmployee(employee);
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
    }
}
