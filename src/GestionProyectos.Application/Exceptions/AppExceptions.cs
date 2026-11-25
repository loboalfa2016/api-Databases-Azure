namespace GestionProyectos.Application.Exceptions;

public class NotFoundException(string message) : Exception(message);
public class BusinessRuleException(string message) : Exception(message);
public class BadRequestException(string message) : Exception(message);