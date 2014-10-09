﻿using System.Threading.Tasks;

namespace Banzai
{
    /// <summary>
    /// Defines a node in which the first matching ShouldExecute() node is executed.
    /// </summary>
    /// <typeparam name="T">Type on which the node operates.</typeparam>
    public interface IFirstMatchNodeBase<T> : IMultiNode<T>
    {
    }

    /// <summary>
    /// Defines a base node in which the first matching ShouldExecute() node is executed.
    /// Inherit from this class for custom FirstMatchNodes.
    /// </summary>
    /// <typeparam name="T">Type on which the node operates.</typeparam>
    public abstract class FirstMatchNodeBase<T> : MultiNode<T>, IFirstMatchNodeBase<T>
    {
        /// <summary>
        /// Executes child nodes of the current node.
        /// </summary>
        /// <param name="context">Current ExecutionContext.</param>
        /// <returns>NodeResultStatus representing the current node result.</returns>
        protected override async Task<NodeResultStatus> ExecuteChildrenAsync(ExecutionContext<T> context)
        {
            NodeResult<T> result = null;
            
            foreach (var childNode in Children)
            {
                result = await childNode.ExecuteAsync(context).ConfigureAwait(false);

                if (result.Status != NodeResultStatus.NotRun)
                    break;
            }

            if(result != null)
                return result.Status;

            return NodeResultStatus.NotRun;
        }

    }
}