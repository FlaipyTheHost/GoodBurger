namespace GoodBurger.Exceptions;

public class DuplicateItemException(string item)
    : Exception($"Item '{item}' was already added to this order.");
