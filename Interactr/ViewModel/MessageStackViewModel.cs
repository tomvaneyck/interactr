using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interactr.Model;
using Interactr.Reactive;
using Message = Interactr.Model.Message;

namespace Interactr.ViewModel
{
    /// <summary>
    /// The message stack view model.
    /// </summary>
    public class MessageStackViewModel
    {
        /// <summary>
        /// message view models that map to the messages in the diagram.
        /// The index of the message view model in the list is equal to the tick of the message view model.
        /// </summary>
        public ReactiveList<MessageViewModel> MessageViewModels { get; }

        /// <summary>
        /// The activation bar view models.
        /// </summary>
        public ReactiveList<ActivationBarViewModel> ActivationBars { get; } =
            new ReactiveList<ActivationBarViewModel>();

        public MessageStackViewModel(Diagram diagram)
        {
            // Map Messages to MessageViewModels
            MessageViewModels = diagram.Messages.CreateDerivedList(msg => new MessageViewModel(msg, 0)).ResultList;

            // When the diagram changes, recalculate layout.
            Observable.Merge(
                diagram.Messages.OnAdd.Select(_ => Unit.Default),
                diagram.Messages.OnDelete.Select(_ => Unit.Default),
                diagram.Messages.ObserveEach(msg => msg.ReceiverChanged).Select(_ => Unit.Default),
                diagram.Messages.ObserveEach(msg => msg.SenderChanged).Select(_ => Unit.Default)
            ).Subscribe(_ => CalculateLayout());
        }

        /// <summary>
        /// Recalculate the layout of the messages and activation bars in the messageStack view model.
        /// </summary>
        private void CalculateLayout()
        {
            ActivationBars.Clear();

            // Handle the edge case where there are no messages.
            if (MessageViewModels.Count == 0)
            {
                return;
            }

            // Iterate over messages, tick of message = index in list
            for (int i = 0; i < MessageViewModels.Count; i++)
            {
                MessageViewModels[i].Tick = i;
            }

            // Iterate over messages, maintain stack, create activation bar on pop.
            // Push = invocation message
            // Pop = result message
            Stack<(Party Party, int StartTick)> stack = new Stack<(Party Party, int Index)>();
            foreach (MessageViewModel messageVM in MessageViewModels)
            {
                Message message = messageVM.Message;

                if (message.Type == Message.MessageType.Invocation)
                {
                    stack.Push((message.Receiver, messageVM.Tick));
                }
                else
                {
                    (Party receivingParty, int startTick) = stack.Pop();
                    if (receivingParty != messageVM.Receiver)
                    {
                        // Messages are not balanced, abort!!!
                        return;
                    }

                    ActivationBars.Add(new ActivationBarViewModel(message.Receiver, startTick, messageVM.Tick));
                }
            }

            // Add activation bar for initiator starting at tick 0, ending at last message tick.
            ActivationBars.Add(new ActivationBarViewModel(MessageViewModels[0].Message.Sender, 0,
                MessageViewModels.Last().Tick));
        }

        /// <summary>
        /// Create the lifeline view model for a specific party.
        /// </summary>
        /// <param name="party"> The party to create a lifeline for.</param>
        /// <returns></returns>
        public LifeLineViewModel CreateLifeLineForParty(PartyViewModel party)
        {
            return new LifeLineViewModel(this, party);
        }
    }
}