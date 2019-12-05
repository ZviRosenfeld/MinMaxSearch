# MinMaxSearch
A MinMax Search Engine.
MinMax search is a popular search technique used for finding the next-best move in zero-summed games such as tic-tac-toe, checkers or backgammon.

## Download
You can find MinMaxSearch library on nuget.org via package name MinMaxSearch.

## How to Use

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
var CancellationTokenSource = new CancellationTokenSource();
var engine =  new SearchEngine()
{
    FavorShortPaths = true,
    DieEarly = true,
    MaxScore = 99,
    MinScore = -99
};
var searchResult = engine.Search(startState, searchDepth, CancellationTokenSource.Token);
```

SearchEngine has a number of Search methods that expect different parameters. Most of the parameters are straight-forward. I'd like to elaborate on the IState one.

### IState

There are 2 types of states: IDeterministicState and IProbabilisticState. All your game states will need to implement one of these states.

**IDeterministicState**

States used for deterministic games (games that have no element of luck in them), such as tic-tac-toe or checkers.
```csharp
public interface IDeterministicState : IState
{
    /// <summary>
    /// returns a list of the state's neighbors. Note that a win state shouldn't return any neighbors.
    /// </summary>
    IEnumerable<IState> GetNeighbors();
	
    /// <summary>
    /// returns the state's evaluation (how good it is).
    /// Evaluate must return a value smaller then double.MaxValue and greater then double.MinValue
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

The code contains examples of [Tic-tac-toe](TicTacToeTests/TicTacToeState.cs) and [connect4](Connect4Tests/Connect4State.cs) states.

You can find a tutorial on how to create a tic-tac-toe state [here](https://github.com/ZviRosenfeld/MinMaxSearch/wiki/Tic-Tac-Toe-Tutorial).

**IProbabilisticState**

States used for indeterministic games (games that have an element of luck in them), such as backgammon.

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
    /// Evaluate must return a value smaller then double.MaxValue and greater then double.MinValue
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

The code contains an examples of a [Probabilistic Connect-4 State](ProbabilisticConnect4Tests/ProbabilisticConnect4State.cs) (a probabilistic version of Connect-4).

You can find a tutorial on how to create a probabilistic version of tic-tac-toe [here](https://github.com/ZviRosenfeld/MinMaxSearch/wiki/Probabilistic-Tic-Tac-Toe-Tutorial).

### Tutorials

You can find a tutorial on how to create a tic-tac-toe state [here](https://github.com/ZviRosenfeld/MinMaxSearch/wiki/Tic-Tac-Toe-Tutorial), and one on a probabilistic version of tic-tac-toe [here](https://github.com/ZviRosenfeld/MinMaxSearch/wiki/Probabilistic-Tic-Tac-Toe-Tutorial).

### SearchEngine options:
SearchEngine can be configured with the following options:

**PreventLoops:**
In some games - such as tic-tac-toe or connect 4 - loops are impossible. In others - like chess - loops can be quite common. If this flag is set to true, the program will automatically recognize loop situations and not look any deeper when they occur.
Note that this will only work if Equals is implement in a meaningful way on your states.

**FavorShortPaths:**
If true, the algorithm will favor short solutions over long solutions when they both result in the same score.
If this option is off, you may experience seemingly weird behavior. Say the algorithm sees that Min can set a trap that will end in Max's defeat in six moves. Without favoring short paths, the algorithm might decide to "give up", causing Max to perform random moves, and possibly lose much sooner - even though its opponent may not have noticed the trap.

**ParallelismMode**
There are 3 ParallelismMode: FirstLevelOnly, which is the recommended mode and normally yields the fastest searches. 
In addition, there are the NonParallelism and TotalParallelism modes. 
In the TotalParallelism mode you can set the degree of parallelism using the MaxDegreeOfParallelism field (this field will be ignored otherwise).

**DieEarly:**
If this option is set to true, the algorithm will rerun as soon as it finds a score bigger then or equal to SearchEngine.MaxScore for Max or smaller or equal to SearchEngine.MinScore for Min.
The rationale behind this is that once the algorithm finds a win there's no point in more searching. (We assume that a score greater then MaxScore is a win for Max, and one smaller then MinScore is a win for Min).
Note that this will only work if Equals is implement in a meaningful way on your states.

**IsUnstableState:**
Some states are more interesting than others. With this delegate you can tell the algorithm to continue searching for "interesting" states even after max search depth is exceeded.
IsUnstableState is a delegate of type Func<IState, int, List<IState>, bool>. It receives a state and a list of the states leading up to it, and decides if it's safe to terminate the search at this state.

**Pruners:**
You can use the method SearchEngine.AddPruner(IPruner pruner) to add pruners to the search algorithm.
Pruners can be implemented by implementing the IPruner interface. Then, the ShouldPrune(IState state, int depth, List<IState> passedThroughStates) method will be called on every state the algorithm checks. This can provide you with a lot of customization power over the algorithm.

## IterativeSearch

The [IterativeSearchWrapper](MinMaxSearch/IterativeSearchWrapper.cs) wraps a search engine and can be used to perform [iterative searches](https://en.wikipedia.org/wiki/Iterative_deepening_depth-first_search).
In Iterative search, a depth-limited version of depth-first search is run repeatedly with increasing depth limits till some condition is met.

example:
```csharp
var startState = new TicTacToeState();
var startDepth = 2;
var endDepth = 5;
var CancellationTokenSource = new CancellationTokenSource();
var engine = new SearchEngine();
var iterativeEngine = new IterativeSearchWrapper(engine);
// This will run an IterativeSearch beginning at depth 2, and ending with depth 5 (including)
var searchResult = engine.IterativeSearch(startState, startDepth, endDepth, CancellationTokenSource.Token);
```

## CompetitionManager

Want to test the effect of different evaluation-strategies or search options? CompetitionManager is you friend.

CompetitionManager can play a complete game in which the players can be using a different engines/search-depths/evaluation-strategies.
CompetitionManager will return statistics on the game, including which player won, and how long each player took doing his searching.

```CSharp
namespace MinMaxSearch.Benchmarking
{
    public static class CompetitionManager
    {
        /// <summary>
        /// With this method you can simulate a complete game and compare different evaluation-strategies.
        /// </summary>
        /// <param name="engine"> The engine to use</param>
        /// <param name="startState"> The starting sate</param>
        /// <param name="searchDepth"> How deep should we search</param>
        /// <param name="maxPlayDepth"> After how many moves should we terminate the game if no one won</param>
        /// <param name="maxAlternateEvaluation"> Will be used to evaluate the board on max's turn in stead of the state's regaler Evaluate method (if null, will use the default state's evaluation method)</param>
        /// <param name="minAlternateEvaluation"> Will be used to evaluate the board on min's turn in stead of the state's regaler Evaluate method (if null, will use the default state's evaluation method)</param>
        public static CompetitionResult Compete(this SearchEngine engine, IDeterministicState startState,
            int searchDepth, Func<IState, int, List<IState>, double> maxAlternateEvaluation = null,
            Func<IState, int, List<IState>, double> minAlternateEvaluation = null, int maxPlayDepth = int.MaxValue,
            CancellationToken? cancellationToken = null)
        {
            ...
        }

        /// <summary>
        /// With this method you can simulate a complete game and compare different search-depth or evaluation-strategies.
        /// </summary>
        /// <param name="engine"> The engine to use</param>
        /// <param name="startState"> The starting sate</param>
        /// <param name="playerMaxSearchDepth"> How deep should max search</param>
        /// <param name="playerMinSearchDepth"> How deep should min search</param>
        /// <param name="maxPlayDepth"> After how many moves should we terminate the game if no one won</param>
        /// <param name="maxAlternateEvaluation"> Will be used to evaluate the board on max's turn in stead of the state's regaler Evaluate method (if null, will use the default state's evaluation method)</param>
        /// <param name="minAlternateEvaluation"> Will be used to evaluate the board on min's turn in stead of the state's regaler Evaluate method (if null, will use the default state's evaluation method)</param>
        public static CompetitionResult Compete(this SearchEngine engine, IDeterministicState startState,
            int playerMaxSearchDepth, int playerMinSearchDepth, Func<IState, int, List<IState>, double> maxAlternateEvaluation = null,
            Func<IState, int, List<IState>, double> minAlternateEvaluation = null, int maxPlayDepth = int.MaxValue,
            CancellationToken? cancellationToken = null)
        {
            ...
        }

        /// <summary>
        /// With this method you can simulate a complete game and compare different engines, search-depths or evaluation-strategies.
        /// </summary>
        /// <param name="maxEngine"> An engine to use for max</param>
        /// <param name="minEngine"> An engine to use for min</param>
        /// <param name="startState"> The starting sate</param>
        /// <param name="playerMaxSearchDepth"> How deep should max search</param>
        /// <param name="playerMinSearchDepth"> How deep should min search</param>
        /// <param name="maxPlayDepth"> After how many moves should we terminate the game if no one won</param>
        /// <param name="maxAlternateEvaluation"> Will be used to evaluate the board on max's turn in stead of the state's regaler Evaluate method (if null, will use the default state's evaluation method)</param>
        /// <param name="minAlternateEvaluation"> Will be used to evaluate the board on min's turn in stead of the state's regaler Evaluate method (if null, will use the default state's evaluation method)</param>
        public static CompetitionResult Compete(SearchEngine maxEngine, SearchEngine minEngine,
            IDeterministicState startState, int playerMaxSearchDepth, int playerMinSearchDepth, 
            int maxPlayDepth = int.MaxValue, Func<IState, int, List<IState>, double> maxAlternateEvaluation = null,
            Func<IState, int, List<IState>, double> minAlternateEvaluation = null, CancellationToken? cancellationToken = null)
        {
            ...
        }
    }
}
```
