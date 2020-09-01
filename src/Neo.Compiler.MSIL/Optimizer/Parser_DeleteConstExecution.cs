using Neo.VM;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.Optimizer
{
    class Parser_DeleteConstExecution : IOptimizeParser
    {
        private uint OptimizedCount = 0;

        public bool NeedRightAddress => false;
        public bool WillChangeAddress => OptimizedCount > 0;

        public void Init()
        {
            OptimizedCount = 0;
        }

        /// <summary>
        /// Parse
        /// </summary>
        /// <param name="items">Items</param>
        public void Parse(List<INefItem> items)
        {
            for (int x = items.Count - 1; x >= 1; x--)
            {
                if (!(items[x] is NefInstruction ins))
                {
                    continue;
                }

                switch (ins.OpCode)
                {
                    // One stack item

                    case OpCode.ISNULL:
                        {
                            if (items[x - 1] is NefInstruction p1)
                            {
                                if (p1.IsPush(out _))
                                {
                                    items.RemoveRange(x - 1, 1);
                                    OptimizedCount++;
                                    x -= 1;

                                    ins.UpdateForPush(0);
                                }
                                else if (p1.OpCode == OpCode.PUSHNULL)
                                {
                                    items.RemoveRange(x - 1, 1);
                                    OptimizedCount++;
                                    x -= 1;

                                    ins.UpdateForPush(1);
                                }
                            }
                            break;
                        }
                    case OpCode.DROP:
                        {
                            if (items[x - 1] is NefInstruction p1)
                            {
                                if (p1.IsPushOrNull(out _))
                                {
                                    items.RemoveRange(x - 1, 2);
                                    OptimizedCount++;
                                    x -= 1;
                                }
                            }
                            break;
                        }
                    case OpCode.SIGN:
                        {
                            if (items[x - 1] is NefInstruction p1 && p1.IsPush(out var v1))
                            {
                                items.RemoveRange(x - 1, 1);
                                OptimizedCount++;
                                x -= 1;

                                ins.UpdateForPush(v1.Sign);
                            }
                            break;
                        }
                    case OpCode.ABS:
                        {
                            if (items[x - 1] is NefInstruction p1 && p1.IsPush(out var v1) &&
                                BigInteger.Abs(v1).IsValidValue())
                            {
                                items.RemoveRange(x - 1, 1);
                                OptimizedCount++;
                                x -= 1;

                                ins.UpdateForPush(BigInteger.Abs(v1));
                            }
                            break;
                        }
                    case OpCode.NEGATE:
                        {
                            if (items[x - 1] is NefInstruction p1 && p1.IsPush(out var v1) &&
                                (-v1).IsValidValue())
                            {
                                items.RemoveRange(x - 1, 1);
                                OptimizedCount++;
                                x -= 1;

                                ins.UpdateForPush(-v1);
                            }
                            break;
                        }
                    case OpCode.INC:
                        {
                            if (items[x - 1] is NefInstruction p1 && p1.IsPush(out var v1) &&
                                (v1 + 1).IsValidValue())
                            {
                                items.RemoveRange(x - 1, 1);
                                OptimizedCount++;
                                x -= 1;

                                ins.UpdateForPush(v1 + 1);
                            }
                            break;
                        }
                    case OpCode.DEC:
                        {
                            if (items[x - 1] is NefInstruction p1 && p1.IsPush(out var v1) &&
                                (v1 - 1).IsValidValue())
                            {
                                items.RemoveRange(x - 1, 1);
                                OptimizedCount++;
                                x -= 1;

                                ins.UpdateForPush(v1 - 1);
                            }
                            break;
                        }

                    // Two stack items

                    case OpCode.ADD:
                        {
                            if (x >= 2 &&
                                items[x - 1] is NefInstruction p1 &&
                                items[x - 2] is NefInstruction p2 &&
                                p1.IsPush(out var v1) &&
                                p2.IsPush(out var v2)
                                )
                            {
                                var value = v2 + v1;
                                if (value.IsValidValue())
                                {
                                    items.RemoveRange(x - 2, 2);
                                    OptimizedCount++;
                                    x -= 2;

                                    ins.UpdateForPush(value);
                                }
                            }
                            break;
                        }
                    case OpCode.SUB:
                        {
                            if (x >= 2 &&
                                items[x - 1] is NefInstruction p1 &&
                                items[x - 2] is NefInstruction p2 &&
                                p1.IsPush(out var v1) &&
                                p2.IsPush(out var v2)
                                )
                            {
                                var value = v2 - v1;
                                if (value.IsValidValue())
                                {
                                    items.RemoveRange(x - 2, 2);
                                    OptimizedCount++;
                                    x -= 2;

                                    ins.UpdateForPush(value);
                                }
                            }
                            break;
                        }
                    case OpCode.MUL:
                        {
                            if (x >= 2 &&
                                items[x - 1] is NefInstruction p1 &&
                                items[x - 2] is NefInstruction p2 &&
                                p1.IsPush(out var v1) &&
                                p2.IsPush(out var v2)
                                )
                            {
                                var value = v2 * v1;
                                if (value.IsValidValue())
                                {
                                    items.RemoveRange(x - 2, 2);
                                    OptimizedCount++;
                                    x -= 2;

                                    ins.UpdateForPush(value);
                                }
                            }
                            break;
                        }
                    case OpCode.DIV:
                        {
                            if (x >= 2 &&
                                items[x - 1] is NefInstruction p1 &&
                                items[x - 2] is NefInstruction p2 &&
                                p1.IsPush(out var v1) && v1 != BigInteger.Zero &&
                                p2.IsPush(out var v2)
                                )
                            {
                                items.RemoveRange(x - 2, 2);
                                OptimizedCount++;
                                x -= 2;

                                ins.UpdateForPush(v2 / v1);
                            }
                            break;
                        }
                    case OpCode.MOD:
                        {
                            if (x >= 2 &&
                                items[x - 1] is NefInstruction p1 &&
                                items[x - 2] is NefInstruction p2 &&
                                p1.IsPush(out var v1) &&
                                p2.IsPush(out var v2)
                                )
                            {
                                items.RemoveRange(x - 2, 2);
                                OptimizedCount++;
                                x -= 2;

                                ins.UpdateForPush(v2 % v1);
                            }
                            break;
                        }

                    // Three stack items

                    case OpCode.ROT:
                        {
                            if (x >= 3 &&
                                items[x - 1] is NefInstruction p1 &&
                                items[x - 2] is NefInstruction p2 &&
                                items[x - 3] is NefInstruction p3 &&
                                (p1.IsPushOrNull(out _) || p1.IsPushData(out _)) &&
                                (p2.IsPushOrNull(out _) || p2.IsPushData(out _)) &&
                                (p3.IsPushOrNull(out _) || p3.IsPushData(out _))
                                )
                            {
                                items[x - 3] = p2;
                                items[x - 2] = p1;
                                items[x - 1] = p3;

                                items.RemoveRange(x, 1);
                                OptimizedCount++;
                                x -= 3;
                            }
                            break;
                        }
                }
            }
        }
    }
}
