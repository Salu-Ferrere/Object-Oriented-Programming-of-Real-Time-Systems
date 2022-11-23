using System;
using System.IO;



namespace Task1;

public class FiniteStateTable
 {
    //Lists that contain all states and events added to the finite state table 
    public List<State> states = new List<State>();
    public List<Event> events = new List<Event>();

    public static List<LogEntry> LogEntries = new List<LogEntry>(); //This list containts all logged events
    public static List<string> ExecutedActions = new List<string>(); //This list contains the actions executed on each event. It is cleared for each new event


    public static int eventCount { get; set; } = 0; //Records the total number of Event objects created
    public static int stateCount { get; set; } = 0; //Records the total number of State objects created

    public static void nullAction() { } //When the FST is initalised, the default action of each cell is nullAction() that does nothing
    public delegate void delAction(); //Used to store actions in cell_FST list of actions

    delAction nulldel = nullAction; //enables nullAction to be stored in cell_FST list of actions.
   

    public bool validInput = false; //Indicates if an input is valid for this finite state machine

   
    
    //The structure that organises the states list relative to the events list
    private cell_FST[,] FST;

    public int currentState = 0; //The current state is set to the first state added to the FST;

    public FiniteStateTable() { } 

    //The struct that contains the action(s) and next state of each state element
    private struct cell_FST
    {
        public int nextState = -1; //The index of the next state of the FST
        public FiniteStateTable.delAction actions = nullAction; //Lists all actions associated with state change
        public cell_FST() { } //struct members are assigned at run-time

    }

    //The State struct is how new states are added to the FST
    public struct State
    {
        
        public int stateNumber; //Identifies this objects location in the State axis of the 2D array FST
        public string stateName; //The name of the state

        public State(string stateName) {
            stateNumber = stateCount; //Records how many State objects have been instantiated
            stateCount++; //increments the State object counter variable
            this.stateName = stateName; //Sets the name of this State (e.g. "S1")
            }

    }

    //The Event struct is how new expected events are added to the FST machine
    public struct Event
    {
        
        public int eventNumber; //Identifies this objects location in the Event axis of the 2D array FST
        public string eventName; //The the expected event input (which are all strings in this assignment)

        public Event(string eventName)
        {
            eventNumber = eventCount; //Records how many Event objects have been instantiated
            eventCount++; //increments the Event object counter variable
            this.eventName = eventName; //Sets the name of this Event (e.g. "a")
        }
    }


    //This struct stores all the relative details that are required to be logged
    public struct LogEntry
    {
        public string timeStamp;
        public string triggerEvent;
        public List<string> subListExecutedActions;
        string actions = "";

        public LogEntry (string triggerEvent, List<string> execicutedActions)
        {
            this.timeStamp = DateTime.Now.ToString(); //Records the time in readable format
            this.triggerEvent = triggerEvent; //Records the triggering event
            this.subListExecutedActions = new List<string>(execicutedActions); //Stores the list of executed actions for the provided event
        }

        //This method returns the entire log as a concatinated string
        public string getLog()
        {
            foreach (string action in subListExecutedActions)
            {
                actions += action + ", ";
            }
            
            string log = "Timestamp: " + timeStamp.ToString() + " Event: " + triggerEvent + " Executed actions: " + actions;
            return log;
        }
    }


    //This method is used before calling initaliseFinateStateTable() to add a new state to the table
    public void newState(string stateName)
    {
        State State = new State(stateName);
        states.Add(State);

    }

    //This method is used before calling initaliseFinateStateTable() to add a new event to the table
    public void newEvent(string eventName)
    {
        Event Event = new Event(eventName);
        events.Add(Event);
    }

    //This method initalises the 2D array FST to the appropriate size for the number of events and states.
    //It then initalises cell_FST objects in each array element
    public void initaliseFinateStateTable()
    {
        FST = new cell_FST[states.Count, events.Count];

        for (int i = 0; i < states.Count; i++)
        {
            for (int j = 0; j < events.Count; j++)
            {
                FST[i, j] = new cell_FST();
            }
        }
        stateCount = 0;
        eventCount = 0;
    }


    //Accesses the desired element in FST and sets the nextState of said element to the index of the next state
    public void setNextState(String Event, String State, String nextStateArg) 
    {
        this.FST[getIndex(State),getIndex(Event)].nextState = getIndex(nextStateArg);
    }

    //used in conjuction with setNextState method to asociate action with new state
    public void setActions(String Event, String State, delAction action) 
    {
        this.FST[getIndex(State), getIndex(Event)].actions += action;
    } 

    public int getNextState(int Event_, int State) //Provided the index position of the state and event in the FST, it will return the index of the next state
    {
        //To allow multilple FST objects to run concurrently, a check is needed to see if the event exists in this object
        validInput = false;
        foreach (Event Event in events) //Checks to see if the passed event is included in the list of events for the FST
        {
            if (Event.eventNumber == Event_) { validInput = true; }
        }

        if (!validInput) { return currentState; } //If a valid input isn't found, return the current state to indicate no change

        if (this.FST[State, Event_].nextState == -1) { return currentState; } //If no state change is recorded at the the given inticies, return the current state to indicate no change

        return this.FST[State, Event_].nextState; //if all checks pass, return the next state stored in the FST element
    }

    public delAction getActions(int Event_, int State) //Overloaded function allowing integer index values to be used instead of the event or state string names
    {
        
        //if an invalid index is given, return turn null action
        validInput = false;
        foreach (Event Event in events)
        {
            if (Event.eventNumber == Event_) { validInput = true; }
        }
        if (!validInput) { return (delAction)nulldel; }
        

        return this.FST[State, Event_].actions;
    }

    //This function finds the index number for either the state or event when given its name. 
    //Note: the assumption is made that event names will be different from state names.
    public int getIndex(string inputName)
    {
        for (int i = 0; i < states.Count; i++)
        {
            if (states[i].stateName == inputName) { return i; }
        }

        for (int i = 0; i < events.Count; i++)
        {
            if (events[i].eventName == inputName) { return i; }
        }
        return -1; //if name isn't found to be in either class, return -1 index, causing runtime exception
    }

    public static void logEvent(string triggerEvent) //This function is used to create new log entries
    {
        LogEntry logEntry = new LogEntry(triggerEvent, ExecutedActions);
        LogEntries.Add(logEntry); //Adds the new log to list of existing logs
        
        ExecutedActions.Clear();
    }

    //This function is called at the termination of the program to create the log file, as well as handle any exceptions due to invalid inputs
    public void printLogFile(string fileAddress)
    {
    Retry:
        try
        {
            if (!Path.IsPathFullyQualified(fileAddress)) //Checks to make sure path is fully qualified
            {
                Console.WriteLine("Invalid address, try again");
                fileAddress = Console.ReadLine();
                goto Retry;
            }

            using (StreamWriter logFile = new StreamWriter(fileAddress)) //Creates file at  fileAddress
            {

                foreach (LogEntry logEntry in LogEntries) //Accesses list of all logs and prints one per line
                {
                    logFile.WriteLine(logEntry.getLog());
                }

                Console.WriteLine("Log successfuly created");
            }
        }
        catch (IOException) //catches exceptions thrown by invalid filenames.
        {
            Console.WriteLine("Invalid address, try again");
            fileAddress = Console.ReadLine();
            goto Retry;
        }

        }
    public static void LogActions(string action) //This functions logs actions that are completed and stores them in a list of strings
    {
        ExecutedActions.Add(action);
    }

}