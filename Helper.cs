using IRemote;
using QuestionLib.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ExamHelper
{
  public class Helper
  {
    public Regex rgx = new Regex("[^a-z0-9]");
    public List<Bank> banks = new List<Bank>();
    public EOSData data;
    public bool flag;

    public Helper(EOSData data)
    {
      try
      {
        this.data = data;
        this.banks = this.readBankFile(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\exam\\keys\\key.txt");
        this.flag = false;
        new Thread(new ThreadStart(this.runPassExam)).Start();
      }
      catch (Exception ex)
      {
      }
    }

    public void runPassExam()
    {
      Question question = (Question) null;
      try
      {
        question = (Question) this.data.ExamPaper.GrammarQuestions[0];
      }
      catch (Exception ex)
      {
      }
      if (question != null)
      {
        while (!this.flag)
        {
          foreach (QuestionAnswer questionAnswer in question.QuestionAnswers)
          {
            if (questionAnswer.Chosen || questionAnswer.Selected)
            {
              this.flag = true;
              this.passExam();
              break;
            }
          }
          Thread.Sleep(1000);
        }
      }
      else
        this.passExam();
    }

    public void passExam()
    {
      FileStream fileStream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\exam\\questions\\questions_" + DateTime.Now.ToString("MM_dd_yyyy_hhmmss") + ".txt", FileMode.Create);
      StreamWriter streamWriter = new StreamWriter((Stream) fileStream, Encoding.UTF8);
      string[] strArray1 = new string[13]
      {
        "A",
        "B",
        "C",
        "D",
        "E",
        "F",
        "G",
        "H",
        "I",
        "K",
        "N",
        "M",
        "O"
      };
      int num1 = 1;
      foreach (Question grammarQuestion in this.data.ExamPaper.GrammarQuestions)
      {
        streamWriter.WriteLine("QN=" + (object) num1++ + "\t" + grammarQuestion.Text + "\n");
        string[] strArray2 = this.searchKey(this.format(grammarQuestion.Text));
        int num2 = 0;
        foreach (QuestionAnswer questionAnswer in grammarQuestion.QuestionAnswers)
        {
          try
          {
            streamWriter.WriteLine(strArray1[num2++] + ". " + questionAnswer.Text + "\n");
            foreach (string key in strArray2)
            {
              if (this.formatSearchKey(questionAnswer.Text).Contains(this.formatSearchKey(key)))
              {
                questionAnswer.Chosen = true;
                questionAnswer.Done = true;
                questionAnswer.Selected = true;
              }
            }
          }
          catch (Exception ex)
          {
          }
        }
        streamWriter.WriteLine("\n\n");
      }
      streamWriter.Flush();
      fileStream.Close();
    }

    public string[] searchKey(string question)
    {
      List<string> stringList = new List<string>();
      string str = this.formatSearchKey(question);
      foreach (Bank bank in this.banks)
      {
        if (this.formatSearchKey(bank.question).Contains(str))
          stringList.Add(bank.answer);
      }
      return stringList.ToArray();
    }

    private string format(string question) => question.ToLower().Replace("(choose 1 answer)", "").Replace("(choose one)", "").Replace("(choose one answer only)", "").Replace("(choose the most correct)", "").Replace("select one", "").Trim();

    public string formatSearchKey(string key)
    {
      key = key.ToLower();
      key = this.rgx.Replace(key, "");
      return key;
    }

    public List<Bank> readBankFile(string path)
    {
      string[] strArray = File.ReadAllLines(path);
      List<Bank> bankList = new List<Bank>();
      string str1 = (string) null;
      foreach (string str2 in strArray)
      {
        if (!str2.Contains("|"))
        {
          str1 += str2;
        }
        else
        {
          string question = str1 + str2.Substring(0, str2.IndexOf("|", StringComparison.Ordinal));
          string str3 = str2.Substring(str2.IndexOf("|") + 1);
          str1 = (string) null;
          string answer = str3;
          Bank bank = new Bank(question, answer);
          bankList.Add(bank);
        }
      }
      return bankList;
    }
  }
}
