using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;

namespace Raygust.Core.Parser
{
    public class ArgumentParser
    {
        protected string[] _arguments;
        protected ArgumentConfigurations _configurations;

        public ArgumentParser(string[] arguments, ArgumentConfigurations configurations)
        {
            _arguments = arguments;
            _configurations = configurations;
        }

        public int GetParamIndex(string paramName)
        {
            int result = -1;
            for (int i = 0; i < _arguments.Length; i++)
            {
                if (_arguments[i].Equals(paramName))
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        public string GetParam(string paramName)
        {
            string result = string.Empty;
            int index = GetParamIndex(paramName);
            if (index < 0)
            {
                return result;
            }
            if (index + 1 < _arguments.Length)
            {
                result = _arguments[index + 1];
            }
            return result;
        }

        public bool GetParamAsDouble(string paramName, ref double paramValue)
        {
            string param = GetParam(paramName);
            return double.TryParse(param, out paramValue);
        }

        public bool GetParamAsInt(string paramName, ref int paramValue)
        {
            string param = GetParam(paramName);
            return int.TryParse(param, out paramValue);
        }

        public string[] GetParamArray(string paramName)
        {
            string[] result = null;
            int index = GetParamIndex(paramName);
            if (index < 0 || index + 1 >= _arguments.Length)
            {
                return result;
            }

            result = new string[] { };

            for (int i = index + 1; i < _arguments.Length; i++)
            {
                if (_arguments[i].StartsWith("-"))
                {
                    break;
                }
                Array.Resize<string>(ref result, result.Length + 1);
                result[result.Length - 1] = _arguments[i];
            }

            return result;
        }

        public bool GetParamAsDoubleArray(string paramName, ref double[] paramValues)
        {
            string[] paramArray = GetParamArray(paramName);
            bool result = true;

            if (paramArray == null)
            {
                return false;
            }
            Array.Resize<double>(ref paramValues, paramArray.Length);
            for (int i = 0; i < paramValues.Length; i++)
            {
                if (!double.TryParse(paramArray[i], out paramValues[i]))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public bool GetParamAsIntArray(string paramName, ref int[] paramValues)
        {
            string[] paramArray = GetParamArray(paramName);
            bool result = true;

            if (paramArray == null)
            {
                return false;
            }
            Array.Resize<int>(ref paramValues, paramArray.Length);
            for (int i = 0; i < paramValues.Length; i++)
            {
                if (!int.TryParse(paramArray[i], out paramValues[i]))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public string[] ExtractUnknownArguments()
        {
            string[] result = null;

            if (_configurations.Items.Count == 0)
            {
                return (string[])_arguments.Clone();
            }

            bool isRecognized = false;
            result = new string[] { };

            for (int i = 0; i < _arguments.Length; i++)
            {
                if (_arguments[i].StartsWith("-"))
                {
                    if (_configurations.Items.Exists(r => r.Name.Equals(_arguments[i])))
                    {
                        isRecognized = true;
                        continue;
                    }
                    else
                    {
                        Array.Resize<string>(ref result, result.Length + 1);
                        result[result.Length - 1] = _arguments[i];
                        isRecognized = false;
                    }
                }
                else
                {
                    if (!isRecognized)
                    {
                        Array.Resize<string>(ref result, result.Length + 1);
                        result[result.Length - 1] = _arguments[i];
                    }
                }

            }

            return result;
        }

        public string[] ExtractKnownArguments()
        {
            string[] result = null;

            if (_configurations.Items.Count == 0)
            {
                return (string[])_arguments.Clone();
            }

            bool isRecognized = false;
            result = new string[] { };

            for (int i = 0; i < _arguments.Length; i++)
            {
                if (_arguments[i].StartsWith("-"))
                {
                    if (!_configurations.Items.Exists(r => r.Name.Equals(_arguments[i])))
                    {
                        isRecognized = false;
                        continue;
                    }
                    else
                    {
                        Array.Resize<string>(ref result, result.Length + 1);
                        result[result.Length - 1] = _arguments[i];
                        isRecognized = true;
                    }
                }
                else
                {
                    if (isRecognized)
                    {
                        Array.Resize<string>(ref result, result.Length + 1);
                        result[result.Length - 1] = _arguments[i];
                    }
                }
            }
            return result;
        }
    }
}
