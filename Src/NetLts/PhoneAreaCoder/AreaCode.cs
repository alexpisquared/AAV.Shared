namespace PhoneAreaCoder;

public readonly record struct AreaCode(string Code, string Area, int Prio, string Oset, string Note)
{
  public string Tooltip => $"{Area,-5}\t{Oset,3} h \t{Note}";
  public bool IsLocal => Prio <= 1;
  public bool IsGta => Prio <= 3;
  public bool IsOntario => Prio <= 5;
  public bool IsCanada => Prio <= 8;
}
