﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;


//using Newtonsoft.Json.Linq;

namespace SplitAndMerge
{
    public partial class Utils
    {
        public static void CheckArgs(int args, int expected, string msg, bool exactMatch = false)
        {
            if (args < expected || (exactMatch && args != expected))
            {
                throw new ArgumentException("Expecting " + expected +
                    " arguments but got " + args + " in " + msg);
            }
        }
        public static void CheckPosInt(Variable variable)
        {
            CheckInteger(variable);
            if (variable.Value <= 0)
            {
                throw new ArgumentException("Expected a positive integer instead of [" +
                                               variable.Value + "]");
            }
        }
        public static void CheckPosInt(int number, string name)
        {
            if (number < 0)
            {
                string realName = Constants.GetRealName(name);
                throw new ArgumentException("Expected a positive integer instead of [" +
                                               number + "] in [" + realName + "]");
            }
        }
        public static void CheckNonNegativeInt(Variable variable)
        {
            CheckInteger(variable);
            if (variable.Value < 0)
            {
                throw new ArgumentException("Expected a non-negative integer instead of [" +
                                               variable.Value + "]");
            }
        }
        public static void CheckInteger(Variable variable)
        {
            CheckNumber(variable);
            if (variable.Value % 1 != 0.0)
            {
                throw new ArgumentException("Expected an integer instead of [" +
                                               variable.Value + "]");
            }
        }
        public static void CheckNumber(Variable variable)
        {
            if (variable.Type != Variable.VarType.NUMBER)
            {
                throw new ArgumentException("Expected a number instead of [" +
                                               variable.AsString() + "]");
            }
        }
        public static void CheckArray(Variable variable, string name)
        {
            if (variable.Tuple == null)
            {
                string realName = Constants.GetRealName(name);
                throw new ArgumentException("An array expected for variable [" +
                                               realName + "]");
            }
        }
        public static void CheckNotEmpty(ParsingScript script, string varName, string name)
        {
            if (!script.StillValid() || string.IsNullOrEmpty(varName))
            {
                string realName = Constants.GetRealName(name);
                throw new ArgumentException("Incomplete arguments for [" + realName + "]");
            }
        }
        public static void CheckNotEnd(ParsingScript script, string name)
        {
            if (!script.StillValid())
            {
                string realName = Constants.GetRealName(name);
                throw new ArgumentException("Incomplete arguments for [" + realName + "]");
            }
        }
        public static void CheckNotNull(object obj, string name, int index = -1)
        {
            if (obj == null)
            {
                string indexStr = index >= 0 ? " in position " + (index + 1) : "";
                string realName = Constants.GetRealName(name);
                throw new ArgumentException("Invalid argument " + indexStr +
                                            " in function [" + realName + "]");
            }
        }
        public static void CheckNotNull(string name, ParserFunction func)
        {
            if (func == null)
            {
                string realName = Constants.GetRealName(name);
                throw new ArgumentException("Variable or function [" + realName + "] doesn't exist");
            }
        }
        public static void CheckNotNull(object obj, string name, ParsingScript script)
        {
            if (obj == null)
            {
                string realName = Constants.GetRealName(name);
                ThrowErrorMsg("Object [" + realName + "] doesn't exist.", script, realName);
            }
        }

        public static void CheckNotEnd(ParsingScript script)
        {
            if (!script.StillValid())
            {
                throw new ArgumentException("Incomplete function definition.");
            }
        }
        public static void CheckNotEmpty(string varName, string name)
        {
            if (string.IsNullOrEmpty(varName))
            {
                string realName = Constants.GetRealName(name);
                throw new ArgumentException("Incomplete arguments for [" + realName + "]");
            }
        }
        public static void CheckForValidName(string name, ParsingScript script)
        {
            string illegals = "\"'?!";
            for (int i = 0; i < illegals.Length; i++)
            {
                char ch = illegals[i];
                if (name.Contains(ch))
                {
                    ThrowErrorMsg("Variable [" + name + "] contains illegal character [" + ch + "]",
                                  script, name);
                }
            }
        }

