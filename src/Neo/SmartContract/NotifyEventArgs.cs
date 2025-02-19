// Copyright (C) 2015-2025 The Neo Project.
//
// NotifyEventArgs.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Extensions;
using Neo.Network.P2P.Payloads;
using Neo.VM;
using Neo.VM.Types;
using System;
using Array = Neo.VM.Types.Array;

namespace Neo.SmartContract
{
    /// <summary>
    /// The <see cref="EventArgs"/> of <see cref="ApplicationEngine.Notify"/>.
    /// </summary>
    public class NotifyEventArgs : EventArgs, IInteroperable
    {
        /// <summary>
        /// The container that containing the executed script.
        /// </summary>
        public IVerifiable ScriptContainer { get; }

        /// <summary>
        /// The script hash of the contract that sends the log.
        /// </summary>
        public UInt160 ScriptHash { get; }

        /// <summary>
        /// The name of the event.
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// The arguments of the event.
        /// </summary>
        public Array State { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyEventArgs"/> class.
        /// </summary>
        /// <param name="container">The container that containing the executed script.</param>
        /// <param name="script_hash">The script hash of the contract that sends the log.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="state">The arguments of the event.</param>
        public NotifyEventArgs(IVerifiable container, UInt160 script_hash, string eventName, Array state)
        {
            ScriptContainer = container;
            ScriptHash = script_hash;
            EventName = eventName;
            State = state;
        }

        public void FromStackItem(StackItem stackItem)
        {
            throw new NotSupportedException();
        }

        public StackItem ToStackItem(IReferenceCounter referenceCounter)
        {
            return new Array(referenceCounter)
                {
                    ScriptHash.ToArray(),
                    EventName,
                    State
                };
        }

        public StackItem ToStackItem(IReferenceCounter referenceCounter, ApplicationEngine engine)
        {
            if (engine.IsHardforkEnabled(Hardfork.HF_Domovoi))
            {
                return new Array(referenceCounter)
                {
                    ScriptHash.ToArray(),
                    EventName,
                    State.OnStack ? State : State.DeepCopy(true)
                };
            }

            return new Array(referenceCounter)
                {
                    ScriptHash.ToArray(),
                    EventName,
                    State
                };
        }
    }
}
