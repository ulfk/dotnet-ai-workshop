namespace QuizApp;

public class QuestionResult(string question, string answer, bool answerIsCorrect, string outcome)
{
    public string Question { get; set; } = question;

    public string Answer { get; set; } = answer;

    public bool AnswerIsCorrect { get; set; } = answerIsCorrect;

    public string Outcome { get; set; } = outcome;
}