        static void ThrowErrorMsg(string msg, ParsingScript script, string token)
        {
            string code     = script == null || string.IsNullOrEmpty(script.OriginalScript) ? "" : script.OriginalScript;
            int lineNumber  = script == null ? 0 : script.OriginalLineNumber;
            string filename = script == null || string.IsNullOrEmpty(script.Filename) ? "" : script.Filename;
            int minLines    = script == null || script.OriginalLine.ToLower().Contains(token.ToLower()) ? 1 : 2;

            ThrowErrorMsg(msg, code, lineNumber, filename, minLines);
        }

        static void ThrowErrorMsg(string msg, string script, int lineNumber, string filename = "", int minLines = 1)
        {
            string [] lines = script.Split('\n');
            lineNumber = lines.Length <= lineNumber ? -1 : lineNumber;
            if (lineNumber < 0)
            {
                throw new ParsingException(msg);
            }

            var currentLineNumber = lineNumber;
            var line = lines[lineNumber].Trim();
            var collectMore = line.Length < 3 || minLines > 1;
            var lineContents = line;

            while (collectMore && currentLineNumber > 0)
            {
                line = lines[--currentLineNumber].Trim();
                collectMore = line.Length < 2 || (minLines > lineNumber - currentLineNumber + 1);
                lineContents = line + "  " + lineContents;
            }

            if (lines.Length > 1)
            {
                string lineStr = currentLineNumber == lineNumber ? "Line " + (lineNumber + 1) :
                                 "Lines " + (currentLineNumber + 1) + "-" + (lineNumber + 1);
                msg += " " + lineStr + ": " + lineContents;
            }

            StringBuilder stack = new StringBuilder();
            stack.AppendLine("" + currentLineNumber);
            stack.AppendLine(filename);
            stack.AppendLine(line);

            throw new ParsingException(msg, stack.ToString());
        }

        static void ThrowErrorMsg(string msg, string code, int level, int lineStart, int lineEnd, string filename)
        {
            var lineNumber = level > 0 ? lineStart : lineEnd;
            ThrowErrorMsg(msg, code, lineNumber, filename);
        }

        public static bool ExtractParameterNames(List<Variable> args, string functionName, ParsingScript script)
        {
            CustomFunction custFunc = ParserFunction.GetFunction(functionName, script) as CustomFunction;
            if (custFunc == null)
            {
                return false;
            }

            var realArgs = custFunc.RealArgs;
            for (int i = 0; i < args.Count && i < realArgs.Length; i++)
            {
                string name = args[i].CurrentAssign;
                args[i].ParamName = string.IsNullOrEmpty(name) ? realArgs[i] : name;
            }
            return true;
        }

        public static string GetLine(int chars = 40)
        {
            return string.Format("-").PadRight(chars, '-');
        }

        public static string GetFileText(string filename)
        {
            string fileContents = string.Empty;
            if (File.Exists(filename))
            {
                fileContents = File.ReadAllText(filename);
            }
            else
            {
                throw new ArgumentException("Couldn't read file [" + filename +
                                            "] from disk.");
            }
            return fileContents;
        }

        public static void PrintScript(string script, ParsingScript parentSript)
        {
            StringBuilder item = new StringBuilder();

            bool inQuotes = false;

            for (int i = 0; i < script.Length; i++)
            {
                char ch = script[i];
                inQuotes = ch == Constants.QUOTE ? !inQuotes : inQuotes;

                if (inQuotes)
                {
                    Interpreter.Instance.AppendOutput(ch.ToString());
                    continue;
                }
                if (!Constants.TOKEN_SEPARATION.Contains(ch))
                {
                    item.Append(ch);
                    continue;
                }
                if (item.Length > 0)
                {
                    string token = item.ToString();
                    Interpreter.Instance.AppendOutput(token);
                    item.Length = 0;
                }
                Interpreter.Instance.AppendOutput(ch.ToString());
            }
        }

        public static string[] GetFileLines(string filename)
        {
            try
            {
                string[] lines = File.ReadAllLines(filename);
                return lines;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Couldn't read file [" + filename +
                                            "] from disk: " + ex.Message);
            }
        }

        public static string[] GetFileLines(string filename, int from, int count)
        {
            try
            {
                var allLines = File.ReadAllLines(filename);
                if (allLines.Length <= count)
                {
                    return allLines;
                }

                if (from < 0)
                {
                    // last n lines
                    from = allLines.Length - count;
                }

                string[] lines = allLines.Skip(from).Take(count).ToArray();
                return lines;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Couldn't read file from disk: " + ex.Message);
            }
        }

