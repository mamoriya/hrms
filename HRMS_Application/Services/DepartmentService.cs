using HRMS_Application.Models;
using HRMS_Application.Repositories;

namespace HRMS_Application.Services
{
    public class DepartmentService
    {
        private readonly DepartmentRepository _repository;

        public DepartmentService(DepartmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<Department?> CreateDepartment(Department department)
        {
            if (department == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(department.Name))
            {
                return null;
            }

            if (await _repository.NameExists(department.Name))
            {
                return null;
            }

            return await _repository.CreateDepartment(department);
        }

        public async Task<List<Department>> GetAllDepartments()
        {
            return await _repository.GetAllDepartments();
        }

        public async Task<Department?> UpdateDepartment(
            int id,
            Department department)
        {
            var existingDepartment =
                await _repository.GetDepartmentById(id);

            if (existingDepartment == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(department.Name))
            {
                return null;
            }

            existingDepartment.Name = department.Name;

            return await _repository.UpdateDepartment(existingDepartment);
        }

        public async Task<bool> DeleteDepartment(int id)
        {
            var department =
                await _repository.GetDepartmentById(id);

            if (department == null)
            {
                return false;
            }

            await _repository.DeleteDepartment(department);

            return true;
        }
    }
}
