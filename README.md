# MinMaxSearch
A MinMax Search Engine.
MinMax search is a popular search technique used for finding the next-best move in zero-summed games such as tic-tac-toe, checkers or backgammon.

## Download
You can find MinMaxSearch library on nuget.org via package name MinMaxSearch.

## How to Use
To use this algorithm, you'll need to create a new instance of SearchEngine. 
SearchEngine has a number of Search methods that expect different parameters. Most of the parameters are straight-forward. I'd like to elaborate on the IState one.

### IState

There are 2 types of states: IDeterministicState and IProbabilisticState. All you game states will need to implement one of these states.

**IDeterministicState**

States used for deterministic games (games that have no element of luck in them), like tic-tac-toe or checkers.
```csharp
public interface IDeterministicState : IState
{
    /// <summary>
    /// returns a list of the state's neighbors. Note that a win state shouldn't return any neighbors.
    /// </summary>
    IEnumerable<IState> GetNeighbors();
	
    /// <summary>
    /// returns the state's evaluation (how good it is).
    /// </summary>
    double Evaluate(int depth, List<IState> passedThroughStates);
	
    /// <summary>
    /// values can be Player.Max and Player.Min (Player is an enum in the code). 
    /// Max is the player trying to get the best score, while Min is the player trying to get the worst score.
    /// While most in most games, turns will alternate between Max and Min, you can really implement any order you want.
    /// </summary>
    Player Turn { get; }
}
```

*In addition, I recommend that your states also implement object's Equals and GetHashCode methods, as many of the algorithm optimizations rely on these methods being implemented in a meaningful way.*

The code contains examples for [Tic-tac-toe](TicTacToeTests/TicTacToeState.cs) and [connect4](Connect4Tests/Connect4State.cs) states.

**IProbabilisticState**

States used for indeterministic games (games that have an element of luck in them), like backgammon.

Note that even for indeterministic games, the first state will need to be of type IDeterministicState. 
This reflects the fact that algorithm answers the question "what move should I do next?" and this question has no meaning when the next state's outcome depends on a probability.
If your implementing a game like backgammon, the first state should "know" what the user rolled, so it should be an IDeterministicState.
```csharp
public interface IDeterministicState : IState
{
    /// <summary>
    /// returns a tuple containing a probability, and a list of the s neighbors for that probability
    /// Note that a win state shouldn't return any neighbors.
    /// </summary>
    IEnumerable<Tuple<double, List<IState>>> GetNeighbors();
	
    /// <summary>
    /// returns the state's evaluation (how good it is).
    /// </summary>
    double Evaluate(int depth, List<IState> passedThroughStates);
	
    /// <summary>
    /// values can be Player.Max and Player.Min (Player is an enum in the code). 
    /// Max is the player trying to get the best score, while Min is the player trying to get the worst score.
    /// While most in most games, turns will alternate between Max and Min, you can really implement any order you want.
    /// </summary>
    Player Turn { get; }
}
```

*In addition, I recommend that your states also implement object's Equals and GetHashCode methods, as many of the algorithm optimizations rely on these methods being implemented in a meaningful way.*

### Examples
Following are a few snippets take from the project's unit tests. You can refer to the [Connnect4 Tests](Connect4Tests/Connect4Tests.cs) or the [Tic-tac-toe Tests](TicTacToeTests/TicTacToeBassicTests.cs) for more examples.

example1:
```csharp
var startState = new TicTacToeState();
var searchDepth = 5;
var engine = new SearchEngine();
var searchResult = engine.Search(startState, searchDepth);
```

example2:
```csharp
var startState = new Connect4State();
var searchDepth = 5;
var cancellationToken = new CancellationTokenSource();
var engine =  new SearchEngine()
{
    MaxDegreeOfParallelism = 2,
    FavorShortPaths = true
};
var searchResult = engine.Search(startState, searchDepth, cancellationToken);
```

### SearchEngine options:
SearchEngine can be configured with the following options:

**PreventLoops:**
In some games - such as tic-tac-toe or connect 4 - loops are impossible. In others - like chess - loops can be quite common. If this flag is set to true, the program will automatically recognize loop situations and not look any deeper when they occur.
Note that this will only work if Equals is implement in a meaningful way on your states.

**FavorShortPaths:**
If true, the algorithm will favor short solutions over long solutions when they both result in the same score.
If this option is off, you may experience seemingly weird behavior. Say the algorithm sees that Min can set a trap that will end in Max's defeat in six moves. Without favoring short paths, the algorithm might decide to "give up", causing Max to perform random moves, and possibly lose much sooner - even if its opponent may not have noticed the trap.

**MaxDegreeOfParallelism**
Note that a higher degree of parallelism doesn't necessarily equal a faster search. You should probably do some benchmarking to find the degree of parallelism best suited for your problem.

**DieEarly:**
If this option is set to true, the algorithm will rerun as soon as it finds a score bigger then SearchEngine.MaxScore for Max or SearchEngine.MinScore for Min.
The rationale behind this is that once the algorithm finds a win there's no point in more searching. (We assume that a score greater then MaxScore is a win for Max, and one smaller then MinScore is a win for Min).
Note that this will only work if Equals is implement in a meaningful way on your states.

**IsUnstableState:**
Some states are more interesting than others. With this delegate you can tell the algorithm to continue searching for "interesting" states even after it's reached the max search depth.
IsUnstableState is a delegate of the form Func<IState, int, List<IState>, bool>. It receives a state and the states leading up to it, and decides if it's safe to terminate the search at this state.

**Pruners:**
You can use the method SearchEngine.AddPruner(IPruner pruner) to add pruners to the search algorithm.
Pruners can be implemented by implementing the IPruner interface. Then, the ShouldPrune(IState state, int depth, List<IState> passedThroughStates) method will be called on every state the algorithm checks. This can provide you with a lot of customization power over the algorithm.

## Benchmarking
An optimization that will improve one search can hurt another. That's why benchmarking is so impotent. It lets you customize the search to best suite your needs.

**BanckmarkResult[] Benchmark(this SearchEngine searchEngine, IState startState, int searchDepth, int times)**: This is an extension method that will provide you with information regarding the search's performance.
