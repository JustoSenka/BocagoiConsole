using System;

namespace BocagoiConsole.Common;

using BocagoiConsole.Singletons;
using System.Collections.Generic;

public static class ConsoleHelper
{
    private const char emptyChar = ' ';

    public static string ReadLine()
    {
        return GlobalSettings.Instance.Data.UseDoubleSpaceConsole ? CustomReadLine() : Console.ReadLine();
    }

    public static string CustomReadLine()
    {
        List<char> list = new List<char>();
        CustomReadLineInternal(
            GetLength: () => list.Count,
            GetFullStringChars: () => list,
            GetLastChar: () => list.Count == 0 ? '\0' : list[list.Count - 1],
            Append: list.Add,
            InsertAt: list.Insert,
            RemoveAt: list.RemoveAt,
            RemoveRange: list.RemoveRange,
            Clear: list.Clear);
        return string.Concat(list).TrimEnd();
    }

    private static void CustomReadLineInternal(
        Func<int> GetLength,
        Func<IEnumerable<char>> GetFullStringChars,
        Func<char> GetLastChar,
        Action<char> Append,
        Action<int, char> InsertAt,
        Action<int> RemoveAt,
        Action<int, int> RemoveRange = null,
        Action Clear = null)
    {
        int position = 0;

        RemoveRange ??= (index, length) =>
        {
            for (int i = 0; i < length; i++)
            {
                RemoveAt(index);
            }
        };

        Clear ??= () => RemoveRange(0, GetLength());

        static void MoveNegative(int count)
        {
            int bufferWidth = Console.BufferWidth;
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            for (int i = 0; i < count; i++)
            {
                if (left > 0)
                {
                    left--;
                }
                else
                {
                    top--;
                    left = bufferWidth - 1;
                }
            }
            Console.CursorLeft = left;
            Console.CursorTop = top;
        }

        static void MovePositive(int count)
        {
            int bufferWidth = Console.BufferWidth;
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            for (int i = 0; i < count; i++)
            {
                if (left == bufferWidth - 1)
                {
                    top++;
                    left = 0;
                }
                else
                {
                    left++;
                }
            }
            Console.CursorLeft = left;
            Console.CursorTop = top;
        }

        void MoveToOrigin() => MoveNegative(position);

        void MoveToTail() => MovePositive(GetLength() - position);

        static void ConsoleWriteChar(char @char)
        {
            // TODO: This does not need to write to console,
            // but we need to deal with cursor position somehow

            int temp = Console.CursorLeft;
            Console.Write(@char);
            if (Console.CursorLeft == temp)
            {
                MovePositive(1);
            }
        }

        static void ConsoleWriteString(string @string)
        {
            // TODO: This does not need to write to console,
            // but we need to deal with cursor position somehow

            foreach (char c in @string)
            {
                ConsoleWriteChar(c);
            }
        }

        void ConsoleRewriteFullString()
        {
            MoveToOrigin();

            foreach (char c in GetFullStringChars())
            {
                int temp = Console.CursorLeft;
                Console.Write(c);
                if (Console.CursorLeft == temp)
                {
                    MovePositive(1);
                }
            }

            MoveNegative(GetLength() - position);
        }

        while (true)
        {
            ConsoleRewriteFullString();

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (keyInfo.Key is ConsoleKey.Enter)
            {
                if (!keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) &&
                    !keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) &&
                    !keyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt))
                {
                    MovePositive(GetLength() - position);
                    Console.WriteLine();
                    break;
                }
            }
            else if (keyInfo.Key is ConsoleKey.Backspace)
            {
                if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
                {
                    MoveToOrigin();
                    ConsoleWriteString(new string(emptyChar, GetLength() - position) + new string(' ', position));
                    MoveNegative(GetLength());
                    RemoveRange(0, position);
                    position = 0;
                }
                else if (position > 0)
                {
                    if (position == GetLength())
                    {
                        MoveNegative(1);
                        ConsoleWriteChar(' ');
                        MoveNegative(1);
                    }
                    else
                    {
                        MoveToTail();
                        MoveNegative(1);
                        ConsoleWriteChar(' ');
                        MoveNegative(GetLength() - position + 1);
                    }
                    RemoveAt(position - 1);
                    position--;
                }
            }
            else if (keyInfo.Key is ConsoleKey.Delete)
            {
                if (!keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) &&
                    !keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) &&
                    !keyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt))
                {
                    if (position < GetLength())
                    {
                        int left = Console.CursorLeft;
                        int top = Console.CursorTop;
                        MoveToTail();
                        MoveNegative(1);
                        ConsoleWriteChar(' ');
                        Console.CursorLeft = left;
                        Console.CursorTop = top;
                        RemoveAt(position);
                        continue;
                    }
                }
            }
            else if (keyInfo.Key is ConsoleKey.Escape)
            {
                if (!keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) &&
                    !keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) &&
                    !keyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt))
                {
                    MoveToOrigin();
                    int left = Console.CursorLeft;
                    int top = Console.CursorTop;
                    ConsoleWriteString(new string(' ', GetLength()));
                    Console.CursorLeft = left;
                    Console.CursorTop = top;
                    Clear();
                    position = 0;
                }
            }
            else if (keyInfo.Key is ConsoleKey.Home)
            {
                if (!keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) &&
                    !keyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt))
                {
                    if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        MoveToOrigin();
                        ConsoleWriteString(new string(emptyChar, GetLength() - position) + new string(' ', position));
                        MoveNegative(GetLength());
                        RemoveRange(0, position);
                        position = 0;
                    }
                    else
                    {
                        MoveToOrigin();
                        position = 0;
                    }
                }
            }
            else if (keyInfo.Key is ConsoleKey.End)
            {
                if (!keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) &&
                    !keyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt))
                {
                    if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        MoveToOrigin();
                        ConsoleWriteString(new string(emptyChar, position) + new string(' ', GetLength() - position));
                        MoveNegative(GetLength() - position);
                        RemoveRange(position, GetLength() - position);
                    }
                    else
                    {
                        MoveToTail();
                        position = GetLength();
                    }
                }
            }
            else if (keyInfo.Key is ConsoleKey.LeftArrow)
            {
                if (!keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) &&
                    !keyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt))
                {
                    if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        MoveToOrigin();
                        position = 0;
                    }
                    else
                    {
                        if (position > 0)
                        {
                            MoveNegative(1);
                            position--;
                        }
                    }
                }
            }
            else if (keyInfo.Key is ConsoleKey.RightArrow)
            {
                if (!keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) &&
                    !keyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt))
                {
                    if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        MoveToTail();
                        position = GetLength();
                    }
                    else
                    {
                        if (position < GetLength())
                        {
                            MovePositive(1);
                            position++;
                        }
                    }
                }
            }
            else
            {
                if (!(keyInfo.KeyChar is '\0'))
                {
                    if (keyInfo.KeyChar == ' ' && GetLastChar() == ' ')
                    {
                        MovePositive(GetLength() - position);
                        Console.WriteLine();
                        break;
                    }

                    if (position == GetLength())
                    {
                        ConsoleWriteChar(keyInfo.KeyChar);
                        Append(keyInfo.KeyChar);
                        position++;
                    }
                    else
                    {
                        int left = Console.CursorLeft;
                        int top = Console.CursorTop;
                        MoveToTail();
                        ConsoleWriteChar(keyInfo.KeyChar);
                        Console.CursorLeft = left;
                        Console.CursorTop = top;
                        MovePositive(1);
                        InsertAt(position, keyInfo.KeyChar);
                        position++;
                    }
                }
            }
        }
    }
}
