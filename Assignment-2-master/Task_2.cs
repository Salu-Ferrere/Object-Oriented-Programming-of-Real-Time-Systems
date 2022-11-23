using System;

namespace Task2;


public class Program : Task1.FiniteStateTable
{
   //Here the actions for the task2 FST are defined
    public static void actionWRun() 
    { 
        Console.Write("Action W \n");
        LogActions("Action W");
    }
    public static void actionXRun() 
    {   
        Console.Write("Action X \n");
        LogActions("Action X");
    }
    public static void actionYRun() 
    {   
        Console.Write("Action Y \n");
        LogActions("Action Y");
    }
    public static void actionZRun() 
    {   
        Console.Write("Action Z \n");
        LogActions("Action Z");
    }

    //These are the action methods that are passed to the FST on creation of the machine1 instance
    public static void actionW()
    {
        var actionsW_Thread = new Thread(actionWRun);
        actionsW_Thread.Start();
    }
    public static void actionX()
    {
        var actionsX_Thread = new Thread(actionXRun);
        actionsX_Thread.Start();
    }
    public static void actionY()
    {
        var actionsY_Thread = new Thread(actionYRun);
        actionsY_Thread.Start();
    }
    public static void actionZ()
    {
        var actionsZ_Thread = new Thread(actionZRun);
        actionsZ_Thread.Start();
    }

    static void Main(string[] args)
    {

        //Set up finite-state machine1
        Program machine1 = new Program();

        //Sets how many states are in the FST and their names.
        machine1.newState("S0"); //First state added is the entry point
        machine1.newState("S1");
        machine1.newState("S2");

        //Sets what the expected events are in the FST.
        machine1.newEvent("a");
        machine1.newEvent("b");
        machine1.newEvent("c");

        //Initialises the FST array to be the correct size for the number of new events and states added, and initalises 
        //cell_FST objects in each element
        machine1.initaliseFinateStateTable();


        //Populate the cell_FST elements in the array with the desired actions and state changes

        //Set S0 node
        //Transition to S1
        machine1.setNextState("a", "S0", "S1");
        machine1.setActions("a", "S0", actionX);
        machine1.setActions("a", "S0", actionY);


        

        //Set S1 node
        //Transition to S0
        machine1.setNextState("a", "S1", "S0");
        machine1.setActions("a", "S1", actionW);

        //Transition to S2
        machine1.setNextState("b", "S1", "S2");
        machine1.setActions("b", "S1", actionX);
        machine1.setActions("b", "S1", actionZ);


        //Set S2 node
        //Transition to S1
        machine1.setNextState("c", "S2", "S1");
        machine1.setActions("c", "S2", actionX);
        machine1.setActions("c", "S2", actionY);

        //Transiton to S0
        machine1.setNextState("a", "S2", "S0");
        machine1.setActions("a", "S2", actionW);


        //setup finite state machine2
        
        Program machine2 = new Program();

        machine2.newState("SB");
        machine2.newState("SA");

        machine2.newEvent("in S1");
        machine2.newEvent("a");
        

        machine2.initaliseFinateStateTable();

        //Set SA node
        machine2.setNextState("a", "SA", "SB");

        //Set SB node
        machine2.setNextState("in S1", "SB", "SA");
        machine2.setActions("in S1", "SB", Program2.actionJ);
        machine2.setActions("in S1", "SB", Program2.actionK);
        machine2.setActions("in S1", "SB", Program2.actionL);



        //User interface
        string inputEvent;
        int inputEventIndex = 0;
        bool inputValid = false;
        string logFileAddress;


        while (true)
        {

            do
            {
                System.Console.WriteLine("Please press a, b, or c:");
                inputEvent = Console.ReadKey().KeyChar.ToString();

                System.Console.WriteLine("");
                System.Console.WriteLine("");
                
                if (inputEvent == "q") //Exit program if q pressed
                {
                    System.Console.WriteLine("Please provide fully qualified logfile address: ");
                    logFileAddress = Console.ReadLine();
                    machine1.printLogFile(logFileAddress); //creates the log file at program termination
                    Environment.Exit(0);
                }
                else
                {
                    inputEventIndex = machine1.getIndex(inputEvent);
                    if (inputEventIndex == -1)
                    {
                        inputValid = false;
                        Console.WriteLine("Invalid input");
                    } else
                    {
                        inputValid = true;
                    }

                }

            } while (!inputValid);


            //Performs all actions associated with the state change.
            machine1.getActions(machine1.getIndex(inputEvent), machine1.currentState).Invoke();

            //Displays the new state entered.
            machine1.currentState = machine1.getNextState(machine1.getIndex(inputEvent), machine1.currentState);
            System.Console.WriteLine("Current state of machine1 is " + machine1.states[machine1.currentState].stateName);
            System.Console.WriteLine("");


            //Performs all actions associated with the state change.
            restartEventCheck:
            machine2.getActions(machine2.getIndex(inputEvent), machine2.currentState).Invoke();
            Thread.Sleep(100); //This prevents the machine from progressing to the next state before the action threads have completed.
         
            //Displays the new state entered.
            machine2.currentState = machine2.getNextState(machine2.getIndex(inputEvent), machine2.currentState);
            System.Console.WriteLine("Current state of machine2 is " + machine2.states[machine2.currentState].stateName);
            System.Console.WriteLine("");
            logEvent(inputEvent);

            //Checks to see if machine1 is in S1, and if machine2 is in SB. If it is, a new event is triggered "in S1" and machine2 needs to run again with the new event
            if (machine1.currentState == machine1.getIndex("S1") & machine2.currentState == machine2.getIndex("SB"))
            {
                inputEvent = "in S1";
              
                goto restartEventCheck;
            }
           
        }
        
    }
}