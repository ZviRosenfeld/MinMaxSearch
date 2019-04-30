# MinMaxSearch
A MinMax Search Engine.
MinMax search is a popular search technique used for finding the next-best move in zero-summed games such as tic-tac-toe or checkers.

## How to use
To use this algorithm, you'll need to create a new instance of SearchEngine. Then you can call searchEngine.Evaluate(IState startState, Player player, int maxDepth) which will return an object of type SearchResult. SearchResult will contain - among other stuff - the best next move.

searchEngine.Evaluate expects 2 arguments:

**IState:** 
this is an interface that your game-specific states will need to implement. The interface requires that you implement the following 2 methods:
1) IEnumerable<IState> GetNeighbors(); - returns a list of the state's neighbors. *Note that a win state shouldn't return any neighbors*.
2) double Evaluate(int depth, List<IState> passedThroughStates); - returns the state's evaluation (how good it is).

In addition, we recommend that your states also implement object's Equals and GetHashCode methods, as many of the algorithm optimizations rely on these methods being implemented in a meaningful way.

**Player:**
The algorithm assumes the existence of 2 players: Player.Max and Player.Min (Player is an enum in the code).
Max is the player trying to get the best score, while Min is the player trying to get the worst score. You can choose which player you want to search for.

### Examples
The project contains unit tests with states for the games tic-tac-toe and connect 4. You can refer to them for examples.

### SearchEngine options:
SearchEngine can be configured with the following options:

**PreventLoops:**
In some games - such as tic-tac-toe or connect 4 - loops are impossible. In others - like chess - loops can be quite common. If this flag is set to true, the program will automatically recognize loop situations and not look any deeper when they occur.
Note that this will only work if Equals is implement in a meaningful way on your states.

**RememberDeadEndStates:**
If set to true, the engine will remember any states from which all paths lead to end-state, so that next time it won't need to calculate the whole search-tree.
Again, note that this will only work if Equals is implement in a meaningful way on your states.

**FavorShortPaths:**
If true, the algorithm will favor short solutions over long solutions when they both result in the same score.
If this option is off, you may experience seemingly weird behavior. Say the algorithm sees that Min can set a trap that will end in Max's defeat in six moves. Without favoring short paths, the algorithm might decide to "give up", causing Max to perform random moves, and possibly lose much sooner - even if its opponent may not have noticed the trap.

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


