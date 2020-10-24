// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Control

    open System.Threading

    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Control

    [<Sealed; CompiledName("FSharpAsyncReplyChannel`1")>]
    /// <summary>A handle to a capability to reply to a PostAndReply message.</summary>
    ///
    /// <category index="4">Agents</category>
    type AsyncReplyChannel<'Reply> =
        /// <summary>Sends a reply to a PostAndReply message.</summary>
        /// <param name="value">The value to send.</param>
        member Reply : value:'Reply -> unit

    /// <summary>A message-processing agent which executes an asynchronous computation.</summary>
    ///
    /// <remarks>The agent encapsulates a message queue that supports multiple-writers and 
    /// a single reader agent. Writers send messages to the agent by using the Post 
    /// method and its variations.
    ///
    /// The agent may wait for messages using the Receive or TryReceive methods or
    /// scan through all available messages using the Scan or TryScan method.</remarks>
    ///
    /// <category index="4">Agents</category>
    [<Sealed; AutoSerializable(false); CompiledName("FSharpMailboxProcessor`1")>]
    type MailboxProcessor<'Msg> =

        /// <summary>Creates an agent. The <c>body</c> function is used to generate the asynchronous 
        /// computation executed by the agent. This function is not executed until 
        /// <c>Start</c> is called.</summary>
        ///
        /// <param name="body">The function to produce an asynchronous computation that will be executed
        /// as the read loop for the MailboxProcessor when Start is called.</param>
        /// <param name="cancellationToken">An optional cancellation token for the <c>body</c>.
        /// Defaults to <c>Async.DefaultCancellationToken</c>.</param>
        ///
        /// <returns>The created MailboxProcessor.</returns>
        new :  body:(MailboxProcessor<'Msg> -> Async<unit>) * ?cancellationToken: CancellationToken -> MailboxProcessor<'Msg>

        /// <summary>Creates and starts an agent. The <c>body</c> function is used to generate the asynchronous 
        /// computation executed by the agent.</summary>
        ///
        /// <param name="body">The function to produce an asynchronous computation that will be executed
        /// as the read loop for the MailboxProcessor when Start is called.</param>
        /// <param name="cancellationToken">An optional cancellation token for the <c>body</c>.
        /// Defaults to <c>Async.DefaultCancellationToken</c>.</param>
        ///
        /// <returns>The created MailboxProcessor.</returns>
        static member Start  :  body:(MailboxProcessor<'Msg> -> Async<unit>) * ?cancellationToken: CancellationToken -> MailboxProcessor<'Msg>

        /// <summary>Posts a message to the message queue of the MailboxProcessor, asynchronously.</summary>
        ///
        /// <param name="message">The message to post.</param>
        member Post : message:'Msg -> unit

        /// <summary>Posts a message to an agent and await a reply on the channel, synchronously.</summary>
        ///
        /// <remarks>The message is generated by applying <c>buildMessage</c> to a new reply channel 
        /// to be incorporated into the message. The receiving agent must process this 
        /// message and invoke the Reply method on this reply channel precisely once.</remarks>
        /// <param name="buildMessage">The function to incorporate the AsyncReplyChannel into
        /// the message to be sent.</param>
        /// <param name="timeout">An optional timeout parameter (in milliseconds) to wait for a reply message.
        /// Defaults to -1 which corresponds to <see cref="F:System.Threading.Timeout.Infinite"/>.</param>
        ///
        /// <returns>The reply from the agent.</returns>
        member PostAndReply : buildMessage:(AsyncReplyChannel<'Reply> -> 'Msg) * ?timeout : int -> 'Reply

        /// <summary>Posts a message to an agent and await a reply on the channel, asynchronously.</summary> 
        ///
        /// <remarks>The message is generated by applying <c>buildMessage</c> to a new reply channel 
        /// to be incorporated into the message. The receiving agent must process this 
        /// message and invoke the Reply method on this reply channel precisely once.</remarks>
        /// <param name="buildMessage">The function to incorporate the AsyncReplyChannel into
        /// the message to be sent.</param>
        /// <param name="timeout">An optional timeout parameter (in milliseconds) to wait for a reply message.
        /// Defaults to -1 which corresponds to <see cref="F:System.Threading.Timeout.Infinite"/>.</param>
        ///
        /// <returns>An asynchronous computation that will wait for the reply from the agent.</returns>
        member PostAndAsyncReply : buildMessage:(AsyncReplyChannel<'Reply> -> 'Msg) * ?timeout : int -> Async<'Reply>

        /// <summary>Like PostAndReply, but returns None if no reply within the timeout period.</summary>
        ///
        /// <param name="buildMessage">The function to incorporate the AsyncReplyChannel into
        /// the message to be sent.</param>
        /// <param name="timeout">An optional timeout parameter (in milliseconds) to wait for a reply message.
        /// Defaults to -1 which corresponds to <see cref="F:System.Threading.Timeout.Infinite"/>.</param>
        ///
        /// <returns>The reply from the agent or None if the timeout expires.</returns> 
        member TryPostAndReply : buildMessage:(AsyncReplyChannel<'Reply> -> 'Msg) * ?timeout : int -> 'Reply option

        /// <summary>Like AsyncPostAndReply, but returns None if no reply within the timeout period.</summary>
        ///
        /// <param name="buildMessage">The function to incorporate the AsyncReplyChannel into
        /// the message to be sent.</param>
        /// <param name="timeout">An optional timeout parameter (in milliseconds) to wait for a reply message.
        /// Defaults to -1 which corresponds to <see cref="F:System.Threading.Timeout.Infinite"/>.</param>
        ///
        /// <returns>An asynchronous computation that will return the reply or None if the timeout expires.</returns> 
        member PostAndTryAsyncReply : buildMessage:(AsyncReplyChannel<'Reply> -> 'Msg) * ?timeout : int -> Async<'Reply option>

        /// <summary>Waits for a message. This will consume the first message in arrival order.</summary> 
        ///
        /// <remarks>This method is for use within the body of the agent. 
        ///
        /// This method is for use within the body of the agent. For each agent, at most 
        /// one concurrent reader may be active, so no more than one concurrent call to 
        /// Receive, TryReceive, Scan and/or TryScan may be active.</remarks>
        /// <param name="timeout">An optional timeout in milliseconds. Defaults to -1 which corresponds
        /// to <see cref="F:System.Threading.Timeout.Infinite"/>.</param>
        ///
        /// <returns>An asynchronous computation that returns the received message.</returns>
        /// <exception cref="T:System.TimeoutException">Thrown when the timeout is exceeded.</exception>
        member Receive : ?timeout:int -> Async<'Msg>

        /// <summary>Waits for a message. This will consume the first message in arrival order.</summary> 
        ///
        /// <remarks>This method is for use within the body of the agent. 
        ///
        /// Returns None if a timeout is given and the timeout is exceeded.
        ///
        /// This method is for use within the body of the agent. For each agent, at most 
        /// one concurrent reader may be active, so no more than one concurrent call to 
        /// Receive, TryReceive, Scan and/or TryScan may be active.</remarks>
        /// <param name="timeout">An optional timeout in milliseconds. Defaults to -1 which
        /// corresponds to <see cref="F:System.Threading.Timeout.Infinite"/>.</param>
        ///
        /// <returns>An asynchronous computation that returns the received message or
        /// None if the timeout is exceeded.</returns>
        member TryReceive : ?timeout:int -> Async<'Msg option>
        
        /// <summary>Scans for a message by looking through messages in arrival order until <c>scanner</c> 
        /// returns a Some value. Other messages remain in the queue.</summary>
        ///
        /// <remarks>Returns None if a timeout is given and the timeout is exceeded.
        ///
        /// This method is for use within the body of the agent. For each agent, at most 
        /// one concurrent reader may be active, so no more than one concurrent call to 
        /// Receive, TryReceive, Scan and/or TryScan may be active.</remarks>
        /// <param name="scanner">The function to return None if the message is to be skipped
        /// or Some if the message is to be processed and removed from the queue.</param>
        /// <param name="timeout">An optional timeout in milliseconds. Defaults to -1 which corresponds
        /// to <see cref="F:System.Threading.Timeout.Infinite"/>.</param>
        ///
        /// <returns>An asynchronous computation that <c>scanner</c> built off the read message.</returns>
        /// <exception cref="T:System.TimeoutException">Thrown when the timeout is exceeded.</exception>
        member Scan : scanner:('Msg -> (Async<'T>) option) * ?timeout:int -> Async<'T>

        /// <summary>Scans for a message by looking through messages in arrival order until <c>scanner</c> 
        /// returns a Some value. Other messages remain in the queue.</summary>
        ///
        /// <remarks>This method is for use within the body of the agent. For each agent, at most 
        /// one concurrent reader may be active, so no more than one concurrent call to 
        /// Receive, TryReceive, Scan and/or TryScan may be active.</remarks>
        /// <param name="scanner">The function to return None if the message is to be skipped
        /// or Some if the message is to be processed and removed from the queue.</param>
        /// <param name="timeout">An optional timeout in milliseconds. Defaults to -1 which corresponds
        /// to <see cref="F:System.Threading.Timeout.Infinite"/>.</param>
        ///
        /// <returns>An asynchronous computation that <c>scanner</c> built off the read message.</returns>
        member TryScan : scanner:('Msg -> (Async<'T>) option) * ?timeout:int -> Async<'T option>

        /// <summary>Starts the agent.</summary>
        member Start : unit -> unit

        /// <summary>Raises a timeout exception if a message not received in this amount of time. By default
        /// no timeout is used.</summary>
        member DefaultTimeout : int with get, set
        
        /// <summary>Occurs when the execution of the agent results in an exception.</summary>
        [<CLIEvent>]
        member Error : IEvent<System.Exception>

        interface System.IDisposable

        /// <summary>Returns the number of unprocessed messages in the message queue of the agent.</summary>
        member CurrentQueueLength : int
