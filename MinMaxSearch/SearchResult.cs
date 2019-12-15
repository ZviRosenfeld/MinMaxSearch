using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinMaxSearch.Exceptions;

namespace MinMaxSearch
{
    public class SearchResult
    {
        public SearchResult(double evaluation, List<IState> stateSequence, int leaves, int internalNodes,
            bool fullTreeSearched, bool allChildrenAreDeadEnds)
        {
            Evaluation = evaluation;
            StateSequence = stateSequence;
            Leaves = leaves;
            InternalNodes = internalNodes;
            FullTreeSearched = fullTreeSearched;
            AllChildrenAreDeadEnds = allChildrenAreDeadEnds;
            if (allChildrenAreDeadEnds && !fullTreeSearched)
                throw new InternalException($"Code 1002 ({nameof(allChildrenAreDeadEnds)} but not {nameof(fullTreeSearched)})");

            IsSearchCompleted = true;
            SearchDepth = -1;
            SearchTime = TimeSpan.Zero;
        }

        public SearchResult(double evaluation, IState endState, bool fullTreeSearched = true, bool allChildrenAreDeadEnds = true)
        {
            Evaluation = evaluation;
            IsSearchCompleted = true;
            SearchDepth = 0;
            StateSequence = new List<IState> {endState};
            Leaves = 1;
            InternalNodes = 0;
            FullTreeSearched = fullTreeSearched;
            AllChildrenAreDeadEnds = allChildrenAreDeadEnds;
            if (allChildrenAreDeadEnds && !fullTreeSearched)
                throw new InternalException($"Code 1003 ({nameof(allChildrenAreDeadEnds)} but not {nameof(fullTreeSearched)})");

            SearchTime = TimeSpan.Zero;
        }

        public SearchResult(SearchResult other, TimeSpan searchTime, int searchDepth, bool isSearchCompleted)
        {
            Evaluation = other.Evaluation;
            StateSequence = other.StateSequence.ToList();
            Leaves = other.Leaves;
            InternalNodes = other.InternalNodes;
            FullTreeSearched = other.FullTreeSearched;
            AllChildrenAreDeadEnds = other.AllChildrenAreDeadEnds;
            SearchTime = searchTime;
            IsSearchCompleted = isSearchCompleted;
            SearchDepth = searchDepth;
        }

        public IState NextMove => StateSequence.First();

        public double Evaluation { get; }

        /// <summary>
        /// Note that this will be empty for probabilistic states.
        /// Also, this might be cut off if you're using caching (the default is to use caching).
        /// </summary>
        public List<IState> StateSequence { get; }
        
        /// <summary>
        /// The number of leaves in the search tree
        /// </summary>
        public int Leaves { get; }

        /// <summary>
        /// The number of InternalNodes in the search tree
        /// </summary>
        public int InternalNodes { get; }

        /// <summary>
        /// Returns true if the full tree was searched. This will return true even if some of the branches were prunned.
        /// </summary>
        public bool FullTreeSearched { get; }

        /// <summary>
        /// Returns true only if all paths lead to daed ends. If part of the tree was prunned, this will return false.
        /// </summary>
        public bool AllChildrenAreDeadEnds { get; }

        public TimeSpan SearchTime { get; }

        public int SearchDepth { get; }

        /// <summary>
        /// Was the search completed, or stopped early due to a a cancellation token?
        /// In IterativeSearchs IsSearchCompleted will be true as long as at least the first search was completed (even if deeper searches were canceled).
        /// </summary>
        public bool IsSearchCompleted { get; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("SearchResult:");
            stringBuilder.AppendLine(nameof(Evaluation) + " == " + Evaluation);
            stringBuilder.AppendLine(nameof(SearchTime) + " == " + SearchTime);
            stringBuilder.AppendLine(nameof(Leaves) + " == " + Leaves);
            stringBuilder.AppendLine(nameof(InternalNodes) + " == " + InternalNodes);
            stringBuilder.AppendLine(nameof(SearchDepth) + " == " + SearchDepth);
            stringBuilder.AppendLine(nameof(FullTreeSearched) + " == " + FullTreeSearched);
            stringBuilder.AppendLine(nameof(AllChildrenAreDeadEnds) + " == " + AllChildrenAreDeadEnds);
            stringBuilder.AppendLine(nameof(NextMove) + ":");
            stringBuilder.AppendLine(NextMove.ToString());
            
            return stringBuilder.ToString();
        }
    }
}
