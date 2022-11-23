using System;
using System.Threading;

namespace Task2;
public class Program2 : Task1.FiniteStateTable
{
    //define actions for task3 FST machine
    public static void actionJRun() 
    { 
        Console.WriteLine("actions J \n");
        LogActions("Action J");
    }
    public static void actionKRun() 
    {   
        Console.WriteLine("actions K \n");
        LogActions("Action K");
    }
    public static void actionLRun() 
    {
        Console.WriteLine("actions L \n");
        LogActions("Action L");
    }

    //Define the
    public static void actionJ()
    {
        var actionsJ_Thread = new Thread(actionJRun);
        actionsJ_Thread.Start();
    }

    public static void actionK()
    {
        var actionsK_Thread = new Thread(actionKRun);
        actionsK_Thread.Start();
    }

    public static void actionL()
    {
        var actionsL_Thread = new Thread(actionLRun);
        actionsL_Thread.Start();
    }

}