        public static void WriteFileText(string filename, string text)
        {
            try
            {
                File.WriteAllText(filename, text);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Couldn't write file to disk: " + ex.Message);
            }
        }

        public static void AppendFileText(string filename, string text)
        {
            try
            {
                File.AppendAllText(filename, text);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Couldn't write file to disk: " + ex.Message);
            }
        }

        public static void ThrowException(ParsingScript script, string excName1,
                                          string errorToken = "", string excName2 = "")
        {
#if UNITY_EDITOR == false && UNITY_STANDALONE == false && __ANDROID__ == false && __IOS__ == false
            string msg = Translation.GetErrorString(excName1);
#else
            string msg = excName1;
#endif
            if (!string.IsNullOrEmpty(errorToken))
            {
                msg = string.Format(msg, errorToken);
#if UNITY_EDITOR == false && UNITY_STANDALONE == false && __ANDROID__ == false && __IOS__ == false
                string candidate = Translation.TryFindError(errorToken, script);
#else
                string candidate = null;
#endif


                if (!string.IsNullOrEmpty(candidate) &&
                    !string.IsNullOrEmpty(excName2))
                {
#if UNITY_EDITOR == false && UNITY_STANDALONE == false && __ANDROID__ == false && __IOS__ == false
                    string extra = Translation.GetErrorString(excName2);
#else
                    string extra = excName2;
#endif
                    msg += " " + string.Format(extra, candidate);
                }
            }

            if (!string.IsNullOrEmpty(script.Filename))
            {
#if UNITY_EDITOR == false && UNITY_STANDALONE == false && __ANDROID__ == false && __IOS__ == false
                string fileMsg = Translation.GetErrorString("errorFile");
#else
                string fileMsg = "File: {0}.";
#endif
                msg += Environment.NewLine + string.Format(fileMsg, script.Filename);
            }

            int lineNumber = -1;
            string line = script.GetOriginalLine(out lineNumber);
            if (lineNumber >= 0)
            {
#if UNITY_EDITOR == false && UNITY_STANDALONE == false && __ANDROID__ == false && __IOS__ == false
                string lineMsg = Translation.GetErrorString("errorLine");
#else
                string lineMsg = "Line {0}: [{1}]";
#endif
                msg += string.IsNullOrEmpty(script.Filename) ? Environment.NewLine : " ";
                msg += string.Format(lineMsg, lineNumber + 1, line.Trim());
            }
            throw new ArgumentException(msg);
        }

        public static void PrintList(List<Variable> list, int from)
        {
            Console.Write("Merging list:");
            for (int i = from; i < list.Count; i++)
            {
                Console.Write(" ({0}, '{1}')", list[i].Value, list[i].Action);
            }
            Console.WriteLine();
        }

        public static int GetSafeInt(List<Variable> args, int index, int defaultValue = 0)
        {
            if (args.Count <= index)
            {
                return defaultValue;
            }
            Variable numberVar = args[index];
            if (numberVar.Type != Variable.VarType.NUMBER)
            {
                if (string.IsNullOrEmpty(numberVar.String))
                {
                    return defaultValue;
                }
                int num;
                if (!Int32.TryParse(numberVar.String, NumberStyles.Number,
                                     CultureInfo.InvariantCulture, out num))
                {
                    throw new ArgumentException("Expected an integer instead of [" + numberVar.AsString() + "]");
                }
                return num;
            }
            return numberVar.AsInt();
        }
        public static double GetSafeDouble(List<Variable> args, int index, double defaultValue = 0.0)
        {
            if (args.Count <= index)
            {
                return defaultValue;
            }

            Variable numberVar = args[index];
            if (numberVar.Type != Variable.VarType.NUMBER)
            {
                double num;
                if (!CanConvertToDouble(numberVar.String, out num))
                {
                    throw new ArgumentException("Expected a double instead of [" + numberVar.AsString() + "]");
                }
                return num;
            }
            return numberVar.AsDouble();
        }

