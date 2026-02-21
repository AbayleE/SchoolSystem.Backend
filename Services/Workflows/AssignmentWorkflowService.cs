using System;
using System.Threading.Tasks;
using SchoolSystem.Backend.Services;
using SchoolSystem.Backend.DTOs;
using Microsoft.AspNetCore.Http;

namespace SchoolSystem.Backend.Services.Workflows
{
    public class AssignmentWorkflowService
    {
        private readonly AssignmentService _assignmentService;
        private readonly FileResourceService _fileService;
        private readonly NotificationService _notificationService;
        private readonly EnrollmentService _enrollmentService;
        private readonly GradeService _gradeService;

        public AssignmentWorkflowService(
            AssignmentService assignmentService,
            FileResourceService fileService,
            NotificationService notificationService,
            EnrollmentService enrollmentService,
            GradeService gradeService)
        {
            _assignmentService = assignmentService;
            _fileService = fileService;
            _notificationService = notificationService;
            _enrollmentService = enrollmentService;
            _gradeService = gradeService;
        }

        // ---------------------------------------------------------
        // Student submits assignment
        // ---------------------------------------------------------
        public async Task SubmitAssignment(Guid studentId, Guid assignmentId, IFormFile file)
        {
            var assignment = await _assignmentService.GetByIdAsync(assignmentId);

            if (assignment == null)
                throw new Exception("Assignment not found");

            var isEnrolled = await _enrollmentService.IsStudentInClass(studentId, assignment.ClassId);
            if (!isEnrolled)
                throw new Exception("Student is not enrolled in this class");

            var fileResource = await _fileService.UploadAsync(file);

            await _assignmentService.AddSubmission(studentId, assignmentId, fileResource.Id);

            await _notificationService.NotifyTeacher(
                assignment.TeacherId,
                $"A student submitted work for: {assignment.Title}"
            );
        }

        // ---------------------------------------------------------
        // Teacher grades submission
        // ---------------------------------------------------------
        public async Task GradeSubmission(Guid teacherId, Guid submissionId, int score, string feedback)
        {
            var submission = await _assignmentService.GetSubmission(submissionId);

            if (submission == null)
                throw new Exception("Submission not found");

            if (submission.TeacherId != teacherId)
                throw new Exception("You are not authorized to grade this submission");

            await _gradeService.AssignGrade(
                submission.StudentId,
                submission.AssignmentId,
                score,
                feedback
            );

            await _notificationService.NotifyStudent(
                submission.StudentId,
                $"Your assignment has been graded: {score}"
            );
        }
    }
}