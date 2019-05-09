using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MinMaxSearch
{
    class SearchWorker
    {
        private readonly SearchOptions searchOptions;
        private readonly ThreadManager threadManager;
        private readonly DeterministicSearchUtils deterministicSearchUtils;
        private readonly ProbabilisticSearchUtils probabilisticSearchUtils;
        
        public SearchWorker(SearchOptions searchOptions)
        {
            this.searchOptions = searchOptions;
            threadManager = new ThreadManager(searchOptions.MaxDegreeOfParallelism);
            deterministicSearchUtils = new DeterministicSearchUtils(this, searchOptions, threadManager);
            probabilisticSearchUtils = new ProbabilisticSearchUtils(threadManager, deterministicSearchUtils);
        }

        public SearchResult Evaluate(IState startState, int maxDepth, int depth, double alpha, double bata,
            CancellationToken cancellationToken, List<IState> statesUpToNow)
        {
            if (startState.Turn == Player.Empty)
                throw new EmptyPlayerException(nameof(startState.Turn) + " can't be " + nameof(Player.Empty));

            if (searchOptions.Pruners.Any(pruner => pruner.ShouldPrune(startState, depth, statesUpToNow)))
                return new SearchResult(startState.Evaluate(depth, statesUpToNow), new List<IState> { startState }, 1, 0, true);

            if (ShouldStop(startState, maxDepth, depth, cancellationToken, statesUpToNow))
                return new SearchResult(startState.Evaluate(depth, statesUpToNow), new List<IState> {startState}, 1, 0, false);
            
            if (startState is IDeterministicState deterministicState)
                return deterministicSearchUtils.EvaluateChildren(deterministicState, maxDepth, depth, alpha, bata, cancellationToken, statesUpToNow);

            if (startState is IProbabilisticState probabilisticState)
                return probabilisticSearchUtils.EvaluateChildren(probabilisticState, maxDepth, depth, cancellationToken, statesUpToNow);
            
            throw new BadStateTypeException($"State must implement {nameof(IDeterministicState)} or {nameof(IProbabilisticState)}");
        }
        
        private bool ShouldStop(IState state, int maxDepth, int depth, CancellationToken cancellationToken, List<IState> passedStates)
        {
            if (depth >= maxDepth && !searchOptions.IsUnstableState(state, depth, passedStates))
                return true;
            if (cancellationToken.IsCancellationRequested)
                return true;
            
            return false;
        }
    }
}
