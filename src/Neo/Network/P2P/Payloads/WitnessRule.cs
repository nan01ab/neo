// Copyright (C) 2015-2025 The Neo Project.
//
// WitnessRule.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Extensions;
using Neo.IO;
using Neo.Json;
using Neo.Network.P2P.Payloads.Conditions;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Array = Neo.VM.Types.Array;

namespace Neo.Network.P2P.Payloads
{
    /// <summary>
    /// The rule used to describe the scope of the witness.
    /// </summary>
    public class WitnessRule : IInteroperable, ISerializable, IEquatable<WitnessRule>
    {
        /// <summary>
        /// Indicates the action to be taken if the current context meets with the rule.
        /// </summary>
        public WitnessRuleAction Action;

        /// <summary>
        /// The condition of the rule.
        /// </summary>
        public WitnessCondition Condition;

        int ISerializable.Size => sizeof(WitnessRuleAction) + Condition.Size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(WitnessRule other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            return Action == other.Action &&
                Condition == other.Condition;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return obj is WitnessRule wr && Equals(wr);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Action, Condition.GetHashCode());
        }

        void ISerializable.Deserialize(ref MemoryReader reader)
        {
            Action = (WitnessRuleAction)reader.ReadByte();
            if (Action != WitnessRuleAction.Allow && Action != WitnessRuleAction.Deny)
                throw new FormatException($"Invalid action: {Action}.");
            Condition = WitnessCondition.DeserializeFrom(ref reader, WitnessCondition.MaxNestingDepth);
        }

        void ISerializable.Serialize(BinaryWriter writer)
        {
            writer.Write((byte)Action);
            writer.Write(Condition);
        }

        /// <summary>
        /// Converts the <see cref="WitnessRule"/> from a JSON object.
        /// </summary>
        /// <param name="json">The <see cref="WitnessRule"/> represented by a JSON object.</param>
        /// <returns>The converted <see cref="WitnessRule"/>.</returns>
        public static WitnessRule FromJson(JObject json)
        {
            WitnessRuleAction action = Enum.Parse<WitnessRuleAction>(json["action"].GetString());
            if (action != WitnessRuleAction.Allow && action != WitnessRuleAction.Deny)
                throw new FormatException($"Invalid action: {action}.");

            return new()
            {
                Action = action,
                Condition = WitnessCondition.FromJson((JObject)json["condition"], WitnessCondition.MaxNestingDepth)
            };
        }

        /// <summary>
        /// Converts the rule to a JSON object.
        /// </summary>
        /// <returns>The rule represented by a JSON object.</returns>
        public JObject ToJson()
        {
            return new JObject
            {
                ["action"] = Action,
                ["condition"] = Condition.ToJson()
            };
        }

        void IInteroperable.FromStackItem(StackItem stackItem)
        {
            throw new NotSupportedException();
        }

        public StackItem ToStackItem(IReferenceCounter referenceCounter)
        {
            return new Array(referenceCounter, new StackItem[]
            {
                (byte)Action,
                Condition.ToStackItem(referenceCounter)
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(WitnessRule left, WitnessRule right)
        {
            if (left is null || right is null)
                return Equals(left, right);

            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(WitnessRule left, WitnessRule right)
        {
            if (left is null || right is null)
                return !Equals(left, right);

            return !left.Equals(right);
        }
    }
}
