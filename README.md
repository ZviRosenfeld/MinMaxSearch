# MinMaxSearch

MinMaxSearch is a MinMax Search Engine that was created to be easily customized and simple to use.

MinMax search is a popular search technique used for finding the next-best move in zero-summed games such as tic-tac-toe, checkers or backgammon.

## Download
You can find MinMaxSearch library on nuget.org via package name MinMaxSearch.

## Table of Contents

- [Usage](#usage)
  - [IState](#istate)
  - [IDeterministicState](#ideterministicstate)
  - [IProbabilisticState](#iprobabilisticstate)
	
- [Tutorials](#tutorials)

- [SearchEngine options](#searchengine-options)
  - [PreventLoops](#preventloops)
  - [FavorShortPaths](#favorshortpaths)
  - [ParallelismMode](#parallelismmode)
  - [CacheMode](#cachemode)
  - [DieEarly](#dieearly)
  - [IsUnstableState](#isunstablestate)
  - [Pruners](#pruners)
  - [SkipEvaluationForFirstNodeSingleNeighbor](#skipevaluationforfirstnodesingleneighbor)
  - [StateDefinesDepth](#statedefinesdepth)
  
- [IterativeSearch](#iterativesearch)

- [CompetitionManager](#competitionmanager)

## Usage

example1:
```csharp
IDeterministicState startState = new TicTacToeState();
int searchDepth = 5;
SearchEngine engine = new SearchEngine();
SearchResult searchResult = engine.Search(startState, searchDepth);
```

example2:
```csharp
IDeterministicState startState = new Connect4State();
int searchDepth = 5;
CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
SearchEngine engine =  new SearchEngine()
{
    FavorShortPaths = true,
    DieEarly = true,
    MaxScore = 99,
    MinScore = -99
};
SearchResult searchResult = engine.Search(startState, searchDepth, cancellationTokenSource.Token);
```

SearchEngine has a number of Search methods that expect different parameters. Most of the parameters are straight-forward. I'd like to elaborate on the IState one.

### IState

There are 2 types of states: IDeterministicState and IProbabilisticState. All your game states will need to implement one of these states.

### IDeterministicState

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

### IProbabilisticState

States used for indeterministic games (games that have an element of luck in them), such as backgammon.

Note that even for indeterministic games, the first state will need to be of type IDeterministicState. 
This reflects the fact that algorithm answers the question "what move should I do next?" and this question has no meaning when the next state's outcome depends on a probability.
If you're implementing a game like backgammon, the first state should "know" what the user rolled, so it should be an IDeterministicState.
```csharp
public interface IProbabilisticState : IState
{
    /// <summary>
    /// returns a tuple containing a probability, and a list of neighbors for that probability
    /// Note that a win state shouldn't return any neighbors.
    /// </summary>
    IEnumerable<Tuple<double, IEnumerable<IState>>> GetNeighbors();
	
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

## Tutorials

You can find a tutorial on how to create a tic-tac-toe state [here](https://github.com/ZviRosenfeld/MinMaxSearch/wiki/Tic-Tac-Toe-Tutorial), and one on a probabilistic version of tic-tac-toe [here](https://github.com/ZviRosenfeld/MinMaxSearch/wiki/Probabilistic-Tic-Tac-Toe-Tutorial).

## SearchEngine options
SearchEngine can be configured with the following options:

### PreventLoops
In some games - such as tic-tac-toe or connect 4 - loops are impossible. In others - like chess - loops can be quite common. If this flag is set to true, the program will automatically recognize loop situations and not look any deeper when they occur.
Note that this will only work if Equals is implement in a meaningful way for your states.

### FavorShortPaths
If true, the algorithm will favor short solutions over long solutions when they both result in the same score.
If this option is off, you may experience seemingly weird behavior. Say the algorithm sees that Min can set a trap that will end in Max's defeat in six moves. Without favoring short paths, the algorithm might decide to "give up", causing Max to perform random moves, and possibly lose much sooner - even though its opponent may not have noticed the trap.

Please note that FavorShortPaths may not work togather with caching. 

### ParallelismMode
There are 4 ParallelismMode:
- *FirstLevelOnly*: In this mode only the first level of the search tree will be calculated in parallel. This is the recommended mode and normally yields the fastest searches.
- *ParallelismByLevel*: In this mode, the first x levels of the search will be carried out in parallel. You can determine x by setting "MaxLevelOfParallelism" in the SearchEngine. (available since 1.5.0)
- *NonParallelism*: No parallelism.
- *TotalParallelism*: In this mode, the entire search tree will be calculated in parallel, up to the "MaxDegreeOfParallelism"

Note that "MaxDegreeOfParallelism" will be ignored in all modes other than "TotalParallelism", and "MaxLevelOfParallelism" will be ignored in all modes other than "ParallelismByLevel".

### CacheMode
*Very important:* You can only use that cache if your states' evaluation doesn't change depending on its location in the search tree.
In particular, your states' evaluation can't depend on their depth in the tree of the states they've passed through. 

Caching lets the engine remember stares that lead to certain win, losses or draws, so that it doesn't need to re-search trees it's already searched.
Note that caching will only work if you implement Equals and GetHashValue in a meaningful way for your states. 
Caching is available since version 1.5.0.

We support 3 modes of caching:
- *NoCache*: No Caching.
- *NewCache*: The engine will initialize and use a new cache for every search.
- *ReuseCache*: The engine will re-use the same cache between searches. You can clean the cache by calling the CacheManager's Clean method.

If you're using the ReuseCache option, you can use the FillCache extension method to fill the cache while the program is idle (say, while your opponent is considering their next move).
Just to remember to cancel the FillCache when you're ready to run a search (using the cancellation token).

Please note that when using caching the StateSequence in the SearchResult may be cut off early. 
This is because the cache remembers the evaluations that states will lead to, but not *how* the state lead to that evaluation.
So the StateSequence will end at the cached state.

### DieEarly
If this option is set to true, the algorithm will rerun as soon as it finds a score greater than or equal to SearchEngine.MaxScore for Max or smaller or equal to SearchEngine.MinScore for Min.
The rational behind this is that once the algorithm finds a win there's no point in more searching. (We assume that a score greater then MaxScore is a win for Max, and one smaller then MinScore is a win for Min).
Note that this will only work if Equals is implement in a meaningful way on your states.

### IsUnstableState
Some states are more interesting than others. With this delegate you can tell the algorithm to continue searching for "interesting" states even after max search depth is exceeded.
IsUnstableState is a delegate of type Func<IState, int, List<IState>, bool>. It receives a state and a list of the states leading up to it, and decides if it's safe to terminate the search at this state.

```CSharp
/// <summary>
/// An example of using an IsUnstableState delegate.
/// This delegate works on a Checkers state. It returns true if there is an available jump.
/// </summary>
class IsUnstableStateSample
{
    public ISearchEngine GetEngine()
    {
        var engine = new SearchEngine
        {
            IsUnstableState = IsUnstableCheckersState
        };
        return engine;
    }

    private bool IsUnstableCheckersState(IState state, int depth, List<IState> passedThroghStates)
    {
        var checkersState = (CheckersState) state;
        return checkersState.CanJump();
    }
}
```

### Pruners
You can use the method SearchEngine.AddPruner(IPruner pruner) to add pruners to the search algorithm.
Pruners can be implemented by implementing the [IPruner](https://github.com/ZviRosenfeld/MinMaxSearch/blob/master/MinMaxSearch/Pruners/IPruner.cs) interface. 
Then, the ShouldPrune(IState state, int depth, List<IState> passedThroughStates) method will be called on every state the algorithm checks. 
This can provide you with a lot of customization power over the algorithm.

```CSharp
class SamplePruner : IPruner 
{
    public bool ShouldPrune(IState state, int depth, List<IState> passedThroughStates)
    {
        // Some logic here to decide if we should prune - you probably don't want to always return false for a real pruner.
        return false;
    }
}

class UsePruner
{
    public ISearchEngine GetEngineWithPruner()
    {
        SearchEngine engine = new SearchEngine();
        engine.AddPruner(new SamplePruner());
        return engine;
    }
}
```

### SkipEvaluationForFirstNodeSingleNeighbor
If this is set to true, in the case that the *first* node has a single neighbor, the engine will return that neighbor rather than evaluation the search tree.
The default of this setting is true. (Available since 1.5.0).

Note that this only applies to the first node.

### StateDefinesDepth
Set this to true it is possible to infer a state's depth from the state alone.
This is true for games like tic-tac-toe and connect4, where the depth of a state is the number of tokens on the board.

The engine will use this knowledge to optimize the search.

## IterativeSearch

The [IterativeSearchWrapper](MinMaxSearch/IterativeSearchWrapper.cs) wraps a search engine and can be used to perform [iterative searches](https://en.wikipedia.org/wiki/Iterative_deepening_depth-first_search).
In Iterative search, a depth-limited version of depth-first search is run repeatedly with increasing depth limits till some condition is met.

example:
```csharp
IDeterministicState startState = new TicTacToeState();
int startDepth = 2;
int endDepth = 5;
CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
ISearchEngine engine = new SearchEngine();
IterativeSearchWrapper iterativeEngine = new IterativeSearchWrapper(engine);
// This will run an IterativeSearch beginning at depth 2, and ending with depth 5 (including)
SearchResult searchResult = iterativeEngine.IterativeSearch(startState, startDepth, endDepth, CancellationTokenSource.Token);
```

## CompetitionManager

Want to test the effect of different evaluation-strategies or search options? CompetitionManager is you friend.

CompetitionManager can play a complete game in which the players can be using a different engines/search-depths/evaluation-strategies.
CompetitionManager will return statistics on the game, including which player won, and how long each player took doing his searching.

Exsample1: Comparing different search depths
```CSharp
IDeterministicState startState = new TicTacToeState();
int minSearchDepth = 2;
int maxSearchDepth = 5;
SearchEngine engine = new SearchEngine();

CompetitionResult competitionResult = engine.Compete(startState, maxSearchDepth, minSearchDepth);

// Print some of the results
Console.WriteLine("Max Search Time " + competitionResult.MaxTotalTime);
Console.WriteLine("Min Search Time " + competitionResult.MinTotalTime);
Console.WriteLine("Final Score " + competitionResult.FinalState.Evaluate(0, new List<IState>()));
```

Exsample2: In this example we'll use a different evaluation method for min
```CSharp
IDeterministicState startState = new TicTacToeState();
int searchDepth = 6;
SearchEngine engine = new SearchEngine();

CompetitionResult competitionResult = engine.Compete(startState, searchDepth, minAlternateEvaluation: (s, d, l) => {
                // Some alternate evaluation goes here - you probably don't really want to return 0
                return 0;
            });
			
// Print some of the results
Console.WriteLine("Max Search Time " + competitionResult.MaxTotalTime);
Console.WriteLine("Min Search Time " + competitionResult.MinTotalTime);
Console.WriteLine("Final Score " + competitionResult.FinalState.Evaluate(0, new List<IState>()));
```

Exsample3: Comparing different engines
```CSharp
IDeterministicState startState = new TicTacToeState();
int searchDepth = 5;
int playDepth = 100;
SearchEngine engine1 = new SearchEngine();
SearchEngine engine2 = new SearchEngine();

CompetitionResult competitionResult = CompetitionManager.Compete(engine1, engine2, startState, searchDepth, searchDepth, playDepth);

// Print some of the results
Console.WriteLine("Max Search Time " + competitionResult.MaxTotalTime);
Console.WriteLine("Min Search Time " + competitionResult.MinTotalTime);
Console.WriteLine("Final Score " + competitionResult.FinalState.Evaluate(0, new List<IState>()));
```