        public static string GetSafeString(List<Variable> args, int index, string defaultValue = "")
        {
            if (args.Count <= index)
            {
                return defaultValue;
            }
            return args[index].AsString();
        }
        public static Variable GetSafeVariable(List<Variable> args, int index, Variable defaultValue = null)
        {
            if (args.Count <= index)
            {
                return defaultValue;
            }
            return args[index];
        }

        public static string GetSafeToken(List<Variable> args, int index, string defaultValue = "")
        {
            if (args.Count <= index)
            {
                return defaultValue;
            }

            Variable var = args[index];
            string token = var.ParsingToken;

            return token;
        }

        public static Variable GetVariable(string varName, ParsingScript script, bool testNull = true)
        {
            ParserFunction func = ParserFunction.GetFunction(varName, script);
            if (!testNull && func == null)
            {
                return null;
            }
            Utils.CheckNotNull(varName, func);
            Variable varValue = func.GetValue(script);
            Utils.CheckNotNull(varValue, varName);
            return varValue;
        }

        public static double ConvertToDouble(object obj, ParsingScript script = null)
        {
            string str = obj.ToString();
            double num = 0;

            if (!CanConvertToDouble(str, out num) &&
                script != null)
            {
                ProcessErrorMsg(str, script);
            }
            return num;
        }

        public static bool CanConvertToDouble(string str, out double num)
        {
            return Double.TryParse(str, NumberStyles.Number |
                                        NumberStyles.AllowExponent |
                                        NumberStyles.Float,
                                        CultureInfo.InvariantCulture, out num);
        }

        public static void ProcessErrorMsg(string str, ParsingScript script)
        {
            char ch = script.TryPrev();
            string entity = ch == '(' ? "function":
                            ch == '[' ? "array"   :
                            ch == '{' ? "operand" :
                                        "variable";
            string token    = Constants.GetRealName(str);

            string msg = "Couldn't find " + entity + " [" + token + "].";

            ThrowErrorMsg(msg, script, str);
        }

        public static bool ConvertToBool(object obj)
        {
            string str = obj.ToString();
            double dRes = 0;
            if (CanConvertToDouble(str, out dRes))
            {
                return dRes != 0;
            }
            bool res = false;

            Boolean.TryParse(str, out res);
            return res;
        }
        public static int ConvertToInt(object obj, ParsingScript script = null)
        {
            double num = ConvertToDouble(obj, script);
            return (int)num;
        }

