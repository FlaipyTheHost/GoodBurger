namespace GoodBurger.Exceptions;

public class InvalidOrderException(string message)
    : Exception(message);
