using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Channels;

namespace ACL.business.flow
{

    class FlowChannel
    {
        public Channel<string> Output { get; private set; } = Channel.CreateUnbounded<string>();
    }

    class FlowRuntime
    {
        private readonly Graph<AgentTask> blueprint;
        private Node<AgentTask>? current;
        private bool running = false;


        public FlowRuntime(Graph<AgentTask> blueprint)
        {
            current = blueprint?.Head;
            this.blueprint = blueprint;
        }


        public void Configure()
        {
            var channelMap = new Dictionary<string, Channel<string>>();
            blueprint.TraverseVertices(node =>
            {

                var task = node.Data;
                var agent = task.Agent;

                var channel = Channel.CreateBounded<string>(10);
                channelMap[task.Id] = channel;
                agent.Serve(channel);
                
                /**
                 * 什么时候停止？
                 * 
                 *      action  action 
                 *  a(in,out) b(in,out)
                 */
            });
        }

        public void Start()
        {
            if (current == null)
            {
                throw new InvalidFlowException();
            }

            do
            {

                var task = current.Data;
                var agent = task.Agent;
                //agent.Serve


            } while (running);
        }

        public void Stop()
        {
        }
    }
}
