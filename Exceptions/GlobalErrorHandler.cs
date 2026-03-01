namespace SchoolSystem.Backend.Exceptions;

public class NotFoundException(string message) : Exception(message);
public class InvalidCredentialsException(string message) : Exception(message);
public class InvalidInvitationException(string message) : Exception(message);
public class TenantMismatchException(string message) : Exception(message);