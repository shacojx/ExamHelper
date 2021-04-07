namespace ExamHelper
{
  public class Bank
  {
    public string question { get; set; }

    public string answer { get; set; }

    public Bank(string question, string answer)
    {
      this.question = question;
      this.answer = answer;
    }
  }
}