        public static void Extract(string data, ref string str1, ref string str2,
                                   ref string str3, ref string str4, ref string str5)
        {
            string[] vals = data.Split(new char[] { ',', ':' });
            str1 = vals[0];
            if (vals.Length > 1)
            {
                str2 = vals[1];
                if (vals.Length > 2)
                {
                    str3 = vals[2];
                    if (vals.Length > 3)
                    {
                        str4 = vals[3];
                        if (vals.Length > 4)
                        {
                            str5 = vals[4];
                        }
                    }
                }
            }
        }
        public static int GetNumberOfDigits(string data, int itemNumber = -1)
        {
            if (itemNumber >= 0)
            {
                string[] vals = data.Split(new char[] { ',', ':' });
                if (vals.Length <= itemNumber)
                {
                    return 0;
                }
                int min = 0;
                for (int i = 0; i < vals.Length; i++)
                {
                    min = Math.Max(min, GetNumberOfDigits(vals[i]));
                }
                return min;
            }

            int index = data.IndexOf(".");
            if (index < 0 || index >= data.Length - 1)
            {
                return 0;
            }
            return data.Length - index - 1;
        }
        public static void Extract(string data, ref double val1, ref double val2,
                                                ref double val3, ref double val4)
        {
            string[] vals = data.Split(new char[] { ',', ':' });
            val1 = ConvertToDouble(vals[0].Trim());

            if (vals.Length > 1)
            {
                val2 = ConvertToDouble(vals[1].Trim());
                if (vals.Length > 2)
                {
                    val3 = ConvertToDouble(vals[2].Trim());
                }
                if (vals.Length > 3)
                {
                    val4 = ConvertToDouble(vals[3].Trim());
                }
            }
            else
            {
                val3 = val2 = val1;
            }
        }
        public static string GetFileContents(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return "";
            }
            try
            {
                string[] readText = Utils.GetFileLines(filename);
                return string.Join("\n", readText);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return "";
            }
        }

        public static string RemovePrefix(string text)
        {
            string candidate = text.Trim().ToLower();
            if (candidate.Length > 2 && candidate.StartsWith("l'",
                          StringComparison.OrdinalIgnoreCase))
            {
                return candidate.Substring(2).Trim();
            }

            int firstSpace = candidate.IndexOf(' ');
            if (firstSpace <= 0)
            {
                return candidate;
            }

            string prefix = candidate.Substring(0, firstSpace);
            if (prefix.Length == 3 && candidate.Length > 4 &&
               (prefix == "der" || prefix == "die" || prefix == "das" ||
                prefix == "los" || prefix == "las" || prefix == "les"))
            {
                return candidate.Substring(firstSpace + 1);
            }
            if (prefix.Length == 2 && candidate.Length > 3 &&
               (prefix == "el" || prefix == "la" || prefix == "le" ||
                prefix == "il" || prefix == "lo"))
            {
                return candidate.Substring(firstSpace + 1);
            }
            return candidate;
        }

        //static Variable GetVariableForJToken(JToken aToken)
        //{
        //    JTokenType currentType = aToken.Type;
        //    switch (currentType)
        //    {
        //        case JTokenType.Object:
        //            {
        //                Variable newValue = new Variable(Variable.VarType.ARRAY);
        //                ParseJObjectIntoVariable(aToken as JObject, newValue);
        //                return newValue;
        //            }
        //        case JTokenType.Array:
        //            {
        //                Variable newValue = new Variable(Variable.VarType.ARRAY);
        //                foreach (var aa in aToken)
        //                {
        //                    Variable addVariable = GetVariableForJToken(aa);
        //                    newValue.AddVariable(addVariable);
        //                }
        //                return newValue;
        //            }
        //        case JTokenType.Integer:
        //            return new Variable(aToken.ToObject<Int64>());
        //        case JTokenType.Float:
        //            return new Variable(aToken.ToObject<float>());
        //        case JTokenType.String:
        //            return new Variable(aToken.ToObject<String>());
        //        case JTokenType.Boolean:
        //            return new Variable(aToken.ToObject<Boolean>());
        //        case JTokenType.Null:
        //            return Variable.EmptyInstance;
        //        case JTokenType.None:
        //        case JTokenType.Constructor:
        //        case JTokenType.Property:
        //        case JTokenType.Comment:
        //        case JTokenType.Undefined:
        //        case JTokenType.Date:
        //        case JTokenType.Raw:
        //        case JTokenType.Bytes:
        //        case JTokenType.Guid:
        //        case JTokenType.Uri:
        //        case JTokenType.TimeSpan:
        //            return new Variable(aToken.ToString());
        //    }
        //    return Variable.EmptyInstance;
        //}

        //static void ParseJObjectIntoVariable(JObject jsonObject, Variable aVariable)
        //{
        //    foreach (var currentToken in jsonObject)
        //    {
        //        Variable currentVariable = GetVariableForJToken(currentToken.Value);
        //        aVariable.SetHashVariable(currentToken.Key, currentVariable);
        //    }
        //}

        //public static Variable CreateVariableFromJsonString(string aJSONString)
        //{
        //    Variable newValue = new Variable(Variable.VarType.ARRAY);

        //    try
        //    {
        //        JObject jsonObject = JObject.Parse(aJSONString);
        //        ParseJObjectIntoVariable(jsonObject, newValue);
        //    }
        //    catch (Exception e)
        //    {
        //        newValue.SetHashVariable("error", new Variable(true));
        //        newValue.SetHashVariable("message", new Variable(e.Message));
        //    }

        //    return newValue;
        //}

        public static string GetFullPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }
            try
            {
                path = Path.GetFullPath(path);
            }
            catch(Exception exc)
            {
                Console.WriteLine("Exception converting path {0}: {1}", path, exc.Message);
            }
            return path;
        }

        public static string GetDirectoryName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return GetCurrentDirectory();
            }
            try
            {
                return Path.GetDirectoryName(path);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception getting directory name {0}: {1}", path, exc.Message);
            }
            return GetCurrentDirectory();
        }

        public static string GetCurrentDirectory()
        {
            try
            {
                return Directory.GetCurrentDirectory();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception getting current directory: {0}", exc.Message);
            }
            return "";
        }
    }
}
