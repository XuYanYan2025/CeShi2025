using System;

namespace MathQuiz
{
    class Program
    {
        static void Main(string[] args)
        {
            int score = 0;
            int totalQuestions = 10;

            Console.WriteLine("欢迎来到10以内加减法练习！");


        kaishi:
            Console.WriteLine("我们先来确认题目数量：");

            if (int.TryParse(Console.ReadLine(), out int userAnswer))
            {
                totalQuestions = userAnswer;
            }
            else
            {
                Console.WriteLine("请输入一个有效的数字用于确认题目数量\n");
                goto kaishi;
            }

            Console.WriteLine($"你将回答{totalQuestions}道题目。\n现在就开始吧。。。。。。\n");

            Random random = new Random();

            for (int i = 0; i < totalQuestions; i++)
            {
                // 生成两个10以内的随机数
                int num1 = random.Next(0, 11);
                int num2 = random.Next(0, 11);

                // 随机选择加法或减法
                char operatorChar = random.Next(0, 2) == 0 ? '+' : '-';

                // 计算正确答案
                int correctAnswer = operatorChar == '+' ? num1 + num2 : num1 - num2;

                // 显示题目
                Console.WriteLine($"第{i + 1}题: {num1} {operatorChar} {num2} = ?");

            chongfu:
                // 获取用户输入
                if (int.TryParse(Console.ReadLine(), out userAnswer))
                {
                    if (userAnswer == correctAnswer)
                    {
                        Console.WriteLine("回答正确！\n");
                        score++;
                    }
                    else
                    {
                        Console.WriteLine($"回答错误。正确答案是: {correctAnswer}\n");
                    }
                }
                else
                {
                    Console.WriteLine("请输入一个有效的数字！\n");
                    goto chongfu;
                }
            }

            // 显示最终得分
            Console.WriteLine($"练习结束！你的得分是: " + score * 100 / totalQuestions);
            Console.ReadLine();
        }
    }
}