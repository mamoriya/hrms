using HRMS_Application.Data;
using HRMS_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Application.Repositories
{
    public class EmployeeDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeDocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EmployeeExists(int employeeId)
        {
            return await _context.EmployeeProfiles
                .AnyAsync(e => e.Id == employeeId);
        }

        public async Task<EmployeeDocument> AddDocument(
            EmployeeDocument document)
        {
            _context.EmployeeDocuments.Add(document);

            await _context.SaveChangesAsync();

            return document;
        }

        public async Task<List<EmployeeDocument>> GetDocumentsByEmployeeId(
            int employeeId)
        {
            return await _context.EmployeeDocuments
                .Where(d => d.EmployeeProfileId == employeeId)
                .ToListAsync();
        }

        public async Task<EmployeeDocument?> GetDocumentById(
            int employeeId,
            int documentId)
        {
            return await _context.EmployeeDocuments
                .FirstOrDefaultAsync(d =>
                    d.Id == documentId &&
                    d.EmployeeProfileId == employeeId);
        }

        public async Task DeleteDocument(EmployeeDocument document)
        {
            _context.EmployeeDocuments.Remove(document);

            await _context.SaveChangesAsync();
        }


        public async Task<EmployeeDocument?> GetDocumentById(int documentId)
        {
            return await _context.EmployeeDocuments
                .FirstOrDefaultAsync(d => d.Id == documentId);
        }
      
    }
}
