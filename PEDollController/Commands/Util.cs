﻿using System;
using System.Text;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{
    static class Util
    {
        public static readonly Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>()
        {
            { "ps", new CmdPs() },
            { "rem", new CmdRem() },
            { "end", new CmdEnd() },
            { "help", new CmdHelp() },
            { "load", new CmdLoad() },
            { "exit", new CmdExit() },
            { "doll", new CmdDoll() },
            { "kill", new CmdKill() },
            { "hook", new CmdHook() },
            { "eval", new CmdEval() },
            { "dump", new CmdDump() },
            { "shell", new CmdShell() },
            { "break", new CmdBreak() },
            { "listen", new CmdListen() },
            { "target", new CmdTarget() },
            { "unhook", new CmdUnhook() },
            { "loaddll", new CmdLoadDll() },
            { "verdict", new CmdVerdict() },
        };

        public static void Invoke(string cmd)
        {
            // Remove prefix whitespaces
            cmd = cmd.Trim();

            string[] cmdExplode = cmd.Split(' ');
            string cmdVerb = cmdExplode[0]; // cmdExplode will have element as long as cmd is not null

            // Any command that's empty or start with a # is considered as a comment
            if (String.IsNullOrEmpty(cmdVerb) || cmdVerb.StartsWith("#"))
                cmdVerb = "rem";

            if (!Commands.ContainsKey(cmdVerb))
                throw new ArgumentException(Program.GetResourceString("Commands.Unknown", cmdVerb));

            Dictionary<string, object> options;
            try
            {
                options = Commands[cmdVerb].Parse(cmd);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(Program.GetResourceString("Commands.Invalid", cmd, cmdVerb), e.Message);
            }

            Commands[cmdVerb].Invoke(options);
        }

        public static List<string> ParseOptions(string cmd, OptionSet options)
        {
            try
            {
                return options.Parse(Commandline.ToArgs(cmd));
            }
            catch (OptionException e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public static string RemoveQuotes(string x, bool doUnescape = false)
        {
            if (!x.StartsWith("\"") || !x.EndsWith("\""))
                return x;

            string content = x.Substring(1, x.Length - 2);
            if (!doUnescape)
                return content;

            StringBuilder builder = new StringBuilder();
            bool isEscaped = false;
            foreach(char c in content)
            {
                if (!isEscaped && c == '\\')
                    isEscaped = true;
                else
                    builder.Append(c);
            }

            if (isEscaped)
                throw new ArgumentException(Program.GetResourceString("Commands.Incomplete"));

            return builder.ToString();
        }

        public static string Win32ErrorToMessage(int code)
        {
            return Program.GetResourceString("Commands.Win32Error", code, new System.ComponentModel.Win32Exception(code).Message);
        }
    }
}
